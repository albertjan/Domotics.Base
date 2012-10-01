namespace Domotics.Base
{

    /// <summary>
    /// State description
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// The name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A longer name
        /// </summary>
        string Description { get; set; }
    }
}