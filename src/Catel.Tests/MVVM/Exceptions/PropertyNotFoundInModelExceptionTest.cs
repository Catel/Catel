namespace Catel.Tests.MVVM.Exceptions
{
    using Catel.MVVM;

    using NUnit.Framework;

    [TestFixture]
    public class PropertyNotFoundInModelExceptionTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            try
            {
                throw new PropertyNotFoundInModelException("viewModelProperty", "modelName", "modelPropertyName");
            }
            catch (PropertyNotFoundInModelException ex)
            {
                Assert.That(ex.ViewModelPropertyName, Is.EqualTo("viewModelProperty"));
                Assert.That(ex.ModelName, Is.EqualTo("modelName"));
                Assert.That(ex.ModelPropertyName, Is.EqualTo("modelPropertyName"));
            }
        }
        #endregion
    }
}