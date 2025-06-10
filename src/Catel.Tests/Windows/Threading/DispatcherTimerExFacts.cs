namespace Catel.Tests.Windows.Threading
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using Catel.Windows.Threading;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class DispatcherTimerExFacts
    {
        [Test]
        public async Task Runs_Dispatcher_Methods_With_Mock()
        {
            var dispatcherServiceMock = new Mock<IDispatcherService>();

            var dispatcherTimerEx = new DispatcherTimerEx(dispatcherServiceMock.Object)
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };

            dispatcherTimerEx.Tick += (sender, args) =>
            {
                // Dummy
            };

            using (new DisposableToken<DispatcherTimerEx>(dispatcherTimerEx,
                (x) => x.Instance.Start(),
                (x) => x.Instance.Stop()))
            {
                await Task.Delay(100);

                dispatcherServiceMock.Verify(x => x.BeginInvoke(It.IsAny<Action>(), It.Is<bool>(y => y == true)), Times.AtLeastOnce);
            }
        }

        [Test]
        public async Task Runs_Dispatcher_Methods_Without_BeginInvokeIfRequired_With_Mock()
        {
            var dispatcherServiceMock = new Mock<IDispatcherService>();

            var dispatcherTimerEx = new DispatcherTimerEx(dispatcherServiceMock.Object)
            {
                Interval = TimeSpan.FromMilliseconds(50),
                OnlyBeginInvokeIfRequired = false
            };

            dispatcherTimerEx.Tick += (sender, args) =>
            {
                // Dummy
            };

            using (new DisposableToken<DispatcherTimerEx>(dispatcherTimerEx,
                (x) => x.Instance.Start(),
                (x) => x.Instance.Stop()))
            {
                await Task.Delay(100);

                dispatcherServiceMock.Verify(x => x.BeginInvoke(It.IsAny<Action>(), It.Is<bool>(y => y == false)), Times.AtLeastOnce);
            }
        }

        [Test]
        public async Task Runs_Dispatcher_Methods_With_Implementation()
        {
            var dispatcherTimerEx = new DispatcherTimerEx(new DispatcherService(new DispatcherProviderService()))
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };

            var invoked = false;

            dispatcherTimerEx.Tick += (sender, args) =>
            {
                invoked = true;
            };

            using (new DisposableToken<DispatcherTimerEx>(dispatcherTimerEx,
                (x) => x.Instance.Start(),
                (x) => x.Instance.Stop()))
            {
                await Task.Delay(100);
            }

            Assert.That(invoked, Is.True);
        }

        [TestCase(100, 1)]
        [TestCase(300, 2)]
        [TestCase(800, 4)]
        public async Task Does_Prevent_Duplicate_Ticks(int durationInMilliseconds, int expectedInvocationCount)
        {
            var dispatcherTimerEx = new DispatcherTimerEx(new DispatcherService(new DispatcherProviderService()))
            {
                Interval = TimeSpan.FromMilliseconds(50),
                PreventDuplicateTicks = true
            };

            int invocationCount = 0;

            dispatcherTimerEx.Tick += (sender, args) =>
            {
                invocationCount++;

                Task.Delay(150).Wait();
            };

            using (new DisposableToken<DispatcherTimerEx>(dispatcherTimerEx,
                (x) => x.Instance.Start(),
                (x) => x.Instance.Stop()))
            {
                await Task.Delay(durationInMilliseconds);
            }

            Assert.That(invocationCount, Is.EqualTo(expectedInvocationCount));
        }

        [Test]
        public async Task Does_Not_Prevent_Duplicate_Ticks()
        {
            var dispatcherServiceMock = new Mock<IDispatcherService>();

            var dispatcherTimerEx = new DispatcherTimerEx(dispatcherServiceMock.Object)
            {
                Interval = TimeSpan.FromMilliseconds(50),
                PreventDuplicateTicks = false
            };

            var invocationCount = 0;

            dispatcherTimerEx.Tick += (sender, args) =>
            {
                invocationCount++;

                Task.Delay(150).Wait();
            };

            using (new DisposableToken<DispatcherTimerEx>(dispatcherTimerEx,
                (x) => x.Instance.Start(),
                (x) => x.Instance.Stop()))
            {
                await Task.Delay(100);

                dispatcherServiceMock.Verify(x => x.BeginInvoke(It.IsAny<Action>(), It.Is<bool>(y => y == true)), Times.Exactly(2));
            }
        }

        [Test]
        public async Task Keeps_Invoking_When_Exception_Occurs()
        {
            var dispatcherTimerEx = new DispatcherTimerEx(new DispatcherService(new DispatcherProviderService()))
            {
                Interval = TimeSpan.FromMilliseconds(50),
                PreventDuplicateTicks = true
            };

            var invocationCount = 0;

            dispatcherTimerEx.Tick += (sender, args) =>
            {
                invocationCount++;

                throw new Exception();
            };

            using (new DisposableToken<DispatcherTimerEx>(dispatcherTimerEx,
                (x) => x.Instance.Start(),
                (x) => x.Instance.Stop()))
            {
                await Task.Delay(200);
            }

            Assert.That(invocationCount, Is.GreaterThan(1));
        }
    }
}
