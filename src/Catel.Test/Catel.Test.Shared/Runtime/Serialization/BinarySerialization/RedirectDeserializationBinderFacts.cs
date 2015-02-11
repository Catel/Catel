// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectDeserializationBinderFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Runtime.Serialization
{
    using Catel.Reflection;
    using Catel.Runtime.Serialization.Binary;
    using NUnit.Framework;

    public class RedirectDeserializationBinderFacts
    {
        [TestFixture]
        public class ThePerformanceTests
        {
            [TestCase]
            public void PerformanceTestWithoutMultithreadedInitialization()
            {
                PerformanceTest(-1);
            }

            [TestCase]
            public void PerformanceTestWithMultithreadedInitialization100TypesPerThread()
            {
                PerformanceTest(100);
            }

            [TestCase]
            public void PerformanceTestWithMultithreadedInitialization250TypesPerThread()
            {
                PerformanceTest(250);
            }

            [TestCase]
            public void PerformanceTestWithMultithreadedInitialization500TypesPerThread()
            {
                PerformanceTest(500);
            }

            [TestCase]
            public void PerformanceTestWithMultithreadedInitialization1000TypesPerThread()
            {
                PerformanceTest(1000);
            }

            [TestCase]
            public void PerformanceTestWithMultithreadedInitialization2500TypesPerThread()
            {
                PerformanceTest(2500);
            }

            [TestCase]
            public void PerformanceTestWithMultithreadedInitialization5000TypesPerThread()
            {
                PerformanceTest(5000);
            }

            [TestCase]
            public void PerformanceTestWithMultithreadedInitialization10000TypesPerThread()
            {
                PerformanceTest(10000);
            }

            private void PerformanceTest(int typesPerThread)
            {
                TimeMeasureHelper.MeasureAction(5, "Multithreaded initialization", 
                    () => new RedirectDeserializationBinder(typesPerThread), 
                    () => TypeCache.InitializeTypes());
            }
        }
    }
}

#endif