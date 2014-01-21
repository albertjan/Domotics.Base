namespace Domotics.Hardware.NCD
{
    using global::NCD;

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Base;

    using Couple = System.Tuple<Base.Connection, CoupleLogic.ICoupleLogic, System.Collections.Generic.List<NCDHardwareIdentifier>>;
    
    public class NCDController : IDisposable
    {
        private List<Couple> _endpointCouples; 
        private Dictionary<Tuple<byte, byte>, Couple> EndpointCoupleDictionary { get; set; }
        private NCDExternalSource External { get; set; }

        public NCDController (IExternalSource external)
        {
            External = (NCDExternalSource) external;
            CurrentInputState = new Dictionary<byte, IEnumerable<bool>>();
            CurrentOutputState = new Dictionary<byte, IEnumerable<bool>>();
            OutputStack = new Stack<ushort>();
            InputStack = new Stack<ushort>();
            EndpointCoupleDictionary = new Dictionary<Tuple<byte, byte>, Couple>();
        }

        #region Controllerthread
        
        public List<Couple> EndpointCouples
        {
            get { return _endpointCouples; }
            set
            {
                _endpointCouples = value;
                EndpointCoupleDictionary.Clear();
                foreach (var tuple in value)
                {
                    foreach (var ncdHardwareIdentifier in tuple.Item3)
                    {
                        EndpointCoupleDictionary.Add(
                            Tuple.Create(ncdHardwareIdentifier.Bank, ncdHardwareIdentifier.Unit), tuple);
                    }
                }
            }
        }

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
                        var bank = (byte)((val & 0xFF00) >> 8);
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

        private IDictionary<byte, IEnumerable<bool>> CurrentInputState { get; set; }

        private IDictionary<byte, IEnumerable<bool>> CurrentOutputState { get; set; }

        private void ReportOutputStates (byte bank, IEnumerable<bool> states)
        {
            if (CurrentOutputState.ContainsKey (bank))
            {
                var curBankState = CurrentOutputState[bank].ToList();
                var inpState = states.ToList();

                for (byte i = 0; i < 8; i++)
                {
                    if (curBankState[i] != inpState[i])
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

        private void ReportInputStates (byte bank, IEnumerable<bool> states)
        {
            if (CurrentInputState.ContainsKey(bank))
            {
                var curBankState = CurrentInputState[bank].ToArray();
                var inputState = states.ToArray();

                for (byte i = 0; i < 8; i++)
                {
                    var tuple = Tuple.Create(bank, i);

                    if (!EndpointCoupleDictionary.ContainsKey(tuple)) continue;
                    
                    var couple = EndpointCoupleDictionary[tuple];

                    if (!couple.Item1.Live)
                    {
                        if (curBankState[i] != inputState[i])
                        {
                            CurrentInputState[bank] = inputState;
                        }
                        External.FireInputEvent(couple.Item1.Name, inputState[i] ? "In" : "Out");
                    }
                    else
                    {
                        if (curBankState[i] != inputState[i])
                        {
                            CurrentInputState[bank] = inputState;
                            External.FireInputEvent(couple.Item1.Name, inputState[i] ? "In" : "Out");
                        }
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
            
            NCDComponent = new NCDComponent { BaudRate = 38400, PortName = BasicConfiguration.Configuration.Comport };
            NCDComponent.OpenPort();
            if (!NCDComponent.IsOpen) throw new Exception("Can't open port");

            //-------Output states
            var outputState = NCDComponent.ProXR.RelayBanks.GetRelaysStatusInAllBanks();
            
            for (byte bank = 0; bank < BasicConfiguration.Configuration.AvailableRelayBanks.Count; bank++)
            {
                ReportOutputStates (bank, ParseValue (outputState[bank]));
            }

            Run = new Thread(Runner);
            Run.Start(this);
            Input = new Thread(InputRunner);
            Input.Start(this);
        }

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
                                         Type = ConnectionType.In
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
                                         Type = ConnectionType.Out
                                     };
                }
            }
        }

        private static Dictionary<int, bool> SelectState(Couple endpoint, Dictionary<int, List<bool>> currentState)
        {
            var retval = new Dictionary<int, bool>();
            foreach (var hardwareEndpointIndentifier in endpoint.Item3)
            {
                var bank = hardwareEndpointIndentifier.Bank;
                var relayid = hardwareEndpointIndentifier.Unit;
                retval.Add(bank, currentState[bank][relayid]);
            }
            return retval;
        } 

        public void SetState(Connection connection, State state)
        {
            var couple = EndpointCouples.FirstOrDefault(e => e.Item1 == connection);
            if (couple == null) return;
            
            foreach (var ncdControlMessage in couple.Item2.GetMessages(state, couple.Item3))
            {
                OutputStack.Push(ncdControlMessage.GetMessage());
            }
        }

        public void Dispose()
        {
            BasicConfiguration.Save();
            Run.Abort();
            Input.Abort();
        }
    }
}
