namespace Catel.Tests.BugReports.GH2257
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Services;
    using NUnit.Framework;

    // See https://github.com/Catel/Catel/issues/2257

    [TestFixture]
    public class Catel_Environment_Does_Not_Break_Test
    {
        [Test, MaxTime(5000)]
        public async Task Does_Not_Break_Test_Async()
        {
            // +----------------
            // |
            // v

            // uncomment these and see below to break the test
            var breakIt = false;
            if (breakIt == true)
            {
                Trace.WriteLine("breaking it");

                if (CatelEnvironment.IsInDesignMode)
                {
                    Trace.WriteLine("CatelEnvironment.IsInDesignMode");
                }
            }

            var worker = new WorkingClass(ServiceLocator.Default.ResolveRequiredType<IDispatcherService>());
            await worker.InvokeAsync().ConfigureAwait(true);
            Assert.That(worker.CounterProperty, Is.EqualTo(1));
        }
    }

    public class WorkingClass
    {
        private readonly IDispatcherService _dispatcherService;

        public WorkingClass(IDispatcherService dispatcherService)
        {
            ArgumentNullException.ThrowIfNull(dispatcherService);

            _dispatcherService = dispatcherService;
        }

        public int CounterProperty { get; private set; }

        public async Task InvokeAsync()
        {
            Trace.WriteLine($"Before awaiting InvokeAsync ...");

            await _dispatcherService.InvokeAsync(() =>
            {
                CounterProperty++;
                Trace.WriteLine($"{nameof(InvokeAsync)} increased to {CounterProperty}");

                // +----------------
                // |
                // v
                // after breaking, the test, uncomment the next line to repair
                //return Task.CompletedTask;

            }).ConfigureAwait(false);

            Trace.WriteLine($"after awaiting InvokeAsync ...");
        }
    }
}
