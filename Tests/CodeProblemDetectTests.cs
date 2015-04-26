

using NUnit.Framework;
using System;

namespace GiskardSolutions.GendarmeNUnit.Tests
{
    public class CodeProblemDetectTests : AbstractAssemblyTest
    {
        protected override GendarmeNUnit.RuleConfigGenerator BuildRuleConfig()
        {
            var config = new GendarmeNUnit.RuleConfigGenerator.DefaultConfig();
            config.TestedAssemblies.Add("Tests");
            return config;
        }

        [Test]
        [TestCaseSource("Configuration")]
        public override void CodeProblemDetected(object report)
        {
            Assert.Catch<Exception>(() => base.CodeProblemDetected(report));
            Assert.Pass("Passed: Code problem detected!");
        }
    }
}
