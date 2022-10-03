namespace Catel.Tests.Threading
{
    using System.Collections.Generic;
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

                    ExecutionCount++;
                    ExecutedSuccessfully = true;
                }
            }
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
    }
}
