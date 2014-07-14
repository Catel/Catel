// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Threading
{
    using System;
    using Catel.Threading;
    using NUnit.Framework;

    public class TaskHelperFacts
    {
        [TestFixture]
        public class TheRunAndWaitMethod
        {
            [TestCase]
            public void CorrectlyWaitsForAllTasksToBeCompleted()
            {
                bool a = false;
                bool b = false;
                bool c = false;

                Action taskA = () =>
                {
                    a = true;
                };
                Action taskB = () =>
                {
                    ThreadHelper.Sleep(100);
                    b = true; 
                };
                Action taskC = () =>
                {
                    ThreadHelper.Sleep(200);
                    c = true;
                };

                TaskHelper.RunAndWait(new [] { taskA, taskB, taskC });

                Assert.IsTrue(a);
                Assert.IsTrue(b);
                Assert.IsTrue(c);
            }
        }
    }
}