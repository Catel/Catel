namespace Catel.Tests.Threading
{
    using Catel.Threading;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class TimerFacts
    {
        [TestFixture]
        public class TheChangeMethod
        {
            [TestCase(200, -1, 1000, 1)]
            [TestCase(500, 200, 800, 2)]
            [TestCase(500, 200, 2700, 10)]
            [TestCase(-1, 100, 2500, 0)]
            public async Task ChangesCorrectlyAsync(int dueTime, int interval, int msToWait, int expectedCount)
            {
                var timer = new Timer();

                var tickCount = 0;

                timer.Elapsed += (sender, e) => { tickCount++; };

                timer.Change(dueTime, interval);

                await Task.Delay(msToWait);

                timer.Change(Timeout.Infinite, Timeout.Infinite);

                Assert.AreEqual(expectedCount, tickCount);
            }
        }

        [TestFixture]
        public class TheStressTest
        {
            [Test]
            public async Task StressTestAsync()
            {
                var timer = new Timer();

                var changeCount = 0;
                var tickCount = 0;

                timer.Changed += (sender, e) => { changeCount++; };
                timer.Elapsed += (sender, e) => { tickCount++; };

                var timeouts = new List<int>();
                var random = new Random();

                for (int i = 0; i < 100000; i++)
                {
                    var timeout = random.Next(50, 250);
                    timeouts.Add(timeout);
                }

                ParallelHelper.ExecuteInParallel(timeouts, x => timer.Change(x, x), 10);

                Assert.AreNotEqual(0, changeCount);
            }
        }
    }
}
