using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using Domotics.Base.Test.Fakes;
using FakeItEasy;
using NUnit.Framework;

namespace Domotics.Base.Test
{
    [TestFixture]
    public class RuleLogicFixture
    {
        [SetUp]
        public void Init ()
        {
            Distributor = new Distributor(new[] {new FakeExternalSource()}, new[] {new FakeRuleStore()});
        }

        private Distributor Distributor { get; set; }

        [Test]
        public void SimpleRuleCheckTest ()
        {
            //given
            var rule = new Rule(@"When(""knopje"").IsPushed().Turn(""lampje"")(""on"")", new[] {"knopje", "lampje"});

            //when
            Distributor.RuleStores.First().AddRule (rule);

            //then
            Assert.IsTrue(rule.Check());
        }

        [Test]
        public void SimpleRuleFireTest ()
        {
            //given
            var rule = new Rule (@"When(""knopje"").IsPushed().Turn(""lampje"")(""on"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);
            Distributor.ExternalSources.First().Connections.First(c => c.Name == "knopje").CurrentState = "in";

            //when
            var chd = rule.Fire(new Connection("knopje", ConnectionType.In) {CurrentState = "in"}, "out");
            
            //then
            Assert.AreEqual("lampje",chd.Connection.Name);
            Assert.AreEqual("on", chd.NewState.Name);
        }

        [Test]
        public void InputEventRuleFireTest ()
        {
            //given
            var rule = new Rule (@"When(""knopje"").IsPushed().Turn(""lampje"")(""on"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);
            
            //when
            ((FakeExternalSource) Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            ((FakeExternalSource) Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "out");
            
            //then
            Assert.AreEqual("on", Distributor.ExternalSources.First().Connections.First(c => c.Name == "lampje").CurrentState.Name);
        }

        [Test]
        public void RuleChangesStateTest ()
        {
            //given
            var rule = new Rule (@"When(""knopje"").ChangesState(""out"",""in"").Turn(""lampje"")(""on"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);
            
            //when
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            
            //then
            Assert.AreEqual ("on", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje").CurrentState.Name);
        }
    }
}
