// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using Catel.Services;

#if !XAMARIN && !XAMARIN_FORMS
    using Windows.Threading;

#if UWP
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#else
    using System.Windows.Threading;
#endif

#endif

    /// <summary>
    /// Base class for all view model services.
    /// </summary>
    public abstract class ViewModelServiceBase : ServiceBase, IViewModelService
    {
#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        /// <value>The current dispatcher.</value>
        protected virtual Dispatcher Dispatcher
        {
            get { return DispatcherHelper.CurrentDispatcher; }
        }
#endif
    }
}
