using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GiskardSolutions.GendarmeNUnit
{
    [TestFixture]
    public abstract class AbstractAssemblyTest : IDisposable
    {
        public struct DefectReport
        {
            public DefectReport(string rule, string problem, string solutions, string solutionUrl, string defectSeverity, string defectConfidence, string defectLocation, string defectSource)
            {
                Rule = rule;
                Problem = problem;
                Solutions = solutions;
                SolutionUrl = solutionUrl;
                DefectSeverity = defectSeverity;
                DefectConfidence = defectConfidence;
                DefectLocation = defectLocation;
                DefectSource = defectSource;
            }

            public override string ToString()
            {
                return string.Format("Rule: {0}\r\nDefect: {1}\r\nFile: {2}\r\nSolution: {3}\r\nSolution URL: {4}\r\nDefect severity: {5}\r\nDefect location: {6}\r\nDefect source: {7}",
                    Rule,
                    Problem,
                    DefectSource,
                    Solutions,
                    SolutionUrl,
                    DefectSeverity,
                    DefectLocation,
                    DefectSource
                    );
            }

            public string Rule;
            public string Problem;
            public string Solutions;
            public string SolutionUrl;
            public string DefectSeverity;
            public string DefectConfidence;
            public string DefectLocation;
            public string DefectSource;
        }

        [Test]
        [TestCaseSource("Configuration")]
        public virtual void CodeProblemDetected(object report)
        {
            Assert.Fail(report.ToString());
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (File.Exists(_configFilePath))
                    File.Delete(_configFilePath);
                if (Directory.Exists(_gendarmeService.GendarmeDir))
                    Directory.Delete(_gendarmeService.GendarmeDir, true);
            }
        }

        protected AbstractAssemblyTest()
        {
            _gendarmeService = new GendarmeService(BuildRuleConfig());
        }

        protected IEnumerable Configuration
        {
            get
            {
                _configFilePath = Path.GetTempFileName();
                _ruleConfig = BuildRuleConfig();
                _ruleConfig.Save(_configFilePath);

                _gendarmeService.ExtractGendarme();

                _gendarmeReports = new List<string>();
                foreach (var location in _gendarmeService.GetLocationsOfTestedAssemblies())
                {
                    _gendarmeReports.Add(_gendarmeService.ExecuteGendarmeFor(location));
                }
                foreach (var resultFile in _gendarmeReports)
                {
                    var defectCollection = _gendarmeService.GetDefectsFor(resultFile);
                    foreach (var defect in defectCollection)
                    {
                        yield return defect;
                    }
                }
            }
        }

        protected abstract RuleConfigGenerator BuildRuleConfig();

        private GendarmeService _gendarmeService;
        private string _configFilePath;
        private RuleConfigGenerator _ruleConfig;
        private string _tmpPath;
        private bool _isDisposed = false;
        private List<string> _gendarmeReports;
    }
}
