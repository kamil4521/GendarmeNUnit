# GendarmeNUnit
Gendarme boilerplate for NUnit

## Example analysis class

```cs
    public class ApplicationAssemblyAnalysis : AbstractAssemblyTest
    {
        protected override RuleConfigGenerator BuildRuleConfig()
        {
            var config = new RuleConfigGenerator.CustomConfig();
            config.AddRuleLibrary("Gendarme.Rules.BadPractice", "Gendarme.Rules.BadPractice.dll");
            config.AddRule("Gendarme.Rules.BadPractice", "DisableDebuggingCodeRule");

            GetAssembliesToAnalysis().ToList().ForEach(a => config.TestedAssemblies.Add(a));
            return config;
        }

        public class ApplicationAnalysis : AbstractAnalysis
        {
            protected override IEnumerable<string> GetAssembliesToAnalysis()
            {
                yield return "Giskard.Application";
            }
        }
    }
```
