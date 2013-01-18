using System;
using System.Linq;
using Domotics.Base.Test.Fakes;
using NUnit.Framework;

namespace Domotics.Base.Test
{
    [TestFixture]
    public class DistributorFixture
    {
        [SetUp]
        public void Init()
        {
            FakeExternalSource = new FakeExternalSource();
            Distributor = new Distributor(new[] { FakeExternalSource }, new[] { new FakeRuleStore() });
        }

        private FakeExternalSource FakeExternalSource { get; set; }

        private Distributor Distributor { get; set; }

        [Test]
        public void InputEventHandlerTest()
        {
            //given
            Assert.AreEqual ("out", FakeExternalSource.Connections.First (c => c.Name == "knopje").CurrentState.Name);
            
            //when
            FakeExternalSource.FireInputEvent("knopje", "in");
        
            //then
            Assert.That ("in" == FakeExternalSource.Connections.First (c => c.Name == "knopje").CurrentState.Name, Is.True.After (100));
        }

        [Test]
        public void InputEventHandlerTestTwee()
        {
            //given
            Assert.AreEqual("out", FakeExternalSource.Connections.First(c => c.Name == "knopje").CurrentState.Name);

            //when
            FakeExternalSource.FireInputEvent("knopje", "in");
            FakeExternalSource.FireInputEvent("knopje", "out");

            //then
            Assert.That("out" == FakeExternalSource.Connections.First(c => c.Name == "knopje").CurrentState.Name, Is.True.After(100));
        }

        [Test]
        public void AddRuleTest()
        {
            //given
            var rulestore = Distributor.RuleStores.First();

            //when
            rulestore.AddRule(new Rule(@"When(""knopje"").IsPushed().Switch(""lampje"")", "knopje", "lampje"));
            
            //then
            Assert.IsTrue(Distributor.RuleStores.First().Rules.Count() == 1);
            Assert.IsTrue(Distributor.RuleStores.First().Rules.First().Connections.Count() == 2);
        }

        [Test]
        public void DistributorInistializationDelegateTest()
        {
            //given
            var called = false;
            var externalSource = Distributor.ExternalSources.First();
            ((FakeExternalSource) externalSource).MyDistributorInitializationDelegate = d =>
            {
                Assert.IsNotNull(d);
                called = true;
            };

            //when
            new Distributor(new[] {externalSource}, Distributor.RuleStores);

            //then
            Assert.IsTrue(called);
        }
    }
}