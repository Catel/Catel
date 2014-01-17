// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectDeserializationBinderFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Runtime.Serialization
{
    using Catel.Reflection;
    using Catel.Runtime.Serialization.Binary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class RedirectDeserializationBinderFacts
    {
        [TestClass]
        public class ThePerformanceTests
        {
            [TestMethod]
            public void PerformanceTestWithoutMultithreadedInitialization()
            {
                PerformanceTest(-1);
            }

            [TestMethod]
            public void PerformanceTestWithMultithreadedInitialization100TypesPerThread()
            {
                PerformanceTest(100);
            }

            [TestMethod]
            public void PerformanceTestWithMultithreadedInitialization250TypesPerThread()
            {
                PerformanceTest(250);
            }

            [TestMethod]
            public void PerformanceTestWithMultithreadedInitialization500TypesPerThread()
            {
                PerformanceTest(500);
            }

            [TestMethod]
            public void PerformanceTestWithMultithreadedInitialization1000TypesPerThread()
            {
                PerformanceTest(1000);
            }

            [TestMethod]
            public void PerformanceTestWithMultithreadedInitialization5000TypesPerThread()
            {
                PerformanceTest(5000);
            }

            private void PerformanceTest(int typesPerThread)
            {
                TimeMeasureHelper.MeasureAction(5, "Multithreaded initialization", 
                    () => new RedirectDeserializationBinder(typesPerThread), 
                    () => TypeCache.InitializeTypes(false));
            }
        }
    }
}