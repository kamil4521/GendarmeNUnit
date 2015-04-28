using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

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
                _isDisposed = true;
                _gendarmeService.Dispose();
                _gendarmeService = null;
            }
        }

        protected AbstractAssemblyTest()
        {
            _gendarmeService = new GendarmeService(BuildRuleConfig());
            _gendarmeService.ExtractGendarme();
        }

        protected IEnumerable Configuration
        {
            get
            {
                var gendarmeReports = new List<string>();
                foreach (var location in _gendarmeService.GetLocationsOfTestedAssemblies())
                {
                    gendarmeReports.Add(_gendarmeService.ExecuteGendarmeFor(location));
                }
                foreach (var resultFile in gendarmeReports)
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
        private bool _isDisposed = false;
    }
}
