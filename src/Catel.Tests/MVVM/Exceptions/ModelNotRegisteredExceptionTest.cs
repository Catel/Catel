namespace Catel.Tests.MVVM.Exceptions
{
    using Catel.MVVM;

    using NUnit.Framework;

    [TestFixture]
    public class ModelNotRegisteredExceptionTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            try
            {
                throw new ModelNotRegisteredException("model", "property");
            }
            catch (ModelNotRegisteredException ex)
            {
                Assert.AreEqual("model", ex.ModelName);
                Assert.AreEqual("property", ex.PropertyDeclaringViewModelToModelAttribute);
            }
        }
        #endregion
    }
}