namespace Domotics.Base
{
    /// <summary>
    /// A Directive to change the state of a specific connection.
    /// </summary>
    public class StateChangeDirective
    {
        /// <summary>
        /// the new state
        /// </summary>
        public IState NewState { get; set; }
        /// <summary>
        /// of this connection.
        /// </summary>
        public Connection Connection { get; set; }
    }
}