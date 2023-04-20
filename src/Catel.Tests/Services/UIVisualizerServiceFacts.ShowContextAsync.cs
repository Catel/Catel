namespace Catel.Tests.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Tests.ViewModels;
    using Catel.Tests.Views;
    using Moq;
    using NUnit.Framework;

    public partial class UIVisualizerServiceFacts
    {
        [TestFixture, Apartment(ApartmentState.STA)]
        public class The_ShowContextAsync_Method
        {
            [TestCase, MaxTime(30 * 1000)]
            public async Task Does_Not_Subscribe_More_Than_Once_To_Close_Handler_Async()
            {
                var viewLocatorMock = new Mock<IViewLocator>();
                viewLocatorMock.Setup(x => x.ResolveView(It.IsAny<Type>()))
                    .Returns<Type>(x =>
                    {
                        return typeof(AutoClosingView);
                    });

                var dispatcherServiceMock = new Mock<IDispatcherService>();
                dispatcherServiceMock.Setup(x => x.BeginInvoke(It.IsAny<Action>(), It.IsAny<bool>()))
                    .Callback<Action, bool>((action, whenRequired) =>
                    {
                        action();
                    });

                var uiVisualizerService = new UIVisualizerService(viewLocatorMock.Object, dispatcherServiceMock.Object);

                uiVisualizerService.Register(typeof(AutoClosingViewModel), typeof(AutoClosingView));

                var callbackExecutionCount = 0;

                var uiVisualizerContext = new UIVisualizerContext
                {
                    Data = new AutoClosingViewModel(),
                    CompletedCallback = (sender, e) =>
                    {
                        callbackExecutionCount++;
                    }
                };

                await uiVisualizerService.ShowContextAsync(uiVisualizerContext);

                Assert.That(callbackExecutionCount, Is.EqualTo(1));
            }
        }
    }
}
