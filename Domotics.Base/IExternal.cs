using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domotics.Base
{
    public interface IExternal
    {
        event EventHandler Input;

        IEnumerable<Connection> Connections
        {
            get;
            set;
        }
    }
}
