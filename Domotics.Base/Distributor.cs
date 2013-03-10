using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Domotics.Base
{
    /// <summary>
    /// distributes changes in events to the rules that belong to it.
    /// </summary>
    public class Distributor
    {
        /// <summary>
        /// Create the Distributor.
        /// </summary>
        /// <param name="externalSources">the external sources.</param>
        /// <param name="ruleStores">the stores for the rules</param>
        public Distributor(IEnumerable<IExternalSource> externalSources, IEnumerable<IRuleStore> ruleStores)
        {
            ExternalSources = externalSources;
            RuleStores = ruleStores;

            foreach (var ruleStore in RuleStores)
            {
                ruleStore.Distributor = this;
            }

            //subscribe to all input events on all external sources. And call the delegate.
            foreach (var externalSource in ExternalSources)
            {
                externalSource.Input += ExternalSourceInput;


                if (externalSource.DistributorInitializationDelegate != null)
                    externalSource.DistributorInitializationDelegate(this);
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
            //find all the rules that say something this connection. fire then collect the changes to be made to the states.
            var statechangedirectives =
                AllRules.Where (r => r.Connections.Any(c => c.Name == args.Connection.Name))
                        .Select(r => r.Fire(r.Connections.First(r1 => r1.Name == args.Connection.Name), args.NewState))
                        .SelectMany(s => s);


            //do not only send the message back to the source it came from also check if others want to know
            //about it.
            var externals = ExternalSources.SelectMany(e => e.Connections, (e1, c1) => new { ExternalSource = e1, Connection = c1 }).ToList();
            
            foreach (var stateChangeDirective in statechangedirectives.Where(s => s != StateChangeDirective.NoOperation))
            {
                var directive = stateChangeDirective;
                foreach (var external in externals.Where(e => e.Connection.Name == directive.Connection.Name).Select(e => e.ExternalSource))
                {
                    external.SetState (directive.Connection, directive.NewState.Name);
                }
            }

            if (args.Connection.Copied)
                externals.First(t => t.Connection.Name == args.Connection.Name && !t.Connection.Copied).ExternalSource.
                    SetState(args.Connection, args.NewState.Name);
            else
                ((IExternalSource)sender).SetState(args.Connection, args.NewState.Name);
        }

        /// <summary>
        /// list of external sources
        /// </summary>
        public IEnumerable<IExternalSource> ExternalSources { get; set; }

        /// <summary>
        /// list of stores of rules
        /// </summary>
        public IEnumerable<IRuleStore> RuleStores { get; set; }

        /// <summary>
        /// resolve the connections to their real ones by their name.
        /// </summary>
        /// <param name="connectionNames">connection names</param>
        /// <returns>a list of connections.</returns>
        public IEnumerable<Connection> ResolveConnections(IEnumerable<string> connectionNames)
        {
            return ExternalSources.SelectMany(e => e.Connections)
                                  .Where(c => !c.Copied)
                                  .Join(connectionNames, c => c.Name, s => s, (c, s) => c)
                                  .ToList();
        }
    }
}
