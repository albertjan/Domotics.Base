using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Domotics.Base.DSL;
using Domotics.Base.Test.Fakes;
using NUnit.Framework;

namespace Domotics.Base.Test
{
    [TestFixture]
    public class RuleLogicFixture
    {
        [SetUp]
        public void Init ()
        {
            Distributor = new Distributor(new IExternalSource[] { new FakeExternalSource(), new AnotherFakeExternalSource() }, new[] {new FakeRuleStore()});
        }

        private Distributor Distributor { get; set; }

        [Test]
        public void SimpleRuleCheckTest ()
        {
            //given
            var rule = new Rule(@"When(""knopje"").IsPushed().Switch(""lampje"")", new[] {"knopje", "lampje"});

            //when
            Distributor.RuleStores.First().AddRule (rule);

            //then
            Assert.IsTrue(rule.Check());
        }

        [Test]
        public void SimpleRuleFireTest ()
        {
            //given
            var rule = new Rule (@"When(""knopje"").IsPushed().Switch(""lampje"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);
            Distributor.ExternalSources.First().Connections.First(c => c.Name == "knopje").CurrentState = "in";

            rule.LastTriggered = DateTime.Now.Ticks;

            //when
            var chd = rule.Fire(new Connection("knopje", ConnectionType.In) { CurrentState = "in" }, "out").ToList();
            
            //then
            Assert.AreEqual("lampje",chd.First().Connection.Name);
            Assert.AreEqual("on", chd.First().NewState.Name);
        }

