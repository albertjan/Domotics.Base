using System;
using System.Collections.Generic;
using Domotics.Base;

namespace Domotics.RuleStore.CouchDB
{
    public class Store : IRuleStore
    {
        public IEnumerable<Rule> Rules { get; private set; }
        public Distributor Distributor { set; private get; }
        public bool AddRule(Rule rule)
        {
            throw new NotImplementedException();
        }
    }
}
