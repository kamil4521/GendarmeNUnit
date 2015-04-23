using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiskardSolutions.GendarmeNUnit
{
    [SetUpFixture]
    public abstract class GendarmeSetup
    {
        [SetUp]
        public void Setup()
        {
            Initialize();
            RunGendarme();
        }

        protected abstract void Initialize();

        private void RunGendarme()
        {
            throw new NotImplementedException();
        }
    }
}
