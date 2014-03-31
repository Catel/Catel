// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.ios.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



#if IOS

namespace Catel.MVVM.Views
{
    public static partial class ViewExtensions
    {
        static partial void FinalDispatch(IView view, Action action)
        {
            throw new MustBeImplementedException();
        }
    }
}

#endif