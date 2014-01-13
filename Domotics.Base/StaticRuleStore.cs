using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// a basic static rulestore.
    /// first to be implemented.
    /// </summary>
    public class StaticRuleStore : IRuleStore
    {
        /// <summary>
        /// Initializer the rulestore
        /// </summary>
        public StaticRuleStore()
        {
            //CompiledRules = new List<Rule> {new Rule("", new[] {"knopje", "lampje"})};
        }

        private List<Rule> CompiledRules { get; set; }
        
        /// <inherit-doc/>
        public IEnumerable<Rule> Rules 
        { 
            get
            {
                return CompiledRules;
            } 
        }

        /// <inherit-doc/>
        public Distributor Distributor { set; private get; }

        /// <inherit-doc/>
        public bool AddRule(Rule rule)
        {
            throw new System.NotImplementedException();
        }
    }
}
