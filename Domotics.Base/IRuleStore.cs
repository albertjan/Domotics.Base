using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// A thing that stores rules.
    /// </summary>
    public interface IRuleStore
    {
        /// <summary>
        /// a list of rules it has.
        /// </summary>
        IEnumerable<Rule> Rules { get; }

        Distributor Distributor { set; }

        bool AddRule(Rule rule);
    }
}
