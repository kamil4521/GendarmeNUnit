using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GiskardSolutions.GendarmeNUnit
{
    public abstract class RuleConfigGenerator
    {
        public class CustomConfig : RuleConfigGenerator
        {
            public override string RuleSetName
            {
                get { return "custom"; }
            }

            public CustomConfig()
                : base()
            {
                _ruleModuleCollection = new List<RuleModule>();
            }

            public void AddRuleLibrary(string name, string ruleLibraryPath)
            {
                if (!File.Exists(ruleLibraryPath))
                    throw new Exceptions.RuleLibraryFileNotExists(ruleLibraryPath);

                if (_ruleModuleCollection.Any(r => r.Name == name))
                    throw new Exceptions.RuleLibraryExistsInConfig(name);

                _ruleModuleCollection.Add(new RuleModule(name, Path.GetFullPath(ruleLibraryPath)));
            }

            public void EnableAllRules(string ruleLibraryName)
            {
                AddRule(ruleLibraryName, "*");
            }

            public void DisableAllRules(string ruleLibraryName)
            {
                RemoveRule(ruleLibraryName, "*");
            }

            public void AddRule(string ruleLibraryName, string concreteRule)
            {
                if (!_ruleModuleCollection.Any(r => r.Name == ruleLibraryName))
                    throw new Exceptions.RuleLibraryNotExistsInConfig(ruleLibraryName);

                _ruleModuleCollection.Single(r => r.Name == ruleLibraryName).Rules.Add(concreteRule);
            }

            public void RemoveRule(string ruleLibraryName, string concreteRule)
            {
                if (_ruleModuleCollection.Any(r => r.Name == ruleLibraryName))
                    _ruleModuleCollection.Single(r => r.Name == ruleLibraryName).Rules.Remove(concreteRule);
            }

            protected override void UpdateXml()
            {
                var customConfig = RuleConfig.XPathSelectElements(string.Format("/ruleset[@name='{0}']", RuleSetName)).Single();
                foreach (var ruleModule in _ruleModuleCollection)
                {
                    var rules = ruleModule.Rules.Contains("*") ? "*" : string.Join(" | ", ruleModule.Rules);
                    var ruleNode = new XElement("rules",
                        new XAttribute("include", rules),
                        new XAttribute("from", ruleModule.FilePath)
                    );
                    customConfig.Add(ruleNode);
                }
            }

            private class RuleModule
            {
                public string Name { get; private set; }
                public string FilePath { get; private set; }
                public HashSet<string> Rules { get; private set; }

                public RuleModule(string name, string filePath)
                {
                    Name = name;
                    FilePath = filePath;
                    Rules = new HashSet<string>();
                }
            }

            private readonly List<RuleModule> _ruleModuleCollection;
        }

        public class DefaultConfig : RuleConfigGenerator
        {
            public override string RuleSetName
            {
                get { return "default"; }
            }
        }

        public abstract string RuleSetName { get; }

        public List<string> TestedAssemblies { get; private set; }

        public void Save(string fileName)
        {
            UpdateXml();
            RuleConfig.Save(fileName);
        }

        protected XElement RuleConfig { get; private set; }

        protected RuleConfigGenerator()
        {
            TestedAssemblies = new List<string>();
            var currentAssembly = Assembly.GetExecutingAssembly();
            var exampleConfigName = currentAssembly.GetManifestResourceNames().Single(r => r.EndsWith("exampleRules.xml"));
            Stream exampleConfig = currentAssembly.GetManifestResourceStream(exampleConfigName);
            RuleConfig = XElement.Load(exampleConfig);
        }

        protected virtual void UpdateXml()
        {
        }

        public class Exceptions : Exception
        {
            public class RuleLibraryFileNotExists : Exceptions
            {
                public RuleLibraryFileNotExists(string file) : base(string.Format("Rule library {0} not exists", file)) { }
            }

            public class RuleLibraryExistsInConfig : Exceptions
            {
                public RuleLibraryExistsInConfig(string name) : base(string.Format("Rule library {0} exists in config", name)) { }
            }

            public class RuleLibraryNotExistsInConfig : Exceptions
            {
                public RuleLibraryNotExistsInConfig(string name) : base(string.Format("Rule library {0} not exists in config", name)) { }
            }

            public Exceptions() : base() { }
            public Exceptions(string message) : base(message) { }
        }
    }
}
