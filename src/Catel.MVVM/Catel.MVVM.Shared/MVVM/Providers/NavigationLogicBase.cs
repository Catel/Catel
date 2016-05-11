// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationLogicBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Navigation;
    using Views;
    using MVVM;

    /// <summary>
    /// Base class for pages or controls containing navigation logic.
    /// </summary>
    /// <typeparam name="T">Type of the control or page.</typeparam>
    public abstract class NavigationLogicBase<T> : LogicBase
        where T : IView
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private NavigationAdapter _navigationAdapter;

        private bool _hasHandledNavigatingAway;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PageLogic"/> class.
        /// </summary>
        /// <param name="targetPage">The page this provider should take care of.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetPage"/> is <c>null</c>.</exception>
        protected NavigationLogicBase(T targetPage, Type viewModelType = null)
            : base(targetPage, viewModelType)
        {
            CreateNavigationAdapter(false);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target page.
        /// </summary>
        /// <value>The target page.</value>
        public T TargetPage
        {
            get { return (T)TargetView; }
        }
        #endregion

        #region Methods
        private void CreateNavigationAdapter(bool comingFromLoadedEvent)
        {
            if (_navigationAdapter == null)
            {
                _navigationAdapter = new NavigationAdapter(TargetPage);
                _navigationAdapter.NavigatedTo += OnNavigatedTo;
                _navigationAdapter.NavigatingAway += OnNavigatingAway;
                _navigationAdapter.NavigatedAway += OnNavigatedAway;

                if (comingFromLoadedEvent)
                {
                    OnNavigatedTo(this, new NavigatedEventArgs(_navigationAdapter.GetNavigationUriForTargetPage(), NavigationMode.New));
                }
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
        public override async Task OnTargetViewLoadedAsync(object sender, EventArgs e)
        {
            _hasHandledNavigatingAway = false;

            await base.OnTargetViewLoadedAsync(sender, e);

            CreateNavigationAdapter(true);

            EnsureViewModel();
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.TargetView"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override async Task OnTargetViewUnloadedAsync(object sender, EventArgs e)
        {
            if (!_hasHandledNavigatingAway)
            {
                OnNavigatingAway(this, new NavigatingEventArgs(_navigationAdapter.GetNavigationUriForTargetPage(), NavigationMode.New));

                _hasHandledNavigatingAway = true;
            }

            await base.OnTargetViewUnloadedAsync(sender, e);

            DestroyNavigationAdapter();
        }

        private void OnNavigatedTo(object sender, NavigatedEventArgs e)
        {
            OnNavigatedToPage(e);
        }

        private void OnNavigatingAway(object sender, NavigatingEventArgs e)
        {
            OnNavigatingAwayFromPage(e);

            if (!e.Cancel)
            {
                _hasHandledNavigatingAway = true;
            }
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
        protected async virtual void OnNavigatingAwayFromPage(NavigatingEventArgs e)
        {
            bool? result = true;

            result = await SaveAndCloseViewModelAsync();

            if (e.Uri != null && e.Uri.IsNavigationToExternal())
            {
                return;
            }

            if (!result.HasValue || !result.Value)
            {
                e.Cancel = true;
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
            var vm = ViewModel;
            if (vm == null)
            {
                vm = ConstructViewModelUsingArgumentOrDefaultConstructor(null);
                if (vm == null)
                {
                    throw Log.ErrorAndCreateException<InvalidViewModelException>("ViewModel cannot be null");
                }

                ViewModel = vm;
            }
        }
        #endregion
    }
}