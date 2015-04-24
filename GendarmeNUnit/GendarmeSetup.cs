using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GiskardSolutions.GendarmeNUnit
{
    [SetUpFixture]
    public abstract class GendarmeSetup
    {
        [SetUp]
        public void Setup()
        {
            _tmpPath = Path.GetTempPath();
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
            ExtractGendarme();

        }

        private void ExtractGendarme()
        {
            var currentAssembly = Assembly.GetAssembly(typeof(GendarmeSetup));
            var gendarmeArchiveName = currentAssembly.GetManifestResourceNames().Single(r => r.EndsWith("GendarmeBin.zip"));
            Stream gendarmeArchive = currentAssembly.GetManifestResourceStream(gendarmeArchiveName);

            Directory.CreateDirectory(_tmpPath);

            new Unzip(gendarmeArchive, _tmpPath).Extract();
        }

        private string _configFilePath;
        private RuleConfigGenerator _ruleConfig;
        private string _tmpPath;
    }
}
