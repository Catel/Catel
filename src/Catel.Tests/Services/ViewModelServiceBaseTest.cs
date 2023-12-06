namespace Catel.Tests.Services
{
    using Catel.Services;

    using NUnit.Framework;

    [TestFixture]
    public class ViewModelServiceBaseTest
    {
        #region Classes
        private class ViewModelService : ViewModelServiceBase
        {
        }
        #endregion

        #region Methods
        [TestCase]
        public void Name()
        {
            var testService = new ViewModelService();

            Assert.That(testService.Name, Is.EqualTo("ViewModelService"));
        }
        #endregion
    }
}