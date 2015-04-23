using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GiskardSolutions.GendarmeNUnit
{
    public abstract class RuleConfigGenerator
    {
        public class CustomConfig : RuleConfigGenerator
        {
            public CustomConfig() : base()
            {
                _ruleLibaryCollection = new List<string>();
            }

            public void AddRuleLibrary(string ruleLibraryPath)
            {
                if (!File.Exists(ruleLibraryPath))
                    throw new Exceptions.RuleLibraryFileNotExists(ruleLibraryPath);

                _ruleLibaryCollection.Add(ruleLibraryPath);
            }

            protected override void UpdateXml()
            {
                base.UpdateXml();
            }

            private readonly List<string> _ruleLibaryCollection;
        }

        public void Save(string fileName)
        {
            UpdateXml();
            RuleConfig.Save(fileName);
        }

        protected XElement RuleConfig { get; private set; }

        protected RuleConfigGenerator()
        {
            RuleConfig = XElement.Load("exampleRules.xml");
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

            public Exceptions() : base() { }
            public Exceptions(string message) : base(message) { }
        }
    }
}
