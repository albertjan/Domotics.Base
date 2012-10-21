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
        /// A longer name
        /// </summary>
        public string Description { get; set; }

        public static implicit operator State (string name)
        {
            return new State { Name = name };
        }

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

        public static bool operator !=(State left, State right)
        {
            return !(left == right);
        }

        public override bool Equals (object obj)
        {
            if (obj == null) return false;
            var state = obj as State;
            if (state == null) return false;
            return state.Name == Name;
        }

        public override int GetHashCode ()
        {
            return Name.GetHashCode();
        }
    } 
}