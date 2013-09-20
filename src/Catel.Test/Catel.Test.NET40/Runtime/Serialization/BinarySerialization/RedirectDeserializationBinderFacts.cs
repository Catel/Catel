// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectDeserializationBinderFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Diagnostics;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
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
                TypeCache.InitializeTypes(false);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int i = 0; i < 5; i++)
                {
                    var binder = new RedirectDeserializationBinder(typesPerThread);
                }

                stopwatch.Stop();

                Console.WriteLine("Multithreading average ({0} types / thread): {1}ms", typesPerThread, (stopwatch.Elapsed.TotalMilliseconds / 5));
            }
        }
    }
}