
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
    public class GendarmeService : IDisposable
    {
        public GendarmeService(RuleConfigGenerator ruleConfigGenerator)
        {
            _tmpPath = Path.GetTempPath();
            _ruleConfig = ruleConfigGenerator;
            _ruleConfig.Save(_configFilePath = Path.GetTempFileName());
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
            XElement gendarmeResult = XElement.Load(resultFilePath);
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
                string.Format("\"{0}\" --config \"{1}\" --xml \"{2}\" --set \"{3}\" ", assemblyLocation, _configFilePath, reportXmlTmp,
                    _ruleConfig.RuleSetName)
            );
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            var process = Process.Start(startInfo);
            process.WaitForExit();
            if (process.ExitCode != 0 && process.ExitCode != 1)
            {
                string result = process.StandardOutput.ReadToEnd();
                throw new Exceptions.GendarmeExecutionFail(result);
            }
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
            foreach (var assemblyName in _ruleConfig.TestedAssemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    yield return assembly.Location;
                }
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                if (File.Exists(_configFilePath))
                    File.Delete(_configFilePath);
                if (Directory.Exists(GendarmeDir))
                    Directory.Delete(GendarmeDir, true);
            }
        }

        private readonly RuleConfigGenerator _ruleConfig;
        private readonly string _tmpPath;
        private readonly string _configFilePath;
        private bool _isDisposed = false;

        public abstract class Exceptions : Exception
        {
            public class GendarmeExecutionFail : Exceptions
            {
                public GendarmeExecutionFail(string message) : base(message) { }
            }

            protected Exceptions() : base() { }
            protected Exceptions(string message) : base(message) { }
        }
    }
}
