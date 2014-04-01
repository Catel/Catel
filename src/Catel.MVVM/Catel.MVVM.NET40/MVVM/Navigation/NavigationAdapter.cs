// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapterBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Navigation
{
    using System;
    using Catel.Logging;
    using Catel.MVVM.Views;

    /// <summary>
    /// Navigation adapter class because everyone seems to be implementing their own :-(
    /// </summary>
    public partial class NavigationAdapter : NavigationAdapterBase
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private bool _navigationServiceInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationAdapter" /> class.
        /// </summary>
        /// <param name="navigationTarget">The navigation target.</param>
        /// <param name="isComingFromLoadedEvent">if set to <c>true</c>, the adapter is being created in the loaded event.</param>
        public NavigationAdapter(IView navigationTarget, bool isComingFromLoadedEvent)
        {
            Argument.IsNotNull("navigationTarget", navigationTarget);

            NavigationTarget = navigationTarget;
            NavigationTargetType = navigationTarget.GetType();
            NavigationContext = new NavigationContext();

            // Listen to loaded because not every framework already has the application at this stage
            NavigationTarget.Loaded += OnNavigationTargetLoaded;
        }

        #region Properties
        /// <summary>
        /// Gets the navigation target.
        /// </summary>
        /// <value>The navigation target.</value>
        public IView NavigationTarget { get; private set; }

        /// <summary>
        /// Gets the type of the navigation target.
        /// </summary>
        /// <value>The type of the navigation target.</value>
        public Type NavigationTargetType { get; private set; }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        public NavigationContext NavigationContext { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the app has navigated to this view.
        /// </summary>
        public event EventHandler<NavigatedEventArgs> NavigatedTo;

        /// <summary>
        /// Occurs when the app is about to navigate away from this view.
        /// </summary>
        public event EventHandler<NavigatingEventArgs> NavigatingAway;

        /// <summary>
        /// Occurs when the app has navigated away from this view.
        /// </summary>
        public event EventHandler<NavigatedEventArgs> NavigatedAway;
        #endregion

        #region Methods
        partial void DetermineNavigationContext();
        partial void Initialize();
        partial void Uninitialize();

        private void OnNavigationTargetLoaded(object sender, EventArgs e)
        {
            InitializeNavigationService(true);

            NavigationTarget.Loaded -= OnNavigationTargetLoaded;
        }

        private void InitializeNavigationService(bool isComingFromLoadedEvent)
        {
            if (_navigationServiceInitialized)
            {
                return;
            }

            Initialize();

            _navigationServiceInitialized = true;

            if (isComingFromLoadedEvent)
            {
                var eventArgs = new NavigatedEventArgs(string.Empty, NavigationMode.New);
                RaiseNavigatedTo(eventArgs);
            }
        }

        /// <summary>
        /// Uninitializes the navigation service.
        /// </summary>
        public void UninitializeNavigationService()
        {
            if (!_navigationServiceInitialized)
            {
                return;
            }

            Uninitialize();

            _navigationServiceInitialized = false;
        }

        /// <summary>
        /// Gets the navigation target as a type.
        /// </summary>
        /// <typeparam name="T">The type of the navigation target.</typeparam>
        /// <returns>The type.</returns>
        private T GetNavigationTarget<T>()
        {
            return (T) NavigationTarget;
        }

        private void HandleNavigatedEvent(NavigatedEventArgs e)
        {
            var eventArgs = new NavigatedEventArgs(e.Uri, NavigationMode.Unknown);

            // If this navigation event is not meant for this page, exit
            if (!eventArgs.Uri.IsNavigationForView(NavigationTargetType))
            {
                // We are not navigating *to* this view, but maybe we are navigating away
                RaiseNavigatedAway(eventArgs);
                return;
            }

            if (e.Uri != null && e.Uri.IsNavigationToExternal())
            {
                Log.Debug("Navigating away from the application, ignoring navigation");
                return;
            }

            RaiseNavigatedTo(eventArgs);
        }

        /// <summary>
        /// Gets the URI without parameters, thus <c>/Pages/MyView.xaml?id=1</c> will be returned as
        /// <c>/Pages/MyView.xaml</c>/
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The URI without parameters.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        private static string GetUriWithoutParameters(Uri uri)
        {
            Argument.IsNotNull("uri", uri);

            return GetUriWithoutParameters(uri.ToString());
        }

        /// <summary>
        /// Gets the URI without parameters, thus <c>/Pages/MyView.xaml?id=1</c> will be returned as
        /// <c>/Pages/MyView.xaml</c>/
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The URI without parameters.</returns>
        /// <exception cref="ArgumentException">The <paramref name="uri"/> is <c>null</c> or whitespace.</exception>
        private static string GetUriWithoutParameters(string uri)
        {
            Argument.IsNotNullOrWhitespace("uri", uri);

            int index = uri.IndexOf("?");
            if (index != -1)
            {
                uri = uri.Substring(0, index);
            }

            // Protect for double //
            uri = uri.Replace("//", "/");

            return uri;
        }

        /// <summary>
        /// Raises the <see cref="NavigatedTo"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedEventArgs"/> instance containing the event data.</param>
        protected void RaiseNavigatedTo(NavigatedEventArgs e)
        {
            if (!CanHandleNavigation())
            {
                return;
            }

            Log.Debug("Navigated to '{0}'", NavigationTargetType);

            DetermineNavigationContext();

            NavigatedTo.SafeInvoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="NavigatingAway"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingEventArgs"/> instance containing the event data.</param>
        protected void RaiseNavigatingAway(NavigatingEventArgs e)
        {
            if (!CanHandleNavigation())
            {
                return;
            }

            Log.Debug("Navigating away from '{0}'", NavigationTargetType);

            NavigatingAway.SafeInvoke(this, e);

            if (!e.Cancel && e.NavigationMode == NavigationMode.Back)
            {
                Uninitialize();
            }

            if (e.Cancel)
            {
                Log.Debug("Navigating away from '{0}' was canceled", NavigationTargetType);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavigatedAway"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedEventArgs"/> instance containing the event data.</param>
        protected void RaiseNavigatedAway(NavigatedEventArgs e)
        {
            if (!CanHandleNavigation())
            {
                return;
            }

            Log.Debug("Navigated away from '{0}'", NavigationTargetType);

            NavigatedAway.SafeInvoke(this, e);
        }
        #endregion
    }
}