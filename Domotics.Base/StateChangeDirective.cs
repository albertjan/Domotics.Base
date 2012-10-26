namespace Domotics.Base
{
    /// <summary>
    /// A Directive to change the state of a specific connection.
    /// </summary>
    public class StateChangeDirective
    {
        private static readonly StateChangeDirective _stateChangeDirective;

        static StateChangeDirective()
        {
            _stateChangeDirective = new StateChangeDirective();
        }
    
        /// <summary>
        /// the new state
        /// </summary>
        public State NewState { get; set; }
        /// <summary>
        /// of this connection.
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// Is return when no change is required.
        /// </summary>
        public static StateChangeDirective NoOperation { get { return _stateChangeDirective; } }
    }
}