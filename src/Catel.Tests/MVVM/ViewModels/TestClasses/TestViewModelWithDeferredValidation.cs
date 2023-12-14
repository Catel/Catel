namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    public class TestViewModelWithDeferredValidation : TestViewModelWithValidationTags
    {
        #region Constructors
        public TestViewModelWithDeferredValidation()
        {
            DeferValidationUntilFirstSaveCall = true;
        }
        #endregion
    }
}