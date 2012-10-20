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
            return right != null && left != null && left.Name == right.Name;
        }

        public static bool operator !=(State left, State right)
        {
            return !(left == right);
        }

        public static bool operator ==(State left, string right)
        {
            return left != null && left.Name == right;
        }

        public static bool operator !=(State left, string right)
        {
            return !(left == right);
        }
    } 
}