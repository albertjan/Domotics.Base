using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domotics.Base
{
    public class Distributor
    {
        public IEnumerable<IExternal> Externals
        {
            get
            {
                throw new System.NotImplementedException ();
            }
            set
            {
            }
        }

        public IEnumerable<IRuleStore> RuleStores
        {
            get
            {
                throw new System.NotImplementedException ();
            }
            set
            {
            }
        }
    }
}
