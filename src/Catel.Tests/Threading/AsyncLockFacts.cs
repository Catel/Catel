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
            private readonly AsyncLock _asyncLock = new AsyncLock();

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
        public async Task Does_Not_Allow_Multitask_Locks_Async()
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
    }
}
