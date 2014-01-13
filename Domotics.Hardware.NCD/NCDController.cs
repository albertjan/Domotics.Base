using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using Domotics.Base;
using NCD;

namespace Domotics.Hardware.NCD
{
    public class NCDController : IDisposable
    {
        private IExternalSource External { get; set; }

        public NCDController (IExternalSource external)
        {
            External = external;
            CurrentInputState = new Dictionary<int, IEnumerable<bool>>();
            CurrentOutputState = new Dictionary<int, IEnumerable<bool>>();
            OutputStack = new Stack<ushort>();
            InputStack = new Stack<ushort>();
        }

        #region Controllerthread

        private static void Runner (object ncdController)
        {
            if (ncdController == null) throw new ArgumentNullException ("ncdController");
            var controller = (NCDController)ncdController;
            try
            {
                while (true)
                {
                    if (controller.OutputStack.Count > 0)
                    {
                        //  on/off banknumber    relay
                        //  0-1    0-32          0-7
                        // | 0000 | 0000 | 0000 | 0000 |

                        //  turn relay 4 on bank 3 on
                        // | 0001 | 0000 | 0011 | 0100 |

                        var input = controller.OutputStack.Pop ();
                        var relay = (byte)(input & 15);
                        var bank = (byte)((input & 4080) >> 4);
                        var status = (byte)(input >> 12);
                        if (status == 0)
                        {
                            
                            controller.NCDComponent.ProXR.RelayBanks.SelectBank(bank);
                            controller.NCDComponent.ProXR.RelayBanks.TurnOffRelay(relay);
                            //controller.NCDComponent.ProXR.RelayBanks.TurnOffRelayInBank(relay, bank);
                        }
                        else
                        {
                            controller.NCDComponent.ProXR.RelayBanks.SelectBank (bank);
                            controller.NCDComponent.ProXR.RelayBanks.TurnOnRelay (relay);
                            //controller.NCDComponent.ProXR.RelayBanks.TurnOnRelayInBank (relay, bank);
                        }
                    }
                    else
                    {
                        //--------Input states
                        //| bank        |       value |
                        //| 0000 | 0000 | 0000 | 0000 |

                        for (var contactClosureBank = 0;
                             contactClosureBank < BasicConfiguration.Configuration.NumberOfContactClosureBanks;
                             contactClosureBank++)
                        {
                            controller.InputStack.Push((ushort)(((byte) contactClosureBank << 8) +
                                 controller.NCDComponent.ProXR.Scan.ScanValue((byte) contactClosureBank)));
                        }
                    }
                }      
            }
            catch(ThreadAbortException)
            {
                
            }
        }

        private Thread Run { get; set; }

        #endregion

        #region Inputthread

        private Stack<ushort> InputStack { get; set; }

        private static void InputRunner(object ncdController)
        {
            var controller = (NCDController) ncdController;

            try
            {
                while (true)
                {
                    if (controller.InputStack.Count > 0)
                    {
                        var val = controller.InputStack.Pop();
                        var bank = (val & 0xFF00) >> 8;
                        var value = (byte)(val & 0x00FF);
                        controller.ReportInputStates(bank, ParseValue(value));
                    }   
                    Thread.Sleep(5);
                }
            }
            catch(ThreadAbortException)
            {
                
            }
        }

        private IDictionary<int, IEnumerable<bool>> CurrentInputState { get; set; }

        private IDictionary<int, IEnumerable<bool>> CurrentOutputState { get; set; }

        private void ReportOutputStates (int bank, IEnumerable<bool> states)
        {
            if (CurrentOutputState.ContainsKey (bank))
            {
                var curBankState = CurrentOutputState[bank].ToList();
                var inpState = states.ToList();

                for (var i = 0; i < 8; i++)
                {
                    if (curBankState.ElementAt(i) != inpState.ElementAt(i))
                    {
                        CurrentOutputState[bank] = inpState;
                    }
                }
            }
            else
            {
                CurrentOutputState.Add (bank, states);
            }
        }

