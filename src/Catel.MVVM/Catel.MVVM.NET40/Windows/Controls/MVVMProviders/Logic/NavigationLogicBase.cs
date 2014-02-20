// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationLogicBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
    using Microsoft.Phone.Shell;
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

#if SILVERLIGHT
    using NavigationContextType = System.Collections.Generic.Dictionary<string, string>;
#else
    using NavigationContextType = System.Collections.Generic.Dictionary<string, object>;
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

        private bool _navigationServiceInitialized;
        private bool _navigatingToViewComplete;
        private bool _navigatingAwayFromViewCompleted;
        private bool _hasSetNavigationContextOnce;
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
        /// Gets the last navigation mode received.
        /// </summary>
        /// <value>The last navigation mode.</value>
        protected NavigationMode LastNavigationMode { get; private set; }

        /// <summary>
        /// Gets the target page.
        /// </summary>
        /// <value>The target page.</value>
        public T TargetPage
        {
            get { return (T)TargetControl; }
        }

#if NETFX_CORE
        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>The root frame.</value>
        protected Frame RootFrame { get; private set; }
#elif WINDOWS_PHONE
        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>The root frame.</value>
        protected PhoneApplicationFrame RootFrame { get; private set; }
#elif SILVERLIGHT
        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>The root frame.</value>
        protected Frame RootFrame { get; private set; }
#endif

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

            //ViewModel = null;
        }

        /// <summary>
        /// Called when the <c>Navigating</c> event is invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public due to the fact that weak events are used. Otherwise, the navigation completed events
        /// could not be handled (because we unsubscribed from the RootFrame) when navigating away to prevent memory
        /// leaks.
        /// <para />
        /// Please, do not call this method yourself, otherwise you can seriously ruin your apps.
        /// </remarks>
        public void OnNavigatingEvent(object sender, NavigatingCancelEventArgs e)
        {
            LastNavigationMode = e.NavigationMode;

            if (!CanHandleNavigation)
            {
                return;
            }

            if (!CanHandleNavigationAdvanced())
            {
                return;
            }

#if !NETFX_CORE
            if (e.Uri != null && e.Uri.IsNavigationToExternal())
            {
                Log.Debug("Navigating away from the application");

                _navigatingToViewComplete = false;
                _navigatingAwayFromViewCompleted = false;

                SaveAndCloseViewModel();

                return;
            }
#endif

            if (_navigatingToViewComplete && !_navigatingAwayFromViewCompleted)
            {
                // We are navigating away
                OnNavigatingAwayFromPage(e);
            }

            // Reset (if we navigate back to this page which has already been navigated to)
            if (e.IsNavigationForView(TargetControlType))
            {
                _navigatingToViewComplete = false;
                _navigatingAwayFromViewCompleted = false;
            }
        }

        /// <summary>
        /// Called when the <c>Navigated</c> event is invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public due to the fact that weak events are used. Otherwise, the navigation completed events
        /// could not be handled (because we unsubscribed from the RootFrame) when navigating away to prevent memory
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
                // We are not navigating *to* this view, but maybe we are navigating away
                if (!_navigatingAwayFromViewCompleted)
                {
                    OnNavigatedAwayFromPage(e);
                    _navigatingAwayFromViewCompleted = true;
                    return;
                }

                return;
            }

            if (!CanHandleNavigationAdvanced())
            {
                return;
            }

            if (_navigatingToViewComplete)
            {
                return;
            }

            if (e.Uri != null && e.Uri.IsNavigationToExternal())
            {
                Log.Debug("Navigating away from the application, ignoring navigation");
                return;
            }

            OnNavigatedToPage(e);

#if NETFX_CORE
            var navigationContext = e.Parameter;
#elif WINDOWS_PHONE
            var navigationContext = ((PhoneApplicationPage)TargetControl).NavigationContext;
#elif SILVERLIGHT
            var navigationContext = ((Page)e.Content).NavigationContext;
#else
            var navigationContext = e.ExtraData;
