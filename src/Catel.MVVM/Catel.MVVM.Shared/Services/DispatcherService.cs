// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;
using Xamarin.Forms;

namespace Catel.Services
{
    using System;
    using Logging;

#if ANDROID
    using global::Android.App;
    using global::Android.OS;
#elif IOS
    using global::CoreFoundation;
#elif NETFX_CORE
    using Windows.Threading;
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#elif !XAMARIN_FORMS
    using Windows.Threading;
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if !XAMARIN
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherService"/> class.
        /// </summary>
        public DispatcherService()
        {
            // Get current dispatcher to make sure we have one
            var currentDispatcher = CurrentDispatcher;
            if (currentDispatcher != null)
            {
                Log.Debug("Successfully Initialized current dispatcher");
            }
            else
            {
                Log.Warning("Failed to retrieve the current dispatcher at this point, will try again later");
            }
        }
#endif

#if ANDROID
        private readonly Handler _handler = new Handler(Looper.MainLooper);
#elif !XAMARIN
        /// <summary>
        /// Gets the current dispatcher.
        /// <para />
        /// Internally, this property uses the <see cref="DispatcherHelper"/>, but can be overriden if required.
        /// </summary>
        public virtual Dispatcher CurrentDispatcher
        {
            get { return DispatcherHelper.CurrentDispatcher; }
        }
#endif

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "void Invoke(Action action, bool onlyInvokeWhenNoAccess);", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public void Invoke(Action action)
        {
            Invoke(action, true);
        }

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public void Invoke(Action action, bool onlyInvokeWhenNoAccess)
        {
            Argument.IsNotNull("action", action);
#if XAMARIN_FORMS
            SynchronizationContext.Current.Post(state => action(), null);
#elif ANDROID
            _handler.Post(action);
#elif IOS
            DispatchQueue.MainQueue.DispatchSync(() => action());
#else
            var dispatcher = CurrentDispatcher;
            DispatcherExtensions.Invoke(dispatcher, action, onlyInvokeWhenNoAccess);
#endif
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess)
        {
            Argument.IsNotNull("action", action);
#if XAMARIN_FORMS
            SynchronizationContext.Current.Post(state => action(), null);           
#elif ANDROID
            _handler.Post(action);
#elif IOS
            DispatchQueue.MainQueue.DispatchAsync(() => action());
#else
            var dispatcher = CurrentDispatcher;
            DispatcherExtensions.BeginInvoke(dispatcher, action, onlyBeginInvokeWhenNoAccess);
#endif
        }
    }
}