// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using Catel.Services;
    using Windows.Threading;

#if NETFX_CORE
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#else
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Base class for all view model services.
    /// </summary>
    public abstract class ViewModelServiceBase : ServiceBase, IViewModelService
    {
        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        /// <value>The current dispatcher.</value>
        protected virtual Dispatcher Dispatcher
        {
            get { return DispatcherHelper.CurrentDispatcher; }
        }
    }
}
