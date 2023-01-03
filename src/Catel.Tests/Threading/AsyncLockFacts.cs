namespace Catel.Tests.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class AsyncLockFacts
    {
        private class AsyncLockTestClass
        {
            public readonly AsyncLock _asyncLock = new AsyncLock();

            public bool ExecutedSuccessfully { get; private set; }

            public int ExecutionCount { get; private set; }

            public void MethodA()
            {
                using (_asyncLock.Lock())
                {
                    MethodB();
                }
            }

            public void MethodB()
            {
                using (_asyncLock.Lock())
                {
                    ExecutionCount++;
                    ExecutedSuccessfully = true;
                }
            }

            public async Task MethodACombinedAsync()
            {
                using (await _asyncLock.LockAsync())
                {
                    await Task.Delay(200);

                    MethodB();
                }
            }

            public async Task MethodAAsync()
            {
                using (await _asyncLock.LockAsync())
                {
                    await Task.Delay(200);

                    await MethodBAsync();
                }
            }

            public async Task MethodBAsync()
            {
                using (await _asyncLock.LockAsync())
                {
                    await Task.Delay(200);

                    // Lock so the counter does not screw up
                    lock (this)
                    {
                        ExecutionCount++;
                        ExecutedSuccessfully = true;
                    }
                }
            }
        }

        private class DeadlockTestClass
        {
            private readonly AsyncLock _asyncLock = new();

            private int _startedThreads;
            private bool _isTaken;

            private readonly object _lock = new();

            public async Task MethodAsync()
            {
                var threadIndex = 0;

                lock (_lock)
                {
                    threadIndex = ++_startedThreads;
                    Debug.WriteLine($"------started: {threadIndex}");
                }

                try
                {
                    using (await _asyncLock.LockAsync())
                    {
                        Assert.IsFalse(_isTaken);

                        _isTaken = true;

                        Thread.Sleep(300);

                        _isTaken = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ex: {ex}");

                    throw;
                }
                finally
                {
                    Debug.WriteLine($"finished: {threadIndex}");
                }
            }
        }

        [Test]
        public async Task Allow_Recursive_Locks_NonAsync()
        {
            var testClass = new AsyncLockTestClass();

#pragma warning disable CL0001 // Use async overload inside this async method
            var tasksList = new List<Task>(new[]
            {
                Task.Run(() => testClass.MethodA())
            });
#pragma warning restore CL0001 // Use async overload inside this async method

            Task.WaitAll(tasksList.ToArray(), 500);

            Assert.IsTrue(testClass.ExecutedSuccessfully);
        }

        [Test]
        public async Task Allow_Recursive_Locks_CombinedAsync()
        {
            var testClass = new AsyncLockTestClass();

#pragma warning disable CL0001 // Use async overload inside this async method
            var tasksList = new List<Task>(new[]
            {
                Task.Run(async () => await testClass.MethodACombinedAsync())
            });
#pragma warning restore CL0001 // Use async overload inside this async method

            Task.WaitAll(tasksList.ToArray(), 500);

            Assert.IsTrue(testClass.ExecutedSuccessfully);
        }

        [Test]
        public async Task Allow_Recursive_Locks_Async()
        {
            var testClass = new AsyncLockTestClass();

            var tasksList = new List<Task>(new[]
            {
                Task.Run(async () => await testClass.MethodAAsync())
            });

            Task.WaitAll(tasksList.ToArray(), 500);

            Assert.IsTrue(testClass.ExecutedSuccessfully);
        }

        [Test]
        public async Task Does_Not_Allow_Multitask_Locks_Simple_Async()
        {
            var testClass = new AsyncLockTestClass();

            var tasksList = new List<Task>(new[]
            {
                Task.Run(async () => await testClass.MethodAAsync()),
                Task.Run(async () => await testClass.MethodAAsync()),
                Task.Run(async () => await testClass.MethodAAsync())
            });

            Task.WaitAll(tasksList.ToArray(), 500);

            Assert.IsTrue(testClass.ExecutedSuccessfully);
            Assert.AreEqual(1, testClass.ExecutionCount);
        }

        [Test]
        public async Task Does_Not_Allow_Multitask_Locks_Complex_Async()
        {
            var testClass = new AsyncLockTestClass();

            var tasksList = new List<Task>();

            for (int i = 0; i < 50; i++)
            {
                tasksList.Add(Task.Run(async () => await testClass.MethodAAsync()));
            }

            Task.WaitAll(tasksList.ToArray(), 60 * 1000);

            Assert.IsTrue(testClass.ExecutedSuccessfully);
            Assert.AreEqual(50, testClass.ExecutionCount);
        }

        [Test]
        public async Task Does_Allow_Multitask_Locks_Queued_Async()
        {
            var testClass = new AsyncLockTestClass();

            var tasksList = new List<Task>(new[]
            {
                Task.Run(async () => await testClass.MethodAAsync()),
                Task.Run(async () => await testClass.MethodAAsync()),
                Task.Run(async () => await testClass.MethodAAsync())
            });

            Task.WaitAll(tasksList.ToArray(), 10 * 5000);

            Assert.IsTrue(testClass.ExecutedSuccessfully);
            Assert.AreEqual(3, testClass.ExecutionCount);
        }

        [Test]
        public async Task Releases_Is_Taken_Property_Async()
        {
            var testClass = new AsyncLockTestClass();

            var tasksList = new List<Task>(new[]
            {
                Task.Run(async () => await testClass.MethodAAsync())
            });

            Task.WaitAll(tasksList.ToArray(), 500);

            Assert.IsTrue(testClass.ExecutedSuccessfully);
            Assert.IsFalse(testClass._asyncLock.IsTaken);
        }

        [Test]
        public async Task Does_Not_Deadlock_Async()
        {
            Debug.Flush();

            var deadlockClass = new DeadlockTestClass();

            var tasks = new List<Task>();

            for (var i = 0; i < 10; i++)
            {
                var task = Task.Run(deadlockClass.MethodAsync);
                tasks.Add(task);
            }

            var timeout = 10 * 1000;

            if (Debugger.IsAttached)
            {
                timeout = 2 * 60 * 1000;
            }

            Task.WaitAll(tasks.ToArray(), timeout);

            Assert.IsTrue(tasks.All(x => x.IsCompletedSuccessfully));
        }
    }
}
