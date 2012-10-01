using System.Collections.Generic;
using System.Linq;

namespace Domotics.Base
{
    /// <summary>
    /// distributes changes in events to the rules that belong to it.
    /// </summary>
    public class Distributor
    {
        public Distributor(IEnumerable<IExternal> externalSources, IEnumerable<IRuleStore> ruleStores)
        {
            Externals = externalSources;
            RuleStores = ruleStores;
            foreach (var externalSource in externalSources)
            {
                externalSource.Input += ExternalSourceInput;
            }
        }

        /// <summary>
        /// all the rules from all the stores.
        /// </summary>
        private IEnumerable<Rule> AllRules { get { return RuleStores.SelectMany(r => r.Rules); } }
 
        /// <summary>
        /// Handles when a statechage is noticed by the external source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ExternalSourceInput (object sender, ConnectionStateChangedEventHandlerArgs args)
        {
            //find all the rules that say something this connection.
            foreach (var r1 in AllRules.Where(r => r.Connections.Any(c => c.ID == args.ConnectionID)))
            {
                var external = (IExternal) sender;
                //get the statechange directive from the rule
                var statechangedirective = r1.Fire(r1.Connections.First(r => r.ID == args.ConnectionID), args.OldState);
                //set the state on the external source.
                external.SetState(statechangedirective.Connection.ID, statechangedirective.NewState.Name);
            }
        }

        /// <summary>
        /// list of external sources
        /// </summary>
        public IEnumerable<IExternal> Externals { get; set; }

        /// <summary>
        /// list of stores of rules
        /// </summary>
        public IEnumerable<IRuleStore> RuleStores { get; set; }
    }
}
