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
            var rule = new Rule(@"When(""knopje"").IsPushed().Turn(""lampje"")(""on"")", new[] {"knopje", "lampje"});
            Distributor.RuleStores.First().AddRule (rule);

            Assert.IsTrue(rule.Check());
        }

        [Test]
        public void SimpleRuleFireTest ()
        {
            var rule = new Rule (@"When(""knopje"").IsPushed().Turn(""lampje"")(""on"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);
            var chd = rule.Fire(new Connection("knopje", ConnectionType.In) {CurrentState = "out"}, "in");
            Assert.AreEqual("lampje",chd.Connection.Name);
            Assert.AreEqual("on", chd.NewState.Name);
        }
    }
}
