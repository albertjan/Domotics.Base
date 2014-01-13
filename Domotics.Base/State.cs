namespace Domotics.Base
{

    /// <summary>
    /// State description
    /// </summary>
    public class State
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Allows for Convenient conversion from string to Ste
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static implicit operator State (string name)
        {
            return new State { Name = name };
        }

        /// <summary>
        /// Compares states on their names instead of their reference.
        /// </summary>
        /// <param name="left">the state</param>
        /// <param name="right">the state</param>
        /// <returns>a bool</returns>
        public static bool operator ==(State left, State right)
        {
            if (ReferenceEquals (left, right))
            {
                return true;
            }

            if (ReferenceEquals (left, null) || ReferenceEquals (right, null))
            {
                return false;
            }

            return left.Name == right.Name;

        }

        /// <summary>
        /// Compares states on their names instead of their reference.
        /// </summary>
        /// <param name="left">the state</param>
        /// <param name="right">the state</param>
        /// <returns>a bool</returns>
        public static bool operator !=(State left, State right)
        {
            return !(left == right);
        }

        /// <inherit-doc/>
        public override bool Equals (object obj)
        {
            if (obj == null) return false;
            var state = obj as State;
            if (state == null) return false;
            return state.Name == Name;
        }

        public string Type { get { return "test"; } }

        /// <inherit-doc/>
        public override int GetHashCode ()
        {
            return Name.GetHashCode();
        }
    } 
}