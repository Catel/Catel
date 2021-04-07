// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationLogicBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using System.Threading.Tasks;
    using IoC;
    using Logging;
    using Navigation;
    using Views;
    using MVVM;
    using Catel.Services;

    /// <summary>
    /// Base class for pages or controls containing navigation logic.
    /// </summary>
    /// <typeparam name="T">Type of the control or page.</typeparam>
    public abstract class NavigationLogicBase<T> : LogicBase
        where T : class, IView
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
            if (_navigationAdapter is null)
            {
                var serviceLocator = this.GetServiceLocator();
                var navigationService = serviceLocator.ResolveType<INavigationRootService>();
                var navigationRoot = navigationService.GetNavigationRoot();

                _navigationAdapter = new NavigationAdapter(TargetPage, navigationRoot);
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
            if (_navigationAdapter is not null)
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

            if (ViewModelLifetimeManagement != ViewModelLifetimeManagement.FullyManual)
            {
                EnsureViewModel();
            }
            else
            {
                Log.Debug($"View model lifetime management is set to '{Enum<ViewModelLifetimeManagement>.ToString(ViewModelLifetimeManagement)}', not creating view model on loaded event for '{TargetViewType?.Name}'");
            }
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
            if (ViewModelLifetimeManagement != ViewModelLifetimeManagement.FullyManual)
            {
                EnsureViewModel();
            }
            else
            {
                Log.Debug($"View model lifetime management is set to '{Enum<ViewModelLifetimeManagement>.ToString(ViewModelLifetimeManagement)}', not creating view model on navigation event for '{TargetViewType?.Name}'");
            }

            var viewModelAsViewModelBase = ViewModel as ViewModelBase;
            if (viewModelAsViewModelBase is not null)
            {
                var navigationContext = _navigationAdapter.NavigationContext;
                viewModelAsViewModelBase.UpdateNavigationContext(navigationContext);
            }
        }

        /// <summary>
        /// Called when the control has just been navigated away from the page.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingEventArgs"/> instance containing the event data.</param>
#pragma warning disable AvoidAsyncVoid // Avoid async void
        protected async virtual void OnNavigatingAwayFromPage(NavigatingEventArgs e)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            bool? result = true;

            if (ViewModelLifetimeManagement != ViewModelLifetimeManagement.Automatic)
            {
                Log.Debug($"View model lifetime management is set to '{Enum<ViewModelLifetimeManagement>.ToString(ViewModelLifetimeManagement)}', not closing view model on navigation event for '{TargetViewType?.Name}'");
                return;
            }

            result = await SaveAndCloseViewModelAsync();

            if (e.Uri is not null && e.Uri.IsNavigationToExternal())
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
            if (vm is null)
            {
                vm = ConstructViewModelUsingArgumentOrDefaultConstructor(null);
                if (vm is null)
                {
                    throw Log.ErrorAndCreateException<InvalidViewModelException>("ViewModel cannot be null");
                }

                ViewModel = vm;
            }
        }
        #endregion
    }
}
