using System.Collections.Generic;

namespace Domotics.Base.Test.Fakes
{
    public class FakeRuleStore : IRuleStore
    {
        public List<Rule> TestRules { get; set; } 

        public IEnumerable<Rule> Rules { get { return TestRules; } }
    }
}