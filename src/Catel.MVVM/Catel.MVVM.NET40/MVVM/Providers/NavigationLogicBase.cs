// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationLogicBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using Catel.MVVM.Navigation;
    using Catel.MVVM.Views;
    using Logging;
    using MVVM;

    /// <summary>
    /// Base class for pages or controls containing navigation logic.
    /// </summary>
    /// <typeparam name="T">Type of the control or page.</typeparam>
    public abstract class NavigationLogicBase<T> : LogicBase
        where T : IView
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private NavigationAdapter _navigationAdapter;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PageLogic"/> class.
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

            CreateNavigationAdapter();
        }
        #endregion

        #region Properties
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
            get { return (T)TargetView; }
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
        private void CreateNavigationAdapter()
        {
            if (_navigationAdapter == null)
            {
                _navigationAdapter = new NavigationAdapter(TargetPage);
                _navigationAdapter.NavigatedTo += OnNavigatedTo;
                _navigationAdapter.NavigatingAway += OnNavigatingAway;
                _navigationAdapter.NavigatedAway += OnNavigatedAway;
            }
        }

        private void DestroyNavigationAdapter()
        {
            if (_navigationAdapter != null)
            {
                _navigationAdapter.UninitializeNavigationService();
                _navigationAdapter.NavigatedTo -= OnNavigatedTo;
                _navigationAdapter.NavigatingAway -= OnNavigatingAway;
                _navigationAdapter.NavigatedAway -= OnNavigatedAway;
                _navigationAdapter = null;
            }
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.TargetView"/> has just been loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetViewLoaded(object sender, EventArgs e)
        {
            base.OnTargetViewLoaded(sender, e);

            CreateNavigationAdapter();

            EnsureViewModel();
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.TargetView"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetViewUnloaded(object sender, EventArgs e)
        {
            base.OnTargetViewUnloaded(sender, e);

            DestroyNavigationAdapter();
        }

        private void OnNavigatedTo(object sender, NavigatedEventArgs e)
        {
            OnNavigatedToPage(e);
        }

        private void OnNavigatingAway(object sender, NavigatingEventArgs e)
        {
            OnNavigatingAwayFromPage(e);
        }

        private void OnNavigatedAway(object sender, NavigatedEventArgs e)
        {
            OnNavigatedAwayFromPage(e);
        }

        /// <summary>
        /// Called when the control has just been navigated to the page.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNavigatedToPage(NavigatedEventArgs e)
        {
            EnsureViewModel();

            var viewModelAsViewModelBase = ViewModel as ViewModelBase;
            if (viewModelAsViewModelBase != null)
            {
                var navigationContext = _navigationAdapter.NavigationContext;
                viewModelAsViewModelBase.UpdateNavigationContext(navigationContext);
            }
        }

        /// <summary>
        /// Called when the control has just been navigated away from the page.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNavigatingAwayFromPage(NavigatingEventArgs e)
        {
            bool? result = true;

            if (e.Uri != null && e.Uri.IsNavigationToExternal())
            {
                Log.Debug("Navigating away from the application");

                SaveAndCloseViewModel();

                return;
            }

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

            if (e.NavigationMode == NavigationMode.Back && !e.Cancel)
            {
                CloseViewModel(result);
            }
        }

        /// <summary>
        /// Called when the control has just been navigated away from the page.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNavigatedAwayFromPage(NavigatedEventArgs e)
        {
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
        #endregion
    }
}