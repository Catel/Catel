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
        public async Task Runs_Dispatcher_Methods_With_Mock_Async()
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
        public async Task Runs_Dispatcher_Methods_Without_BeginInvokeIfRequired_With_Mock_Async()
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
        public async Task Runs_Dispatcher_Methods_With_Implementation_Async()
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
    }
}
