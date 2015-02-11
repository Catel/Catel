// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Activity.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Android.App
{
    using System;
    using System.ComponentModel;
    using MVVM;
    using MVVM.Providers;
    using MVVM.Views;

    /// <summary>
    /// View implementation that automatically takes care of view models.
    /// </summary>
    public class Activity : global::Android.App.Activity, IPage
    {
        #region Fields
        private readonly PageLogic _logic;
        private object _dataContext;

        private BindingContext _bindingContext;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        public Activity()
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            _logic = new PageLogic(this);
            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
                OnPropertyChanged(e);

                PropertyChanged.SafeInvoke(this, e);
            };

            _logic.ViewModelChanged += (sender, e) => RaiseViewModelChanged();

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);

                ViewModelPropertyChanged.SafeInvoke(this, e);
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                var oldValue = _dataContext;
                var newValue = value;

                _dataContext = value;

                DataContextChanged.SafeInvoke(this, new DataContextChangedEventArgs(oldValue, newValue));
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<PageLogic, Type>(x => x.ViewModelType); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model container should prevent the 
        /// creation of a view model.
        /// <para />
        /// This property is very useful when using views in transitions where the view model is no longer required.
        /// </summary>
        /// <value><c>true</c> if the view model container should prevent view model creation; otherwise, <c>false</c>.</value>
        public bool PreventViewModelCreation
        {
            get { return _logic.GetValue<PageLogic, bool>(x => x.PreventViewModelCreation); }
            set { _logic.SetValue<PageLogic>(x => x.PreventViewModelCreation = value); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel
        {
            get { return _logic.GetValue<PageLogic, IViewModel>(x => x.ViewModel); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view is enabled.
        /// </summary>
        /// <value><c>true</c> if the view is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a property on the container has changed.
        /// </summary>
        /// <remarks>
        /// This event makes it possible to externally subscribe to property changes of a view
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

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> Unloaded;

        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        public event EventHandler<DataContextChangedEventArgs> DataContextChanged;
        #endregion

        #region Methods
        private void RaiseViewModelChanged()
        {
            OnViewModelChanged();

            ViewModelChanged.SafeInvoke(this);
            PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs("ViewModel"));

            if (_bindingContext != null)
            {
                _bindingContext.DetermineIfBindingsAreRequired(ViewModel);
            }
        }

        /// <summary>
        /// Called when the bindings must be added. This can happen
        /// <para />
        /// Normally the binding system would take care of this.
        /// </summary>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns><c>true</c> if the bindings were successfully added.</returns>
        protected virtual void AddBindings(BindingContext bindingContext, IViewModel viewModel)
        {
        }

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            RaiseViewModelChanged();

            Loaded.SafeInvoke(this);

            InitializeBindingContext();
        }

        /// <summary>
        /// Called as part of the activity lifecycle when an activity is going into
        ///  the background, but has not (yet) been killed.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();

            // Note: call *after* base so NavigationAdapter always gets called
            Unloaded.SafeInvoke(this);

            UninitializeBindingContext();
        }

        private void InitializeBindingContext()
        {
            if (_bindingContext != null)
            {
                UninitializeBindingContext();
            }

            _bindingContext = new BindingContext();
            _bindingContext.BindingUpdateRequired += OnBindingUpdateRequired;
            _bindingContext.DetermineIfBindingsAreRequired(ViewModel);
        }

        private void UninitializeBindingContext()
        {
            if (_bindingContext == null)
            {
                return;
            }

            _bindingContext.BindingUpdateRequired -= OnBindingUpdateRequired;
            _bindingContext.Clear();
            _bindingContext = null;
        }

        private void OnBindingUpdateRequired(object sender, EventArgs e)
        {
            AddBindings(_bindingContext, ViewModel);
        }

        /// <summary>
        /// Called when a dependency property on this control has changed.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
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
        #endregion
    }
}