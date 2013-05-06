// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneApplicationPageLogic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls.MVVMProviders.Logic
{
    using System;
    using System.ComponentModel;
    using System.Windows.Navigation;
    using Logging;
    using MVVM;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using ViewModelBase = MVVM.ViewModelBase;

    /// <summary>
    /// MVVM Provider behavior implementation for a phone application page.
    /// </summary>
    public class PhoneApplicationPageLogic : NavigationLogicBase<PhoneApplicationPage>
    {
        #region Constants
        /// <summary>
        /// The key in which the view model will be stored during tombstoning.
        /// </summary>
        public const string TombstoneKey = "tsvm";
        #endregion

        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// A boolean representing whether the application has recovered from tombstoning.
        /// </summary>
        private static bool _recoveredFromTombstoning = false;

        private bool _hasNavigatedAway = false;
        private bool _hasPressedBackButton = false;
        private bool _hasNavigatedAwayAfterBackButtonPress = false;
        private static bool _isTombstoned;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneApplicationPageLogic"/> class.
        /// </summary>
        /// <param name="targetPage">The phone application page this provider should take care of.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetPage"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> does not implement interface <see cref="IViewModel"/>.</exception>
        public PhoneApplicationPageLogic(PhoneApplicationPage targetPage, Type viewModelType)
            : base(targetPage, viewModelType)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the back key cancels the view model. This
        /// means that <see cref="IViewModel.CancelViewModel"/> will be called when the back key is pressed.
        /// <para />
        /// If this property is <c>false</c>, the <see cref="IViewModel.SaveViewModel"/> will be called instead.
        /// <para />
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the back key cancels the view model; otherwise, <c>false</c>.
        /// </value>
        public bool BackKeyCancelsViewModel { get; set; }

        /// <summary>
        /// Gets a value indicating whether the control can be loaded. This is very useful in non-WPF classes where
        /// the <c>LayoutUpdated</c> is used instead of the <c>Loaded</c> event.
        /// <para />
        /// If this value is <c>true</c>, this logic implementation can call the <see cref="OnTargetControlLoaded" /> when
        /// the control is loaded. Otherwise, the call will be ignored.
        /// </summary>
        /// <value><c>true</c> if this instance can control be loaded; otherwise, <c>false</c>.</value>
        /// <remarks>This value is introduced for Windows Phone because a navigation backwards still leads to a call to
        /// <c>LayoutUpdated</c>. To prevent new view models from being created, this property can be overridden by
        /// such logic implementations.</remarks>
        protected override bool CanControlBeLoaded
        {
            get { return !_hasNavigatedAway; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can handle navigation.
        /// </summary>
        /// <value><c>true</c> if this instance can handle navigation; otherwise, <c>false</c>.</value>
        protected override bool CanHandleNavigation
        {
            get { return !_hasNavigatedAwayAfterBackButtonPress; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view is about to tombstone itself.
        /// </summary>
        public event EventHandler<EventArgs> Tombstoning;

        /// <summary>
        /// Occurs when the view has just tombstoned itself.
        /// </summary>
        public event EventHandler<EventArgs> Tombstoned;

        /// <summary>
        /// Occurs when the view is about to recover itself from a tombstoned state.
        /// </summary>
        public event EventHandler<EventArgs> RecoveringFromTombstoning;

        /// <summary>
        /// Occurs when the view has just recovered itself from a tombstoned state.
        /// </summary>
        public event EventHandler<EventArgs> RecoveredFromTombstoning;
        #endregion

        #region Methods
        /// <summary>
        /// Sets the data context of the target control.
        /// <para />
        /// This method is abstract because the real logic implementation knows how to set the data context (for example,
        /// by using an additional data context grid).
        /// </summary>
        /// <param name="newDataContext">The new data context.</param>
        protected override void SetDataContext(object newDataContext)
        {
            TargetControl.DataContext = newDataContext;
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.ViewModel" /> property has just been changed.
        /// </summary>
        protected override void OnViewModelChanged()
        {
            if (ViewModel == null)
            {
                TargetControl.DataContext = null;
            }

            base.OnViewModelChanged();
        }

        /// <summary>
        /// Called when the target control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetControlLoaded(object sender, EventArgs e)
        {
            base.OnTargetControlLoaded(sender, e);

            TargetPage.BackKeyPress += OnTargetPageBackKeyPress;
            //PhoneApplicationService.Current.Deactivated += OnApplicationDeactivated;
        }

        /// <summary>
        /// Called when the target control is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetControlUnloaded(object sender, EventArgs e)
        {
            TargetPage.BackKeyPress -= OnTargetPageBackKeyPress;
            //PhoneApplicationService.Current.Deactivated -= OnApplicationDeactivated;

            base.OnTargetControlUnloaded(sender, e);
        }

        /// <summary>
        /// This method is called when the hardware back key is pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Set e.Cancel to true to indicate that the request was handled by the application.</param>
        private void OnTargetPageBackKeyPress(object sender, CancelEventArgs e)
        {
            if (BackKeyCancelsViewModel)
            {
                CancelAndCloseViewModel();
            }
            else
            {
                SaveAndCloseViewModel();
            }

            HasHandledSaveAndCancelLogic = true;
            _hasPressedBackButton = true;
        }

        /// <summary> 
        /// Tombstones the application.
        /// </summary>
        private void TombstoneIfRequired(NavigatingCancelEventArgs e)
        {
            if (_isTombstoned)
            {
                return;
            }

            var uriString = e.GetUriWithoutQueryInfo();
            if (!uriString.StartsWith("app://external", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var viewModelAsViewModelBase = ViewModel as ViewModelBase;
            if (viewModelAsViewModelBase != null)
            {
                Tombstoning.SafeInvoke(this);

                switch (viewModelAsViewModelBase.TombstoningMode)
                {
                    case TombstoningMode.Disabled:
                        break;

                    case TombstoningMode.Manual:
                        viewModelAsViewModelBase.PrepareForTombstoneStateInternal(PhoneApplicationService.Current.State);
                        break;

                    case TombstoningMode.Auto:
                        var tombstoneData = ((ViewModelBase)ViewModel).SerializeForTombstoning();
                        PhoneApplicationService.Current.State[TombstoneKey] = tombstoneData;

                        Log.Debug("Automatically created tombstone data with key '{0}'", TombstoneKey);
                        break;
                }

                Tombstoned.SafeInvoke(this);
            }

            _isTombstoned = true;
        }

        /// <summary>
        /// Recovers from tombstone.
        /// </summary>
        private void RecoverFromTombstoneIfRequired()
        {
            if (!_isTombstoned)
            {
                return;
            }

            if (PhoneApplicationService.Current.StartupMode == StartupMode.Activate)
            {
                // We just recovered from tombstoning
                if (!_recoveredFromTombstoning)
                {
                    _recoveredFromTombstoning = true;

                    RecoveringFromTombstoning.SafeInvoke(this);

                    EnsureViewModel();

                    var viewModel = ViewModel as ViewModelBase;
                    if (viewModel != null)
                    {
                        switch (viewModel.TombstoningMode)
                        {
                            case TombstoningMode.Disabled:
                                break;

                            case TombstoningMode.Manual:
                                viewModel.RecoverFromTombstoneStateInternal(PhoneApplicationService.Current.State);
                                break;

                            case TombstoningMode.Auto:
                                if (PhoneApplicationService.Current.State.ContainsKey(TombstoneKey))
                                {
                                    byte[] data = PhoneApplicationService.Current.State[TombstoneKey] as byte[];
                                    if (data != null)
                                    {
                                        viewModel.DeserializeFromTombstoning(data);

                                        Log.Debug("Automatically recovered from tombstone data", TombstoneKey);
                                    }
                                }
                                else
                                {
                                    Log.Warning("No tombstone data available with key '{0}', no automatic recovery possible", TombstoneKey);
                                }
                                break;
                        }
                    }

                    RecoveredFromTombstoning.SafeInvoke(this);
                }
            }

            _isTombstoned = false;
        }

        /// <summary>
        /// Called when the control is about to navigate.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs" /> instance containing the event data.</param>
        protected override void OnNavigating(NavigatingCancelEventArgs e)
        {
            _hasNavigatedAway = true;

            if (_hasPressedBackButton)
            {
                _hasNavigatedAwayAfterBackButtonPress = true;
            }

            // Manual tombstoning in navigation because otherwise it will be too late to handle tombstoning
            TombstoneIfRequired(e);
        }

        /// <summary>
        /// Called when the control has just navigated.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected override void OnNavigated(NavigationEventArgs e)
        {
            RecoverFromTombstoneIfRequired();

            _hasNavigatedAway = false;
        }
        #endregion
    }
}
