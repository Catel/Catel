// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using Catel.Services;

#if !XAMARIN
    using Windows.Threading;

#if NETFX_CORE
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
#if !XAMARIN
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
