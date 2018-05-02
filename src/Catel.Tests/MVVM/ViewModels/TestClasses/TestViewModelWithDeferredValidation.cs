// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestViewModelWithDeferredValidation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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