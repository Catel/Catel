// --------------------------------------------------------------------------------------------------------------------
// <copyright file="View.cs" company="Catel development team">
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
    using global::Android.Runtime;

    /// <summary>
    /// Fragment implementation that automatically takes care of view models.
    /// </summary>
    public class Fragment : global::Android.App.Fragment, IUserControl
    {
        #region Fields
        private readonly UserControlLogic _logic;
        private object _dataContext;
        private object _tag;

        private BindingContext _bindingContext;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Fragment"/> class.
        /// </summary>
        /// <param name="javaReference">The java reference.</param>
        /// <param name="transfer">The transfer.</param>
        public Fragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fragment" /> class.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public Fragment()
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            _logic = new UserControlLogic(this, null);
            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
                OnPropertyChanged(e);

                PropertyChanged?.Invoke(this, e);
            };

            _logic.ViewModelChanged += (sender, e) => RaiseViewModelChanged();

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);

                ViewModelPropertyChanged?.Invoke(this, e);
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

                DataContextChanged?.Invoke(this, new DataContextChangedEventArgs(oldValue, newValue));
            }
        }

        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<UserControlLogic, Type>(x => x.ViewModelType); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model container should prevent the 
        /// creation of a view model.
        /// <para />
        /// This property is very useful when using views in transitions where the view model is no longer required.
        /// </summary>
        /// <value><c>true</c> if the view model container should prevent view model creation; otherwise, <c>false</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "ViewModelLifetimeManagement.FullyManual", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public bool PreventViewModelCreation
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.PreventViewModelCreation); }
            set { _logic.SetValue<UserControlLogic>(x => x.PreventViewModelCreation = value); }
        }

        /// <summary>
        /// Gets or sets a the view model lifetime management.
        /// <para />
        /// By default, this value is <see cref="ViewModelLifetimeManagement"/>.
        /// </summary>
        /// <value>
        /// The view model lifetime management.
        /// </value>
        public ViewModelLifetimeManagement ViewModelLifetimeManagement
        {
            get { return _logic.GetValue<UserControlLogic, ViewModelLifetimeManagement>(x => x.ViewModelLifetimeManagement); }
            set { _logic.SetValue<UserControlLogic>(x => x.ViewModelLifetimeManagement = value); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel
        {
            get { return _logic.GetValue<UserControlLogic, IViewModel>(x => x.ViewModel); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user control should close any existing
        /// view model when the control is unloaded from the visual tree.
        /// <para />
        /// Set this property to <c>false</c> if a view model should be kept alive and re-used
        /// for unloading/loading instead of creating a new one.
        /// <para />
        /// By default, this value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the view model should be closed when the control is unloaded; otherwise, <c>false</c>.
        /// </value>
        public bool CloseViewModelOnUnloaded
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.CloseViewModelOnUnloaded, true); }
            set { _logic.SetValue<UserControlLogic>(x => x.CloseViewModelOnUnloaded = value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether parent view model containers are supported. If supported,
        /// the user control will search for a <see cref="IView"/> that implements the <see cref="IViewModelContainer"/>
        /// interface. During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any parent control implementing the <see cref="IViewModelContainer"/> interface, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>false</c>.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if parent view model containers are supported; otherwise, <c>false</c>.
        /// </value>
        public bool SupportParentViewModelContainers
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.SupportParentViewModelContainers, true); }
            set { _logic.SetValue<UserControlLogic>(x => x.SupportParentViewModelContainers = value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user control should automatically be disabled when there is no
        /// active view model.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user control should automatically be disabled when there is no active view model; otherwise, <c>false</c>.
        /// </value>
        public bool DisableWhenNoViewModel
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.DisableWhenNoViewModel, false); }
            set { _logic.SetValue<UserControlLogic>(x => x.DisableWhenNoViewModel = value); }
        }

        /// <summary>
        /// Gets the parent of the view.
        /// </summary>
        /// <value>The parent.</value>
        public object Parent { get { return null; } }

        /// <summary>
        /// Gets the tag of the view.
        /// </summary>
        /// <value>The tag.</value>
        object IView.Tag
        {
            get { return _tag; }
            set { _tag = value; }
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

            ViewModelChanged?.Invoke(this);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));

            if (_bindingContext != null)
            {
                _bindingContext.DetermineIfBindingsAreRequired(ViewModel);
            }
        }

        /// <summary>
        /// Gets the view model as a type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The view model of <c>null</c>.</returns>
        protected TViewModel GetViewModel<TViewModel>()
            where TViewModel : class, IViewModel
        {
            return ViewModel as TViewModel;
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
        /// Called when the fragment is resumed.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();

            Loaded?.Invoke(this);

            InitializeBindingContext();
        }

        /// <summary>
        /// Called when the fragment is paused.
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();

            Unloaded?.Invoke(this);

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
            if (_bindingContext is null)
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
