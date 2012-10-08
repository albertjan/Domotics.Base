using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// a basic static rulestore.
    /// first to be implemented.
    /// </summary>
    public class StaticRuleStore : IRuleStore
    {
        public StaticRuleStore()
        {
            CompiledRules = new List<Rule> {new Rule("turn on//", new[] {"knopje", "lampje"})};
        }

        private List<Rule> CompiledRules { get; set; }
        
        public IEnumerable<Rule> Rules 
        { 
            get
            {
                return CompiledRules;
            } 
        }
    }
}
