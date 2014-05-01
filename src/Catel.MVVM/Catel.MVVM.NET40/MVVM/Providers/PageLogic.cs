// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationPageLogic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using Navigation;
    using Views;
    using MVVM;

    /// <summary>
    /// MVVM Provider behavior implementation for a navigation page.
    /// </summary>
    public class PageLogic : NavigationLogicBase<IPage>
    {
        private bool _hasNavigatedAway = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageLogic"/> class.
        /// </summary>
        /// <param name="targetPage">The page this provider should take care of.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetPage"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> does not implement interface <see cref="IViewModel"/>.</exception>
        public PageLogic(IPage targetPage, Type viewModelType)
            : base(targetPage, viewModelType)
        {
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the view model should be closed when navigating forward.
        /// <para />
        /// By default, Catel will keep the view models and pages in memory to provide a back-navigation stack. Some
        /// pages are not required to be listed in the navigation stack and can have this property set to <c>true</c>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if the view modle must be closed on forward navigation; otherwise, <c>false</c>.</value>
        public bool CloseViewModelOnForwardNavigation { get; set; }
        #endregion

        /// <summary>
        /// Sets the data context of the target control.
        /// <para />
        /// This method is abstract because the real logic implementation knows how to set the data context (for example,
        /// by using an additional data context grid).
        /// </summary>
        /// <param name="newDataContext">The new data context.</param>
        protected override void SetDataContext(object newDataContext)
        {
            TargetView.DataContext = newDataContext;
        }

        /// <summary>
        /// Gets a value indicating whether the control can be loaded. This is very useful in non-WPF classes where
        /// the <c>LayoutUpdated</c> is used instead of the <c>Loaded</c> event.
        /// <para />
        /// If this value is <c>true</c>, this logic implementation can call the  <see cref="NavigationLogicBase{T}.OnTargetViewLoaded" /> when the 
        /// control is loaded. Otherwise, the call will be ignored.
        /// </summary>
        /// <value><c>true</c> if this instance can control be loaded; otherwise, <c>false</c>.</value>
        /// <remarks>This value is introduced for Windows Phone because a navigation backwards still leads to a call to
        /// <c>LayoutUpdated</c>. To prevent new view models from being created, this property can be overridden by 
        /// such logic implementations.</remarks>
        protected override bool CanViewBeLoaded
        {
            get { return !_hasNavigatedAway; }
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.ViewModel" /> property has just been changed.
        /// </summary>
        protected override void OnViewModelChanged()
        {
            if (ViewModel == null)
            {
                TargetView.DataContext = null;
            }

            base.OnViewModelChanged();
        }

        /// <summary>
        /// Called when the control has just navigated away from this page.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedAwayFromPage(NavigatedEventArgs e)
        {
            if (e.Uri.IsNavigationToExternal())
            {
                // Don't handle navigation to external page, we are being deactivated (but not tombstoned)
                return;
            }

            _hasNavigatedAway = true;

            if (CloseViewModelOnForwardNavigation)
            {
                SaveAndCloseViewModel();

                HasHandledSaveAndCancelLogic = true;
            }

            base.OnNavigatedAwayFromPage(e);
        }
    }
}