        private void ReportInputStates (int bank, IEnumerable<bool> states)
        {
            if (CurrentInputState.ContainsKey(bank))
            {
                var curBankState = CurrentInputState[bank].ToList();
                var inputState = states.ToList();

                for (var i = 0; i < 8; i++)
                {
                    if (curBankState.ElementAt(i) != inputState.ElementAt(i))
                    {
                        CurrentInputState[bank] = inputState;
                        //fire input event
                    }
                }
            }
            else
            {
                CurrentInputState.Add(bank, states);
            }
        }

        private static IEnumerable<bool> ParseValue(byte value)
        {
            return new[]
                   {
                       (value & 1) > 0,
                       (value & 2) > 0,
                       (value & 4) > 0,
                       (value & 8) > 0,
                       (value & 16) > 0,
                       (value & 32) > 0,
                       (value & 64) > 0,
                       (value & 128) > 0
                   };
        }

        private Thread Input { get; set; }

        #endregion

        private Stack<ushort> OutputStack { get; set; }

        private NCDComponent NCDComponent { get; set; }

        public void Initialize()
        {
            var configfile = ConfigurationManager.AppSettings["ConfigurationFilePath"];
            if (File.Exists(configfile))
                BasicConfiguration.Load(configfile);
            else
            {
                new BasicConfiguration
                    {
                        Path = configfile
                    }.FillExample();
                
                BasicConfiguration.Save();
                throw new Exception("EmptyConfigurationException, Please fill the configuration with the right information and start again.");
            }
            NCDComponent = new NCDComponent {BaudRate = 38400, PortName = BasicConfiguration.Configuration.Comport};
            //ncdComponent.Port = 1;
            NCDComponent.OpenPort();
            if (!NCDComponent.IsOpen) throw new Exception("Can't open port");

            //On boot load all the states into the outputendpoints via the state mapper. 
            //Select the hardware states and report those IN ORDER  to the endpoint state mapper.
            //To get the current state.

            //-------Output states
            var outputState = NCDComponent.ProXR.RelayBanks.GetRelaysStatusInAllBanks ().Take (BasicConfiguration.Configuration.AvailableRelayBanks.Count).ToList ();
            for (var bank = 0; bank < outputState.Count (); bank++)
            {
                ReportOutputStates (bank + 1, ParseValue (outputState[bank]));
            }
        }

        //private void WaitForState()
        //{
        //    while (CurrentOutputState.Count == 0 || CurrentInputState.Count == 0) Thread.Sleep(100);
        //}

        public void Start()
        {
            Run = new Thread(Runner);
            Run.Start(this);
            Input = new Thread(InputRunner);
            Input.Start(this);
        }


        private NCDEndPointCouplingInformation CouplingInformation { get; set; }

        public IEnumerable<NCDHardwareIdentifier> GetIdentifiers()
        {
            for (byte i = 0; i < BasicConfiguration.Configuration.NumberOfContactClosureBanks; i++)
            {
                for (byte j = 0; j < 8; j++)
                {
                    yield return new NCDHardwareIdentifier
                                     {
                                         Bank = i,
                                         Unit = j,
                                         Type = HardwareEndpointType.Input
                                     };
                }
            }
            foreach (var relayBank in BasicConfiguration.Configuration.AvailableRelayBanks)
            {
                for (byte i = 0; i < relayBank.AvailableRelays; i++)
                {
                    yield return new NCDHardwareIdentifier
                                     {
                                         Bank = (byte) relayBank.Number,
                                         Unit = i,
                                         Type = HardwareEndpointType.Output
                                     };
                }
            }
        }

        private static Dictionary<int, bool> SelectState(Tuple<string, ICoupleLogic, IEnumerable<NCDHardwareIdentifier>> endpoint, IEnumerable<KeyValuePair<int, IEnumerable<bool>>> currentState)
        {
            var retval = new Dictionary<int, bool>();
            foreach (var hardwareEndpointIndentifier in endpoint.Item3)
            {
                var bank = hardwareEndpointIndentifier.Bank;
                var relayid = hardwareEndpointIndentifier.Unit;
                retval.Add(currentState.First(kv => kv.Key == bank).Key,
                           currentState.First(kv => kv.Key == bank).Value.ElementAt(relayid));
            }
            return retval;
        } 

        public void Dispose()
        {
            BasicConfiguration.Save();
            Run.Abort();
            Input.Abort();
        }
    }

    public enum HardwareEndpointType
    {
        Input, Output
    }
}
