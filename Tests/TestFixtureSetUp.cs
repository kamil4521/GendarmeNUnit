using NUnit.Framework;

namespace GiskardSolutions.GendarmeNUnit.Tests
{
    [TestFixture]
    public abstract class TestFixtureSetUp
    {
        [SetUp]
        public virtual void Setup() { }
    }
}
