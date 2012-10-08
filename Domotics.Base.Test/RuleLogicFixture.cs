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
        public void SimpleRuleTest ()
        {
            var rule = new Rule(@"When(""knopje"").IsPushed().Turn(""lampje"")(""on"")", new[] {"knopje", "lampje"});
            ((List<Rule>)Distributor.RuleStores.First().Rules).Add (rule);
            Assert.IsTrue(rule.Check());
        }

    }
}
