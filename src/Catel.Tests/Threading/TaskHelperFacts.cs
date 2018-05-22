// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Threading
{
    using System;
    using Catel.Threading;
    using NUnit.Framework;
    using System.Threading.Tasks;

    public class TaskHelperFacts
    {
        [TestFixture]
        public class TheRunAndWaitMethod
        {
            [TestCase]
            public void CorrectlyWaitsForAllTasksToBeCompleted()
            {
                var a = false;
                var b = false;
                var c = false;

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

            [TestCase]
            public async Task CorrectlyWaitsForAllTasksToBeCompletedAsync()
            {
                var a = false;
                var b = false;
                var c = false;

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

                await TaskHelper.RunAndWaitAsync(new[] { taskA, taskB, taskC });

                Assert.IsTrue(a);
                Assert.IsTrue(b);
                Assert.IsTrue(c);
            }
        }
    }
}