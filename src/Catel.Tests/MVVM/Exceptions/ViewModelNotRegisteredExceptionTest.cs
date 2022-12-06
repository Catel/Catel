namespace Catel.Tests.MVVM.Exceptions
{
    using Catel.MVVM;

    using NUnit.Framework;

    [TestFixture]
    public class ViewModelNotRegisteredExceptionTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            try
            {
                throw new ViewModelNotRegisteredException(typeof(ViewModelBase));
            }
            catch (ViewModelNotRegisteredException ex)
            {
                Assert.AreEqual(typeof(ViewModelBase), ex.ViewModelType);
            }
        }
        #endregion
    }
}