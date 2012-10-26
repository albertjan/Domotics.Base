using System.Linq;
using Castle.Components.DictionaryAdapter;
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
            Distributor = new Distributor(new[] { FakeExternalSource }, new[] {new FakeRuleStore()});
        }

        private FakeExternalSource FakeExternalSource { get; set; }

        private Distributor Distributor { get; set; }

        [Test]
        public void InputEventHandlerTest()
        {
            Assert.AreEqual ("out", FakeExternalSource.Connections.First (c => c.Name == "knopje").CurrentState.Name);
            FakeExternalSource.FireInputEvent("knopje", "in");
            Assert.That ("in" == FakeExternalSource.Connections.First (c => c.Name == "knopje").CurrentState.Name, Is.True.After (100));
        }
    }
}