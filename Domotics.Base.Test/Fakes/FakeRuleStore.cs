using System.Collections.Generic;
using System.Linq;

namespace Domotics.Base.Test.Fakes
{
    public class FakeRuleStore : IRuleStore
    {
        public FakeRuleStore ()
        {
            TestRules = new List<Rule>();
        }

        private List<Rule> TestRules { get; set; } 

        public IEnumerable<Rule> Rules { get { return TestRules; } }
        public Distributor Distributor { set; private get; }

        public bool AddRule(Rule rule)
        {
            rule.Connections = Distributor.ResolveConnections(rule.Connections.Select(c => c.ToString()));
            TestRules.Add(rule);
            return true;
        }
    }
}