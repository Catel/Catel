// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationLogicBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls.MVVMProviders.Logic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Logging;
    using MVVM;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#elif WINDOWS_PHONE
    using Microsoft.Phone.Controls;
#elif SILVERLIGHT
    using System.Windows.Controls;
#endif

#if NETFX_CORE
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;

    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
    using System.Windows.Navigation;

    using UIEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// Base class for pages or controls containing navigation logic.
    /// </summary>
    /// <typeparam name="T">Type of the control or page.</typeparam>
    public abstract class NavigationLogicBase<T> : LogicBase
        where T : FrameworkElement
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if NETFX_CORE
        private Frame _rootFrame;
#elif WINDOWS_PHONE
        private PhoneApplicationFrame _rootFrame;
#elif SILVERLIGHT
        private Frame _rootFrame;
#endif

        private bool _hasNavigatedButNotNavigatedAway;
        private bool _navigationServiceInitialized;
        private bool _navigationComplete;

#if WINDOWS_PHONE
        /// <summary>
        /// The url to which this logic belongs. This way, we can distinguish the right call from the navigation service.
        /// </summary>
        private string _url;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationPageLogic"/> class.
        /// </summary>
        /// <param name="targetPage">The page this provider should take care of.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetPage"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> does not implement interface <see cref="IViewModel"/>.</exception>
        protected NavigationLogicBase(T targetPage, Type viewModelType)
            : base(targetPage, viewModelType)
        {
            NavigatingAwaySavesViewModel = true;

            InitializeNavigationService(false);

            EnsureViewModel();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the logic can currently handle navigation.
        /// </summary>
        protected virtual bool CanHandleNavigation
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has handled save and cancel logic.
        /// </summary>
        /// <value><c>true</c> if this instance has handled save and cancel logic; otherwise, <c>false</c>.</value>
        protected bool HasHandledSaveAndCancelLogic { get; set; }

        /// <summary>
        /// Gets the target page.
        /// </summary>
        /// <value>The target page.</value>
        public T TargetPage
        {
            get { return (T)TargetControl; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether navigating away from the page should save the view model.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if navigating away should save the view model; otherwise, <c>false</c>.
        /// </value>
        public bool NavigatingAwaySavesViewModel { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the <see cref="LogicBase.TargetControl"/> has just been loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetControlLoaded(object sender, UIEventArgs e)
        {
            base.OnTargetControlLoaded(sender, e);

            InitializeNavigationService(true);
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.TargetControl"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetControlUnloaded(object sender, UIEventArgs e)
        {
            base.OnTargetControlUnloaded(sender, e);

#if NET
            Application.Current.Navigated -= OnNavigatedEvent;
#endif

            ViewModel = null;
        }

        /// <summary>
        /// Called when the <c>Navigating</c> event is invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public due to the fact that weak events are used. Otherwise, the navigation completed events
        /// could not be handled (because we unsubscribed from the _rootFrame) when navigating away to prevent memory
        /// leaks.
        /// <para />
        /// Please, do not call this method yourself, otherwise you can seriously ruin your apps.
        /// </remarks>
        public void OnNavigatingEvent(object sender, NavigatingCancelEventArgs e)
        {
            if (!CanHandleNavigation)
            {
                return;
            }

            _hasNavigatedButNotNavigatedAway = false;

#if WINDOWS_PHONE
            var uriWithoutParameters = GetUriWithoutParameters(_rootFrame.CurrentSource);
            if (string.CompareOrdinal(uriWithoutParameters, _url) != 0)
            {
                return;
            }
#endif

            OnNavigating(e);

            if (!e.Cancel && !HasHandledSaveAndCancelLogic)
            {
                if (NavigatingAwaySavesViewModel)
                {
                    SaveAndCloseViewModel();
                }
                else
                {
                    CancelAndCloseViewModel();
                }

                HasHandledSaveAndCancelLogic = true;

#if NETFX_CORE
                _rootFrame.Navigating -= OnNavigatingEvent;
                _rootFrame.Navigated -= OnNavigatedEvent;
#elif WINDOWS_PHONE
                //_rootFrame.BackKeyPress -= OnBackKeyPress;
                //_rootFrame.Navigating -= OnNavigatingEvent;
                //_rootFrame.Navigated -= OnNavigatedEvent;
#elif SILVERLIGHT
                _rootFrame.Navigating -= OnNavigatingEvent;
                _rootFrame.Navigated -= OnNavigatedEvent;
#else
                Application.Current.Navigating -= OnNavigatingEvent;
                Application.Current.Navigated -= OnNavigatedEvent;
#endif
            }

            _navigationComplete = false;
        }

        /// <summary>
        /// Called when the control is about to navigate.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNavigating(NavigatingCancelEventArgs e)
        {
        }

        /// <summary>
        /// Called when the <c>Navigated</c> event is invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public due to the fact that weak events are used. Otherwise, the navigation completed events
        /// could not be handled (because we unsubscribed from the _rootFrame) when navigating away to prevent memory
        /// leaks.
        /// <para />
        /// Please, do not call this method yourself, otherwise you can seriously ruin your apps.
        /// </remarks>
        public void OnNavigatedEvent(object sender, NavigationEventArgs e)
        {
            if (!CanHandleNavigation)
            {
                return;
            }

            // If this navigation event is not meant for this page, exit
            if (!e.IsNavigationForView(TargetControlType))
            {
                return;
            }

            HasHandledSaveAndCancelLogic = false;

            if (_hasNavigatedButNotNavigatedAway)
            {
                return;
            }

            _hasNavigatedButNotNavigatedAway = true;

            if (e.Uri != null && e.Uri.ToString().Contains("app://external"))
            {
                Log.Debug("Navigating away from the application, ignoring navigation");
                return;
            }

            OnNavigated(e);

#if NETFX_CORE
            var navigationContext = e.Parameter;
#elif WINDOWS_PHONE
            var navigationContext = ((PhoneApplicationPage)TargetControl).NavigationContext;
#elif SILVERLIGHT
            var navigationContext = e.Content;
#else
            var navigationContext = e.ExtraData;
#endif

            HandleNavigated(navigationContext);
        }

        /// <summary>
        /// Called when the control has just navigated.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNavigated(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Handles the navigated event.
        /// </summary>
        /// <remarks>
        /// This is a separate method and not handled in the <see cref="OnNavigatedEvent"/> method because it should be
        /// possible to call this without actually having event args.
        /// </remarks>
        private void HandleNavigated(object navigationContext)
        {
#if WINDOWS_PHONE
            string uriWithoutParameters = GetUriWithoutParameters(_rootFrame.CurrentSource);

            if (string.IsNullOrEmpty(_url))
            {
                _url = uriWithoutParameters;
            }

            if (string.CompareOrdinal(uriWithoutParameters, _url) != 0)
            {
                return;
            }
#endif

            if (navigationContext != null)
            {
                CompleteNavigation(navigationContext);
            }
        }

        /// <summary>
        /// Initializes the navigation service. If the navigation service is already initialized, it won't be initialized
        /// again. However, sometimes the RootFrame of a windows phone 7 application is not yet available at startup. Therefore,
        /// this method must be called in both the constructor and the <see cref="OnTargetControlLoaded"/> methods.
        /// </summary>
        /// <param name="isComingFromLoadedEvent">if set to <c>true</c>, this method is called from the loaded event, which means
        /// that the <c>Navigated</c> event already occurred and must be invoked manually.</param>
        private void InitializeNavigationService(bool isComingFromLoadedEvent)
        {
            if (_navigationServiceInitialized)
            {
                return;
            }

#if NETFX_CORE
            _rootFrame = Window.Current.Content as Frame ?? ((Page)TargetControl).Frame;
            if (_rootFrame == null)
            {
                return;
            }

            //this.SubscribeToWeakGenericEvent<NavigatingCancelEventArgs>(_rootFrame, "Navigating", OnNavigatingEvent);
            //this.SubscribeToWeakGenericEvent<NavigationEventArgs>(_rootFrame, "Navigated", OnNavigatedEvent);
            _rootFrame.Navigating += OnNavigatingEvent;
            _rootFrame.Navigated += OnNavigatedEvent;
#elif WINDOWS_PHONE
            _rootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is PhoneApplicationFrame) as PhoneApplicationFrame;
            if (_rootFrame == null)
            {
                return;
            }

            //this.SubscribeToWeakGenericEvent<CancelEventArgs>(_rootFrame, "BackKeyPress", OnBackKeyPress);
            this.SubscribeToWeakGenericEvent<NavigatingCancelEventArgs>(_rootFrame, "Navigating", OnNavigatingEvent);
            this.SubscribeToWeakGenericEvent<NavigationEventArgs>(_rootFrame, "Navigated", OnNavigatedEvent);
            //_rootFrame.Navigating += OnNavigatingEvent;
            //_rootFrame.Navigated += OnNavigatedEvent;
#elif SILVERLIGHT
            _rootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is Frame) as Frame;
            if (_rootFrame == null)
            {
                return;
            }

            //this.SubscribeToWeakGenericEvent<NavigatingCancelEventArgs>(_rootFrame, "Navigating", OnNavigatingEvent);
            //this.SubscribeToWeakGenericEvent<NavigationEventArgs>(_rootFrame, "Navigated", OnNavigatedEvent);
            _rootFrame.Navigating += OnNavigatingEvent;
            _rootFrame.Navigated += OnNavigatedEvent;
#else
            //this.SubscribeToWeakGenericEvent<NavigatingCancelEventArgs>(Application.Current, "Navigating", OnNavigatingEvent);
            //this.SubscribeToWeakGenericEvent<NavigationEventArgs>(Application.Current, "Navigated", OnNavigatedEvent);
            Application.Current.Navigating += OnNavigatingEvent;
            Application.Current.Navigated += OnNavigatedEvent;
#endif

            _navigationServiceInitialized = true;

            if (isComingFromLoadedEvent)
            {
                HandleNavigated(null);
            }
        }

        /// <summary>
        /// Ensures that there is a valid view model. This is a separate method because the view model can be constructed
        /// in the constructor, but also in the OnLoaded event because of the tombstoning capabilities of Windows Phone 7.
        /// <para/>
        /// If there already is a view model, this method will do nothing.
        /// </summary>
        protected void EnsureViewModel()
        {
            if (ViewModel == null)
            {
                ViewModel = ConstructViewModelUsingArgumentOrDefaultConstructor(null);

                if (ViewModel == null)
                {
                    throw new InvalidViewModelException();
                }
            }
        }

        /// <summary>
        /// Completes the navigation.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        private void CompleteNavigation(object navigationContext)
        {
            if (_navigationComplete)
            {
                return;
            }

            EnsureViewModel();

            var viewModelAsViewModelBase = ViewModel as ViewModelBase;
            if (viewModelAsViewModelBase != null)
            {
#if SILVERLIGHT
                viewModelAsViewModelBase.UpdateNavigationContext(ConvertNavigationContextToDictionary(navigationContext as NavigationContext));
#else
                viewModelAsViewModelBase.UpdateNavigationContext(navigationContext as Dictionary<string, object>);
#endif
            }

            _navigationComplete = true;
        }

#if SILVERLIGHT
        /// <summary>
        /// Converts the navigation context to dictionary.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><see cref="Dictionary{TKey,TValue}"/> with all the data from the context.</returns>
        private static Dictionary<string, string> ConvertNavigationContextToDictionary(NavigationContext context)
        {
            var dictionary = new Dictionary<string, string>();

            if (context != null)
            {
                foreach (string key in context.QueryString.Keys)
                {
                    dictionary.Add(key, context.QueryString[key]);
                }
            }

            return dictionary;
        }
#endif

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
        #endregion
    }
}