        [Test]
        public void InputEventRuleFireTest ()
        {
            //given
            var rule = new Rule (@"When(""knopje"").IsPushed().Switch(""lampje"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);
            
            //when
            ((FakeExternalSource) Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            ((FakeExternalSource) Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "out");
            
            //then
            Assert.AreEqual("on", Distributor.ExternalSources.First().Connections.First(c => c.Name == "lampje").CurrentState.Name);
        }

        [Test]
        public void InputEventFromCopiedConnectionRuleFireTest()
        {
            //given
            var rule = new Rule(@"When(""knopje"").IsPushed().Switch(""lampje"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First().AddRule(rule);

            //when
            ((AnotherFakeExternalSource)Distributor.ExternalSources.First(t => t is AnotherFakeExternalSource)).FireInputEvent("knopje", "in");
            ((AnotherFakeExternalSource)Distributor.ExternalSources.First(t => t is AnotherFakeExternalSource)).FireInputEvent("knopje", "out");

            //then
            Assert.AreEqual("on", Distributor.ExternalSources.First().Connections.First(c => c.Name == "lampje").CurrentState.Name);
        }

        [Test]
        public void RuleChangesStateTest ()
        {
            //given
            var rule = new Rule (@"When(""knopje"").ChangesStateWithin(""out"",""in"", 500).Switch(""lampje"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);
            //set the last triggered so we can see it's less than 500ms ago.
            rule.LastTriggered = DateTime.Now.Ticks;
            //when
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            
            //then
            Assert.AreEqual ("on", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje").CurrentState.Name);
        }

        [Test]
        public void RuleInputHeld ()
        {
            //given
            var rule = new Rule (@"When(""knopje"").IsHeld().Switch(""lampje"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule);

            //when
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            //IsHeld means the second event has to happen after 500ms.
            Thread.Sleep (500);
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "out");
         
            //then
            Assert.AreEqual ("on", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje").CurrentState.Name);
        }

        [Test]
        public void RuleInputHeldNotLongEnough ()
        {
            //given
            var rule1 = new Rule (@"When(""knopje"").IsHeld().Switch(""lampje"")", new[] { "knopje", "lampje" });
            Distributor.RuleStores.First ().AddRule (rule1);

            //when
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            //IsHeld means the second event has to happen after 500ms.
            Thread.Sleep (450);
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "out");

            //then
            Assert.AreEqual ("off", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje").CurrentState.Name);
        }

        [Test]
        public void RuleInputHeldWithTwoRules ()
        {
            //given
            var rule1 = new Rule (@"When(""knopje"").IsHeld().Switch(""lampje"")", new[] { "knopje", "lampje" });
            var rule2 = new Rule (@"When(""knopje"").IsPushed().Switch(""lampje2"")", new[] { "knopje", "lampje2" });
            Distributor.RuleStores.First ().AddRule (rule1);
            Distributor.RuleStores.First ().AddRule (rule2);
            
            //when
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            //IsHeld means the second event has to happen after 500ms.
            Thread.Sleep (500);
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "out");

            //then
            Assert.AreEqual ("on", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje").CurrentState.Name);
            Assert.AreEqual ("off", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje2").CurrentState.Name);
        }

        [Test]
        public void RuleInputNotHeldWithTwoRules ()
        {
            //given
            var rule1 = new Rule (@"When(""knopje"").IsHeld().Switch(""lampje"")", new[] { "knopje", "lampje" });
            var rule2 = new Rule (@"When(""knopje"").IsPushed().Switch(""lampje2"")", new[] { "knopje", "lampje2" });
            Distributor.RuleStores.First ().AddRule (rule1);
            Distributor.RuleStores.First ().AddRule (rule2);

            //when
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in");
            //IsHeld means the second event has to happen after 500ms.
            //Thread.Sleep (500);
            ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "out");

            //then
            Assert.AreEqual ("off", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje").CurrentState.Name);
            Assert.AreEqual ("on", Distributor.ExternalSources.First ().Connections.First (c => c.Name == "lampje2").CurrentState.Name);
        }

        [Test]
        public void HotelSwitch()
        {
            //given
            var rule1 = new Rule(@"When(""knopje"").OrWhen(""knopje2"").IsPushed().Switch(""lampje"")",
                                 new[] {"knopje", "knopje2", "lampje"});
            Distributor.RuleStores.First().AddRule(rule1);

            //when
            ((FakeExternalSource) Distributor.ExternalSources.First()).FireInputEvent("knopje", "in");
            ((FakeExternalSource) Distributor.ExternalSources.First()).FireInputEvent("knopje", "out");

            //then
            Assert.That(
                "on" == Distributor.ExternalSources.First().Connections.First(c => c.Name == "lampje").CurrentState.Name,
                Is.True.After(100));


            //when
            ((FakeExternalSource) Distributor.ExternalSources.First()).FireInputEvent("knopje2", "in");
            ((FakeExternalSource) Distributor.ExternalSources.First()).FireInputEvent("knopje2", "out");

            //then
            Assert.AreEqual("off", Distributor.ExternalSources.First().Connections.First(c => c.Name == "lampje").CurrentState.Name);
        }

        [Test]
        public void RuleThatReactsOnAnInputThatDoesntExist ()
        {
            //given
            var rule1 = new Rule(@"When(""knobje"").IsPushed().Switch(""lampje"")", "knopje", "lampje");
            Distributor.RuleStores.First().AddRule(rule1);

            //when & then
            Assert.Throws<LogicException> (() => ((FakeExternalSource)Distributor.ExternalSources.First ()).FireInputEvent ("knopje", "in"));
        }

        [Test]
        public void RuleThatEmitsAStreamOfStateChangeDirectives()
        {
            //given
            var rule1 = new Rule(@"When(""knopje3"").IsLiveFireEvery(100).Increase(""lampje3"", 10, 100, 0)", "knopje3", "lampje3");
            Distributor.RuleStores.First().AddRule(rule1);

            //when
            var sw = new Stopwatch();
            sw.Start();
            var test = Observable.Generate(0, i => Distributor.ExternalSources.First().Connections.First(c => c.Name == "lampje3").CurrentState.Name != "100" , i => i + 1, i => i, i => TimeSpan.FromMilliseconds(10), TaskPoolScheduler.Default);
            test.Subscribe(i => ((FakeExternalSource)Distributor.ExternalSources.First()).FireInputEvent("knopje3", "in"));
            test.Wait();
            sw.Stop();
            Debug.WriteLine("Elapsed: " + sw.ElapsedMilliseconds);
            ((FakeExternalSource)Distributor.ExternalSources.First()).FireInputEvent("knopje3", "out");

            //then
            Assert.AreEqual("100",  Distributor.ExternalSources.First().Connections.First(c => c.Name == "lampje3").CurrentState.Name);
         }
    }
}