using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domotics.Base
{
    /// <summary>
    /// a basic static rulestore.
    /// first to be implemented.
    /// </summary>
    public class StaticRuleStore : IRuleStore
    {
        public List<Rule> Rules { get; set; }
    }
}
