
using NUnit.Framework;

namespace GiskardSolutions.GendarmeNUnit.Tests
{
    public class GendarmeSetupTests : TestFixtureSetUp
    {
        public class GendarmeSetupForTests : GendarmeSetup
        {
            protected override GendarmeNUnit.RuleConfigGenerator BuildRuleConfig()
            {
                return new GendarmeNUnit.RuleConfigGenerator.DefaultConfig();
            }
        }

        public override void Setup()
        {
            base.Setup();
            _gendarmeSetup = new GendarmeSetupForTests();
        }

        [Test]
        public void ShouldRunSetup()
        {
            Assert.DoesNotThrow(() => _gendarmeSetup.Setup());
        }

        private GendarmeSetupForTests _gendarmeSetup;
    }
}
