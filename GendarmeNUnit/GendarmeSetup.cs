using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace GiskardSolutions.GendarmeNUnit
{
    [SetUpFixture]
    public abstract class GendarmeSetup
    {
        [SetUp]
        public void Setup()
        {
            _configFilePath = Path.GetTempFileName();
            _ruleConfig = BuildRuleConfig();
            _ruleConfig.Save(_configFilePath);

            RunGendarme();
        }

        [TearDown]
        public void Dispose()
        {
            if (File.Exists(_configFilePath))
                File.Delete(_configFilePath);
        }

        protected abstract RuleConfigGenerator BuildRuleConfig();

        private void RunGendarme()
        {
            var location = Assembly.GetAssembly(typeof(GendarmeSetup)).Location;

            //new ProcessStartInfo()
            //Process.Start()
        }

        private string _configFilePath;
        private RuleConfigGenerator _ruleConfig;
    }
}
