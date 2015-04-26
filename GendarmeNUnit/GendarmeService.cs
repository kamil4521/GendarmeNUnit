
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GiskardSolutions.GendarmeNUnit
{
    public class GendarmeService
    {
        public GendarmeService(RuleConfigGenerator ruleConfigGenerator)
        {
            _tmpPath = Path.GetTempPath();
            _ruleConfig = ruleConfigGenerator;
        }

        public string TemporaryPath
        {
            get { return _tmpPath; }
        }

        public string GendarmeDir
        {
            get { return Path.Combine(_tmpPath, "GendarmeBin"); }
        }

        public IEnumerable<object> GetDefectsFor(string resultFilePath)
        {
            var results = new List<object>();
            var gendarmeResult = XElement.Load(resultFilePath);
            var readedRules = gendarmeResult.XPathSelectElements("/results/rule");
            foreach (var item in readedRules)
            {
                var rule = item.Attribute("Name").Value;
                var problem = item.Element("problem").Value;
                var solutions = item.Element("solution").Value;
                var solutionUrl = item.Attribute("Uri").Value;

                var defectSeverity = item.Element("target").Element("defect").Attribute("Severity").Value;
                var defectConfidence = item.Element("target").Element("defect").Attribute("Confidence").Value;
                var defectLocation = item.Element("target").Element("defect").Attribute("Location").Value;
                var defectSource = item.Element("target").Element("defect").Attribute("Source").Value;


                results.Add(new AbstractAssemblyTest.DefectReport(
                        rule,
                        problem,
                        solutions,
                        solutionUrl,
                        defectSeverity,
                        defectConfidence,
                        defectLocation,
                        defectSource
                    )
                );
            }
            return results;
        }

        public string ExecuteGendarmeFor(string assemblyLocation)
        {
            string reportXmlTmp = Path.GetTempFileName();
            string gendarmeExe = Path.Combine(GendarmeDir, "gendarme.exe");
            ProcessStartInfo startInfo = new ProcessStartInfo(
                gendarmeExe,
                string.Format("\"{0}\" --xml \"{1}\" --set \"{2}\" ", assemblyLocation, reportXmlTmp,
                    _ruleConfig.RuleSetName)
            );
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            Process.Start(startInfo).WaitForExit();
            return reportXmlTmp;
        }

        public string ExtractGendarme()
        {
            var currentAssembly = Assembly.GetAssembly(typeof(AbstractAssemblyTest));
            var gendarmeArchiveName = currentAssembly.GetManifestResourceNames().Single(r => r.EndsWith("GendarmeBin.zip"));
            Stream gendarmeArchive = currentAssembly.GetManifestResourceStream(gendarmeArchiveName);

            Directory.CreateDirectory(_tmpPath);

            new Unzip(gendarmeArchive, _tmpPath).Extract();
            return _tmpPath;
        }

        public IEnumerable<string> GetLocationsOfTestedAssemblies()
        {
            return AppDomain.
                CurrentDomain.
                GetAssemblies()
                .Where(a => _ruleConfig.TestedAssemblies.Any(x => x == a.GetName().Name))
                .Select(a => a.Location);
        }

        private readonly RuleConfigGenerator _ruleConfig;
        private string _tmpPath;
        private bool _isDisposed = false;
    }
}
