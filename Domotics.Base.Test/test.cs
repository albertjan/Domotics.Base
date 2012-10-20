using Domotics.Base.DSL;
using System.Collections.Generic;
namespace Domotics.Base.Generated
{
    public class ywddg1rf5e : IRuleLogic
    { 
        public StateChangeDirective GetNewState(State input, Connection connection, List<Connection> connections)
        { 
            return new Logic(input, connection, connections).When("knopje").IsPushed().Turn("lampje")("on");
        }
    }
}