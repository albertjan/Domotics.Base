using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Domotics.Base.DSL
{
    //Item1 = The connection that caused the rule to fire
    //Item2 = The new state
    //Item3 = A bool that causes the rule to return null when set to false. i.e. no change.
    //Item4 = LastTriggered in ticks.
    //Item5 = The connection whos state is to change
    using ConnectionState = Tuple<Connection, State, bool, IEnumerable<Connection>, long, Connection>;

    /// <summary>
    /// The base for the user rule stories
    /// </summary>
    public class Logic
    {
        private State Input { get; set; }
        private Connection Connection { get; set; }
        private List<Connection> Connections { get; set; }
        private long LastTriggered { get; set; }
        private Connection AffectedConnection { get; set; }
        private bool ShouldContinue { get; set; }
        /// <summary>
        /// The StateChangeDirectives collected by the rule.
        /// </summary>
        public List<StateChangeDirective> CollectedStateChanges { get; private set; }

        /// <summary>
        /// The start of the story collecting all the 
        /// </summary>
        /// <param name="input">The state to which the input is to change after this story.</param>
        /// <param name="connection">The connection that caused this rule to fire.</param>
        /// <param name="connections">The connection that this rule talks about.</param>
        /// <param name="lastTriggered">The last time the rule was triggered.</param>
        public Logic (State input, Connection connection,  List<Connection> connections, long lastTriggered)
        {
            Input = input;
            Connection = connection;
            Connections = connections;
            LastTriggered = lastTriggered;
            CollectedStateChanges = new List<StateChangeDirective>();
        }

        /// <summary>
        /// The start of a rule story.
        /// </summary>
        /// <param name="connectionName">the name of the input you want it to react on.</param>
        /// <returns>the ConnectionState</returns>
        /// <exception cref="LogicException">Throws when the selected connection is not present in the list of connections.</exception>
        public Logic When (string connectionName)
        {
            var selectedConnection = Connections.FirstOrDefault(c => c.Name == connectionName);
            if (selectedConnection == null) 
                throw new LogicException("Connection this rule should fire (" + connectionName + ") on is not listed in the connections (" + Connections.Aggregate("", ((s, c) => s + c.Name + "," )) + ").");

            ShouldContinue = Connection.Name == connectionName;

            return this;
        }
    
        /// <summary>
        /// Continues the story when the input connection selected with When is pushed for less then 500 ms.
        /// </summary>
        /// <returns>The ConnectionState</returns>
        public Logic IsPushed ()
        {
            return ChangesStateWithin ("in", "out", 500);
        }

        /// <summary>
        /// Continues the story when the first connection selector `When` has not selected an connection, and when this selector does select a connection. Effectively performs an exclusive or, but in non formal language `Or` is most of the time exclusive.
        /// </summary>
        /// <param name="connectionName">the name of the connection to be selected</param>
        /// <returns>the connectionstate</returns>
        /// <exception cref="LogicException">When the connection selected upon is not present.</exception>
        public Logic OrWhen (string connectionName)
        {
            var selectedConnection = Connections.FirstOrDefault(c => c.Name == connectionName);

            if (selectedConnection == null)
                throw new LogicException("Connection this rule should fire (" + connectionName +
                                         ") on is not listed in the connections (" +
                                         Connections.Aggregate("", ((s, c) => s + "," + c.Name)) + ").");

            ShouldContinue = Connection.Name == connectionName ^ ShouldContinue;

            return this;
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is held for atleast 500ms.
        /// </summary>
        /// <returns>the ConnectionState</returns>
        public Logic IsHeld ()
        {
            return IsHeldFor(500);
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is held for the specfied number of milliseconds.
        /// </summary>
        /// <param name="millisecs">the number of milliseconds after which the story is continued.</param>
        /// <returns>the ConnectionState</returns>
        public Logic IsHeldFor (int millisecs)
        {
            return ChangesStateAfter ("in", "out", millisecs);
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is pushed for less than the specified number of milliseconds and when the state changes from From to To.
        /// </summary>
        /// <param name="from">from State</param>
        /// <param name="to">to State</param>
        /// <param name="millisecs">the specified number of milliseconds before which the state has to change.</param>
        /// <returns>the ConnectionState</returns>
        public Logic ChangesStateWithin (State from, State to, int millisecs)
        {
            if (Connection.Type == ConnectionType.In || Connection.Type == ConnectionType.Both)
            {
                Debug.WriteLine ("LT: " + LastTriggered + ", N: " + DateTime.Now.Ticks + ", D: " + (DateTime.Now.Ticks - LastTriggered) + ", " + (LastTriggered + (millisecs * 10000) > DateTime.Now.Ticks));

                ShouldContinue  = ShouldContinue && ((Connection.CurrentState == from && Input == to) &&
                                           LastTriggered + (millisecs*10000) > DateTime.Now.Ticks);


                return this;
            }
            throw new LogicException ("Output Connections cant be \"Pushed\"");
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is pushed for atleast the specified number of milliseconds and when the state changes from From to To.
        /// </summary>
        /// <param name="from">from State</param>
        /// <param name="to">to State</param>
        /// <param name="millisecs">the specified number of milliseconds before which the state has to change.</param>
        /// <returns>the ConnectionState</returns>
        public Logic ChangesStateAfter (State from, State to, int millisecs)
        {
            if (Connection.Type == ConnectionType.In || Connection.Type == ConnectionType.Both)
            {
                ShouldContinue = ShouldContinue && ((Connection.CurrentState == from && Input == to) &&
                                           LastTriggered + (millisecs*10000) < DateTime.Now.Ticks);

                Debug.WriteLine ("LT: " + LastTriggered + ", N: " + DateTime.Now.Ticks + ", D: " + (DateTime.Now.Ticks - LastTriggered) + ", " + (LastTriggered + (millisecs * 10000) < DateTime.Now.Ticks));
                return this;
            }
            throw new LogicException ("Output Connections cant be \"Pushed\"");
        }

        /// <summary>
        /// Conveniance method changes `Turn("name").OnWhenItsOff().OffWhenItsOn()` to `Switch()`
        /// Switches from on to off and the other way round.
        /// </summary>
        /// <param name="connectionName">the name of the affected connection</param>
        /// <returns>A list of changes to be made to the state of the connections.</returns>
        public Logic Switch(string connectionName)
        {
            return Turn(connectionName).OnWhenItsOff().OffWhenItsOn();
        } 
        
        /// <summary>
        /// Adds a statechangedirective when the state is from and changes it to to.
        /// </summary>
        /// <param name="from">when to change the state</param>
        /// <param name="to">to what to change the state</param>
        /// <returns>the current state.</returns>
        public Logic Change(State from, State to)
        {
            if (AffectedConnection.CurrentState == from && ShouldContinue)
            {
                CollectedStateChanges.Add(new StateChangeDirective
                                              {
                                                  Connection = AffectedConnection,
                                                  NewState = to
                                              });
            }
            return this;
        }

        /// <summary>
        /// Conveniance method. for single action buttons.
        /// </summary>
        /// <returns>the state</returns>
        public Logic OnWhenItsOff()
        {
            return Change("off", "on");
        }

        /// <summary>
        /// another conveniance method for single action buttons.
        /// </summary>
        /// <returns>the state</returns>
        public Logic OffWhenItsOn ()
        {
            return Change("on", "off");
        }
        
        /// <summary>
        /// Returns a func that can be called to return the StateChangeDirectives that are a result from the story.
        /// </summary>
        /// <param name="which">a selector to select </param>
        /// <returns>the Func that can be called.</returns>
        public Logic Turn (string which)
        {
            var selectedConnection = Connections.FirstOrDefault (c => c.Name == which);
            if (selectedConnection == null)
                throw new LogicException ("Connection this rule should fire (" + which + ") on is not listed in the connections (" + Connections.Aggregate ("", ((s, c) => s + c.Name + ",")) + ").");

            AffectedConnection = selectedConnection;

            return this;
        }

        
    }

    /// <summary>
    /// An exception that fires when there is a logical error. For example: when an output connection is pushed.
    /// </summary>
    [Serializable]
    public class LogicException : Exception
    {
        /// <inherit-doc/>
        public LogicException(string message) : base (message)
        {
            
        }
    }
}