#endif

            HandleNavigated(navigationContext);

            HasHandledSaveAndCancelLogic = false;
            _navigatingToViewComplete = true;
        }

        /// <summary>
        /// Called when the control has just been navigated to the page.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNavigatedToPage(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Called when the control has just been navigated away from the page.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNavigatingAwayFromPage(NavigatingCancelEventArgs e)
        {
            bool? result = true;

            if (!HasHandledSaveAndCancelLogic)
            {
                if (NavigatingAwaySavesViewModel)
                {
                    result = SaveViewModel();
                }
                else
                {
                    result = CancelViewModel();
                }
            }

            if (!result.HasValue || !result.Value)
            {
                e.Cancel = true;
            }

            HasHandledSaveAndCancelLogic = true;

            if (LastNavigationMode == NavigationMode.Back)
            {
                CloseViewModel(result);

                // Only unsubscribe when navigating back
#if NETFX_CORE
                RootFrame.Navigating -= OnNavigatingEvent;
                RootFrame.Navigated -= OnNavigatedEvent;
#elif WINDOWS_PHONE
                // No need because we are using weak events for phone
                //RootFrame.BackKeyPress -= OnBackKeyPress;
                //RootFrame.Navigating -= OnNavigatingEvent;
                //RootFrame.Navigated -= OnNavigatedEvent;
#elif SILVERLIGHT
                RootFrame.Navigating -= OnNavigatingEvent;
                RootFrame.Navigated -= OnNavigatedEvent;
#else
                Application.Current.Navigating -= OnNavigatingEvent;
                Application.Current.Navigated -= OnNavigatedEvent;
#endif
            }
        }

        /// <summary>
        /// Called when the control has just been navigated away from the page.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNavigatedAwayFromPage(NavigationEventArgs e)
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
            RootFrame = Window.Current.Content as Frame ?? ((Page)TargetControl).Frame;
            if (RootFrame == null)
            {
                return;
            }

            RootFrame.Navigating += OnNavigatingEvent;
            RootFrame.Navigated += OnNavigatedEvent;
#elif WINDOWS_PHONE
            RootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is PhoneApplicationFrame) as PhoneApplicationFrame;
            if (RootFrame == null)
            {
                return;
            }

            //this.SubscribeToWeakGenericEvent<CancelEventArgs>(RootFrame, "BackKeyPress", OnBackKeyPress);
            this.SubscribeToWeakGenericEvent<NavigatingCancelEventArgs>(RootFrame, "Navigating", OnNavigatingEvent);
            this.SubscribeToWeakGenericEvent<NavigationEventArgs>(RootFrame, "Navigated", OnNavigatedEvent);
#elif SILVERLIGHT
            RootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is Frame) as Frame;
            if (RootFrame == null)
            {
                return;
            }

            RootFrame.Navigating += OnNavigatingEvent;
            RootFrame.Navigated += OnNavigatedEvent;
#else
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
        /// Determines whether this instance can handle the current navigation event.
        /// <para />
        /// This method should only be implemented by deriving types if the <see cref="CanHandleNavigation"/>
        /// is not sufficient.
        /// </summary>
        /// <returns><c>true</c> if this instance can handle the navigation event; otherwise, <c>false</c>.</returns>
        protected virtual bool CanHandleNavigationAdvanced()
        {
            return true;
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
            if (_navigatingToViewComplete)
            {
                return;
            }

            EnsureViewModel();

            var viewModelAsViewModelBase = ViewModel as ViewModelBase;
            if (viewModelAsViewModelBase != null)
            {
                NavigationContextType finalNavigationContext = null;

                if (!_hasSetNavigationContextOnce)
                {
#if SILVERLIGHT
                    finalNavigationContext = ConvertNavigationContextToDictionary(navigationContext as NavigationContext);
#else
                    finalNavigationContext = navigationContext as NavigationContextType;
#endif

                    _hasSetNavigationContextOnce = true;
                }

                viewModelAsViewModelBase.UpdateNavigationContext(finalNavigationContext);
            }

            _navigatingToViewComplete = true;
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