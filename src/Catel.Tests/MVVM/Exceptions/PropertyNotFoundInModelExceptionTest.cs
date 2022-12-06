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
                Assert.AreEqual("viewModelProperty", ex.ViewModelPropertyName);
                Assert.AreEqual("modelName", ex.ModelName);
                Assert.AreEqual("modelPropertyName", ex.ModelPropertyName);
            }
        }
        #endregion
    }
}