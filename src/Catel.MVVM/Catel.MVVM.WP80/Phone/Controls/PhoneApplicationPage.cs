// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneApplicationPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Phone.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using IoC;
    using Logging;
    using MVVM;
    using Windows.Controls.MVVMProviders.Logic;
    using Windows.Data;

    /// <summary>
    /// <see cref="PhoneApplicationPage"/> class that supports MVVM with Catel.
    /// </summary>
    /// <remarks>
    /// This control can resolve a view model in the following ways:<para />
    /// <list type="numbered">
    ///   <item>
    ///     <description>By using the <see cref="GetViewModelType()"/> method.</description>
    ///   </item>
    ///   <item>
    ///     <description>By using the <see cref="IViewModelLocator"/> which is registered in the <see cref="IServiceLocator"/>.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">The view model of the view could not be resolved. Use either the <see cref="GetViewModelType()"/> method or <see cref="IViewModelLocator"/>.</exception>
    public class PhoneApplicationPage : Microsoft.Phone.Controls.PhoneApplicationPage, IPhoneApplicationPage
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IViewModelLocator _viewModelLocator;

        private readonly PhoneApplicationPageLogic _logic;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="PhoneApplicationPage"/> class.
        /// </summary>
        static PhoneApplicationPage()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            _viewModelLocator = dependencyResolver.Resolve<IViewModelLocator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneApplicationPage"/> class.
        /// </summary>
        /// <remarks>
        /// It is not possible to inject view models (actually, you can't even
        /// pass view models during navigation in Windows Phone 7).
        /// </remarks>
        public PhoneApplicationPage()
        {
            if (Catel.Environment.IsInDesignMode)
            {
                return;
            }

            var viewModelType = GetViewModelType();
            if (viewModelType == null)
            {
                Log.Debug("GetViewModelType() returned null, using the ViewModelLocator to resolve the view model");

                viewModelType = _viewModelLocator.ResolveViewModel(GetType());
                if (viewModelType == null)
                {
                    const string error = "The view model of the view could not be resolved. Use either the GetViewModelType() method or IViewModelLocator";
                    Log.Error(error);
                    throw new NotSupportedException(error);
                }
            }

            _logic = new PhoneApplicationPageLogic(this, viewModelType);
            _logic.TargetControlPropertyChanged += (sender, e) =>
            {
                OnPropertyChanged(e.FxEventArgs);

                PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs(e.PropertyName));
            };

            _logic.ViewModelChanged += (sender, e) =>
            {
                OnViewModelChanged();

                ViewModelChanged.SafeInvoke(this, e);
                PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs("ViewModel"));
            };

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);

                ViewModelPropertyChanged.SafeInvoke(this, e);
            };

            _logic.DetermineViewModelInstance += (sender, e) =>
            {
                e.ViewModel = GetViewModelInstance(e.DataContext);
            };

            _logic.DetermineViewModelType += (sender, e) =>
            {
                e.ViewModelType = GetViewModelType(e.DataContext);
            };

            _logic.Tombstoning += (sender, e) => OnTombstoning();
            _logic.Tombstoned += (sender, e) => OnTombstoned();
            _logic.RecoveringFromTombstoning += (sender, e) => OnRecoveringFromTombstoning();
            _logic.RecoveredFromTombstoning += (sender, e) => OnRecoveredFromTombstoning();

            ViewModelChanged.SafeInvoke(this);

            Loaded += (sender, e) => OnLoaded(e);
            Unloaded += (sender, e) => OnUnloaded(e);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<PhoneApplicationPageLogic, Type>(x => x.ViewModelType); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel
        {
            get { return _logic.GetValue<PhoneApplicationPageLogic, IViewModel>(x => x.ViewModel); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether navigating away from the page should save the view model.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if navigating away should save the view model; otherwise, <c>false</c>.
        /// </value>
        public bool NavigatingAwaySavesViewModel
        {
            get { return _logic.GetValue<PhoneApplicationPageLogic, bool>(x => x.NavigatingAwaySavesViewModel, true); }
            set { _logic.SetValue<PhoneApplicationPageLogic>(x => x.NavigatingAwaySavesViewModel = value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the back key cancels the view model. This
        /// means that <see cref="IViewModel.CancelViewModel"/> will be called when the back key is pressed.
        /// <para/>
        /// If this property is <c>false</c>, the <see cref="IViewModel.SaveViewModel"/> will be called instead.
        /// <para/>
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the back key cancels the view model; otherwise, <c>false</c>.
        /// </value>
        public bool BackKeyCancelsViewModel
        {
            get { return _logic.GetValue<PhoneApplicationPageLogic, bool>(x => x.BackKeyCancelsViewModel, true); }
            set { _logic.SetValue<PhoneApplicationPageLogic>(x => x.BackKeyCancelsViewModel = value); }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a property on the container has changed.
        /// </summary>
        /// <remarks>
        /// This event makes it possible to externally subscribe to property changes of a <see cref="DependencyObject"/>
        /// (mostly the container of a view model) because the .NET Framework does not allows us to.
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> property has changed.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        /// Occurs when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the type of the view model. If this method returns <c>null</c>, the view model type will be retrieved by naming 
        /// convention using the <see cref="IViewModelLocator"/> registered in the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        protected virtual Type GetViewModelType()
        {
            return null;
        }

        /// <summary>
        /// Gets the type of the view model at runtime based on the <see cref="FrameworkElement.DataContext"/>. If this method returns 
        /// <c>null</c>, the earlier determined view model type will be used instead.
        /// </summary>
        /// <param name="dataContext">The data context. This value can be <c>null</c>.</param>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        /// <remarks>
        /// Note that this method is only called when the <see cref="FrameworkElement.DataContext"/> changes.
        /// </remarks>
        protected virtual Type GetViewModelType(object dataContext)
        {
            return null;
        }

        /// <summary>
        /// Gets the instance of the view model at runtime based on the <see cref="FrameworkElement.DataContext"/>. If this method returns 
        /// <c>null</c>, the logic will try to construct the view model by itself.
        /// </summary>
        /// <param name="dataContext">The data context. This value can be <c>null</c>.</param>
        /// <returns>The instance of the view model or <c>null</c> in case it should be auto created.</returns>
        /// <remarks>
        /// Note that this method is only called when the <see cref="FrameworkElement.DataContext"/> changes.
        /// </remarks>
        protected virtual IViewModel GetViewModelInstance(object dataContext)
        {
            return null;
        }

        /// <summary>
        /// Called when the page is loaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoaded(EventArgs e)
        {
        }

        /// <summary>
        /// Called when the page is unloaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnloaded(EventArgs e)
        {
        }

        /// <summary>
        /// Called when a dependency property on this control has changed.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyValueChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(DependencyPropertyValueChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when a property on the current <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <remarks>
        /// This method does not implement any logic and saves a developer from subscribing/unsubscribing
        /// to the <see cref="ViewModelChanged"/> event inside the same user control.
        /// </remarks>
        protected virtual void OnViewModelChanged()
        {
        }

        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected bool ValidateData()
        {
            return _logic.ValidateViewModel();
        }

        /// <summary>
        /// Discards all changes made by this window.
        /// </summary>
        protected void DiscardChanges()
        {
            _logic.CancelViewModel();
        }

        /// <summary>
        /// Applies all changes made by this window.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected bool ApplyChanges()
        {
            return _logic.SaveViewModel();
        }

        /// <summary>
        /// Called when the view is about to tombstone itself.
        /// </summary>
        /// <remarks>
        /// Note that it is not required to call the base.
        /// </remarks>
        protected virtual void OnTombstoning()
        {
        }

        /// <summary>
        /// Called when the view has just tombstoned itself.
        /// </summary>
        /// <remarks>
        /// Note that it is not required to call the base.
        /// </remarks>
        protected virtual void OnTombstoned()
        {
        }

        /// <summary>
        /// Called when the view is about to recover itself from a tombstoned state.
        /// </summary>
        /// <remarks>
        /// Note that it is not required to call the base.
        /// </remarks>
        protected virtual void OnRecoveringFromTombstoning()
        {   
        }

        /// <summary>
        /// Called when the view has just recovered itself from a tombstoned state.
        /// </summary>
        /// <remarks>
        /// Note that it is not required to call the base.
        /// </remarks>
        protected virtual void OnRecoveredFromTombstoning()
        {
        }
        #endregion
    }
}