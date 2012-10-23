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

        /// <summary>
        /// The distributor it belongs to
        /// </summary>
        Distributor Distributor { set; }

        /// <summary>
        /// A function to be called if you want to add a rule to the rulestore
        /// </summary>
        /// <param name="rule">the rule to add.</param>
        /// <returns>if the rule is complete and valid and it compiles.</returns>
        bool AddRule(Rule rule);
    }
}
