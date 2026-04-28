namespace Catel.Tests.Services;

using System;
using System.Threading;
using Catel.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

[TestFixture, Apartment(ApartmentState.STA), Explicit]
public class IBusyIndicatorServiceExtensionsTests
{
    private IBusyIndicatorService _target;
    private IBusyIndicatorService Target
    {
        get 
        {
            var target = _target;
            if (target is null)
            {
                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock.Setup(x => x.GetString(It.IsAny<string>()))
                    .Returns<string>(x => x);

                target = _target = new BusyIndicatorService(NullLogger<BusyIndicatorService>.Instance,
                    languageServiceMock.Object, new DispatcherService(NullLogger<DispatcherService>.Instance,
                    new DispatcherProviderService(NullLogger<DispatcherProviderService>.Instance)));
            }

            return target;
        }
        set { _target = value; }
    }

    /// <summary>
    /// Use Test_Cleanup to run code after each test has run.
    /// </summary>
    [TearDown]
    public void Test_Cleanup()
    {
        Target = null;
    }

    [Test]
    public void PushInScope_CodeThrowsException_Hides()
    {
        // ARRANGE
        Assert.That(Target.ShowCounter, Is.EqualTo(0));

        // ACT
        try
        {
            using (Target.PushInScope())
            {
                Assert.That(Target.ShowCounter, Is.EqualTo(1));
                throw new ArgumentException();
            }
        }
        catch (ArgumentException)
        {
        }

        // ASSERT
        Assert.That(Target.ShowCounter, Is.EqualTo(0));
    }

    [Test]
    public void PushInScope_WithStatus_CodeThrowsException_Hides()
    {
        // ARRANGE
        Assert.That(Target.ShowCounter, Is.EqualTo(0));

        // ACT
        try
        {
            using (Target.PushInScope("Loading..."))
            {
                Assert.That(Target.ShowCounter, Is.EqualTo(1));
                throw new ArgumentException();
            }
        }
        catch (ArgumentException)
        {
        }

        // ASSERT
        Assert.That(Target.ShowCounter, Is.EqualTo(0));
    }
}
