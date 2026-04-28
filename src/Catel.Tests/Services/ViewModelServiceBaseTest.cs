namespace Catel.Tests.Services;

using Catel.Services;

using NUnit.Framework;

[TestFixture]
public class ViewModelServiceBaseTest
{
    private class ViewModelService : ViewModelServiceBase
    {
    }

    [TestCase]
    public void Name()
    {
        var testService = new ViewModelService();

        Assert.That(testService.Name, Is.EqualTo("ViewModelService"));
    }
}
