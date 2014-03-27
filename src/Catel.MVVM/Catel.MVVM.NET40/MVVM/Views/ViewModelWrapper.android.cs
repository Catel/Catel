// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapper.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.MVVM.Views
{
    public partial class ViewModelWrapper
    {
        partial void CreateWrapper(object viewModelWrapper)
        {
            throw new MustBeImplementedException();
        }

        partial void SetViewModel(IViewModel viewModel)
        {
            throw new MustBeImplementedException();
        }
    }
}

#endif