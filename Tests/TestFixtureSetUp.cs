using NUnit.Framework;

namespace GiskardSolutions.GendarmeNUnit.Tests
{
    [TestFixture]
    public abstract class TestFixtureSetUp
    {
        [TestFixtureSetUp]
        public virtual void Setup() { }
    }
}
