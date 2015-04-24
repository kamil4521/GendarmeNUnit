using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GiskardSolutions.GendarmeNUnit.Tests.RuleConfigGenerator
{
    public class CustomConfig : TestFixtureSetUp
    {
        [Test]
        public void ShouldGenerateCustomConfigWithOneRule()
        {
            var configGenerator = new GendarmeNUnit.RuleConfigGenerator.CustomConfig();
            configGenerator.AddRuleLibrary("Gendarme.Rules.Naming", "..\\..\\..\\GendarmeNUnit\\GendarmeBin\\Gendarme.Rules.Naming.dll");
            configGenerator.AddRule("Gendarme.Rules.Naming", "AvoidDeepNamespaceHierarchyRule");

            var resultFile = Path.GetTempFileName();
            configGenerator.Save(resultFile);

            XDocument config = null;
            Assert.DoesNotThrow(() => config = XDocument.Load(resultFile));
            var rule = config.Element("gendarme").Elements().SingleOrDefault(e => e.Attribute("name").Value == "custom");
            Assert.That(rule, Is.Not.Null);
            Assert.That(rule.Element("rules").Attribute("from").Value, Is.StringEnding("Gendarme.Rules.Naming.dll"));
            Assert.That(rule.Element("rules").Attribute("include").Value, Is.EqualTo("AvoidDeepNamespaceHierarchyRule"));
        }

        [Test]
        public void ShouldGenerateCustomConfigWithEnabledAllRules()
        {
            var configGenerator = new GendarmeNUnit.RuleConfigGenerator.CustomConfig();
            configGenerator.AddRuleLibrary("Gendarme.Rules.Naming", "..\\..\\..\\GendarmeNUnit\\GendarmeBin\\Gendarme.Rules.Naming.dll");
            configGenerator.EnableAllRules("Gendarme.Rules.Naming");

            var resultFile = Path.GetTempFileName();
            configGenerator.Save(resultFile);

            XDocument config = null;
            Assert.DoesNotThrow(() => config = XDocument.Load(resultFile));
            var rule = config.Element("gendarme").Elements().SingleOrDefault(e => e.Attribute("name").Value == "custom");
            Assert.That(rule, Is.Not.Null);
            Assert.That(rule.Element("rules").Attribute("from").Value, Is.StringEnding("Gendarme.Rules.Naming.dll"));
            Assert.That(rule.Element("rules").Attribute("include").Value, Is.EqualTo("*"));
        }
    }

    public class DefaultConfig : TestFixtureSetUp
    {
        [Test]
        public void ShouldGenerateWithDefaultConfig()
        {
            var configGenerator = new GendarmeNUnit.RuleConfigGenerator.DefaultConfig();
            var resultFile = Path.GetTempFileName();
            configGenerator.Save(resultFile);

            Assert.DoesNotThrow(() => XDocument.Load(resultFile));
        }
    }
}
