using GiskardSolutions.GendarmeNUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Class1 : TestFixtureSetUp
    {
        [Test]
        public void SelfTest()
        {
            var configGenerator = new RuleConfigGenerator.CustomConfig();
            configGenerator.AddRuleLibrary("Gendarme.Rules.Naming", "D:\\Documents\\GendarmeNUnit\\GendarmeNUnit\\GendarmeBin\\Gendarme.Rules.Naming.dll");
            configGenerator.AddRule("Gendarme.Rules.Naming", "AvoidDeepNamespaceHierarchyRule");
            configGenerator.EnableAllRules("Gendarme.Rules.Naming");
            //configGenerator.DisableAllRules("Gendarme.Rules.Naming");


            configGenerator.Save("D:\\xxx");
        }
    }
}
