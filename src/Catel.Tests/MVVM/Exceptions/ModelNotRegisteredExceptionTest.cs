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
                Assert.That(ex.ModelName, Is.EqualTo("model"));
                Assert.That(ex.PropertyDeclaringViewModelToModelAttribute, Is.EqualTo("property"));
            }
        }
        #endregion
    }
}