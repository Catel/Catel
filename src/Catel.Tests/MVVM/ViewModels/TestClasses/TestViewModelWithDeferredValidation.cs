namespace Catel.Tests.MVVM.ViewModels.TestClasses;

using System;

public class TestViewModelWithDeferredValidation : TestViewModelWithValidationTags
{
    public TestViewModelWithDeferredValidation(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        DeferValidationUntilFirstSaveCall = true;
    }
}
