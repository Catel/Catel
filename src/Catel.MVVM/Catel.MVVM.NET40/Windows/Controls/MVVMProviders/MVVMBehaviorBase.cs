// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MVVMBehaviorBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls.MVVMProviders
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Interactivity;
    using Data;
    using Logic;

    using Interactivity;
    using MVVM;

    /// <summary>
    /// A <see cref="Behavior"/> base implementation that takes care of generic MVVM behavior logic.
    /// </summary>
    /// <typeparam name="TAttachedType">The type of the attached type.</typeparam>
    /// <typeparam name="TLogicType">The type of the logic type.</typeparam>
    public abstract class MVVMBehaviorBase<TAttachedType, TLogicType> : BehaviorBase<TAttachedType>
        where TAttachedType : FrameworkElement
        where TLogicType : LogicBase
    {
        #region Fields
        /// <summary>
        /// The injected view model.
        /// </summary>
        protected readonly IViewModel InjectedViewModel;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MVVMBehaviorBase&lt;TAttachedType, TLogicType&gt;"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        protected MVVMBehaviorBase(IViewModel viewModel = null)
        {
            InjectedViewModel = viewModel;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the logic.
        /// </summary>
        /// <value>The logic.</value>
        protected TLogicType Logic { get; private set; }

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        protected new TAttachedType AssociatedObject { get { return base.AssociatedObject; } }

        /// <summary>
        /// Gets or sets the type of the view model.
        /// </summary>
        /// <value>The type of the view model.</value>
        /// <remarks>
        /// This is a dependency property because the <see cref="Logic"/> itself has no setter. Therefore, this
        /// can be set in xaml and will be used in derived classes to construct the logic.
        /// <para />
        /// Also, the logic is not constructed when the xaml of the behavior is parsed, so it has to be used later.
        /// </remarks>
        public Type ViewModelType
        {
            get { return (Type)GetValue(ViewModelTypeProperty); }
            set { SetValue(ViewModelTypeProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for ViewModelType.
        /// </summary>
        public static readonly DependencyProperty ViewModelTypeProperty = DependencyProperty.Register("ViewModelType",
            typeof(Type), typeof(MVVMBehaviorBase<TAttachedType, TLogicType>), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the type of the design time view model.
        /// </summary>
        /// <value>The type of the design time view model.</value>
        /// <remarks>
        /// This is a dependency property because the logic is not constructed when the xaml of the behavior is 
        /// parsed, so it has to be used later.
        /// </remarks>
        public Type DesignTimeViewModelType
        {
            get { return (Type)GetValue(DesignTimeViewModelTypeProperty); }
            set { SetValue(DesignTimeViewModelTypeProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for DesignTimeViewModelType.
        /// </summary>
        public static readonly DependencyProperty DesignTimeViewModelTypeProperty = DependencyProperty.Register("DesignTimeViewModelType", 
            typeof(Type), typeof(MVVMBehaviorBase<TAttachedType, TLogicType>), new PropertyMetadata(null));

        /// <summary>
        /// Gets the view model attached to this behavior.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel { get { return Logic.ViewModel; } }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view model is about to construct a new view model. This event can be used to
        /// intercept and inject a dynamically instantiated view model.
        /// </summary>
        public event EventHandler<DetermineViewModelInstanceEventArgs> DetermineViewModelInstance;

        /// <summary>
        /// Occurs when the view model is about to construct a new view model. This event can be used to
        /// intercept and inject a dynamically determined view model type.
        /// </summary>
        public event EventHandler<DetermineViewModelTypeEventArgs> DetermineViewModelType;

        /// <summary>
        /// Raised when the <see cref="ViewModel"/> is changed by the <see cref="Logic"/>.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        /// Raised when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;

        /// <summary>
        /// Occurs when a property on the target control has changed.
        /// </summary>
        public event EventHandler<DependencyPropertyValueChangedEventArgs> TargetControlPropertyChanged;

        /// <summary>
        /// Occurs when the view model container is loading.
        /// </summary>
        public event EventHandler<EventArgs> ViewLoading;

        /// <summary>
        /// Occurs when the view model container is loaded.
        /// </summary>
        public event EventHandler<EventArgs> ViewLoaded;

        /// <summary>
        /// Occurs when the view model container starts unloading.
        /// </summary>
        public event EventHandler<EventArgs> ViewUnloading;

        /// <summary>
        /// Occurs when the view model container is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> ViewUnloaded;

        /// <summary>
        /// Gets a value indicating whether the target control is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        public new bool IsAssociatedObjectLoaded { get { return Logic.IsTargetControlLoaded; } }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the logic required for MVVM.
        /// </summary>
        /// <returns>The <see cref="LogicBase"/> implementation uses by this behavior.</returns>
        protected abstract TLogicType CreateLogic();

        /// <summary>
        /// Validates the required properties. This implementation checks for the <see cref="ViewModelType"/>,
        /// which is mandatory.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="ViewModelType"/> is not set.</exception>
        /// <remarks>
        /// When this method is overriden, don't forget to call the base.
        /// </remarks>
        protected override void ValidateRequiredProperties()
        {
            if (ViewModelType == null)
            {
                throw new InvalidOperationException("The 'ViewModelType' must be set when using this behavior");
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <remarks>
        /// When this method is overriden, don't forget to call the base.
        /// </remarks>
        protected override void Initialize()
        {
            Logic = CreateLogic();

            if (Catel.Environment.IsInDesignMode)
            {
                return;
            }

            Logic.DetermineViewModelInstance += OnDetermineViewModelInstance;
            Logic.DetermineViewModelType += OnDetermineViewModelType;
            Logic.ViewModelChanged += OnLogicViewModelChanged;
            Logic.ViewModelPropertyChanged += OnLogicViewModelPropertyChanged;
            Logic.TargetControlPropertyChanged += OnLogicTargetControlPropertyChanged;

            Logic.ViewLoading += OnViewLoading;
            Logic.ViewLoaded += OnViewLoaded;
            Logic.ViewUnloading += OnViewUnloading;
            Logic.ViewUnloaded += OnViewUnloaded;
        }

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        protected override void Uninitialize()
        {
            Logic.DetermineViewModelInstance -= OnDetermineViewModelInstance;
            Logic.DetermineViewModelType -= OnDetermineViewModelType;
            Logic.ViewModelChanged -= OnLogicViewModelChanged;
            Logic.ViewModelPropertyChanged -= OnLogicViewModelPropertyChanged;
            Logic.TargetControlPropertyChanged -= OnLogicTargetControlPropertyChanged;

            Logic.ViewLoading -= OnViewLoading;
            Logic.ViewLoaded -= OnViewLoaded;
            Logic.ViewUnloading -= OnViewUnloading;
            Logic.ViewUnloaded -= OnViewUnloaded;

            ClearValue(ViewModelTypeProperty);
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.DetermineViewModelInstance"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.Windows.Controls.MVVMProviders.Logic.DetermineViewModelInstanceEventArgs"/> instance containing the event data.</param>
        private void OnDetermineViewModelInstance(object sender, DetermineViewModelInstanceEventArgs e)
        {
            DetermineViewModelInstance.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.DetermineViewModelType"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.Windows.Controls.MVVMProviders.Logic.DetermineViewModelTypeEventArgs"/> instance containing the event data.</param>
        private void OnDetermineViewModelType(object sender, DetermineViewModelTypeEventArgs e)
        {
            DetermineViewModelType.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.ViewModel"/> property has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLogicViewModelChanged(object sender, EventArgs e)
        {
            ViewModelChanged.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when a property on the <see cref="LogicBase.ViewModel"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLogicViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViewModelPropertyChanged.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when a property on the <see cref="LogicBase.TargetControl"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLogicTargetControlPropertyChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            TargetControlPropertyChanged.SafeInvoke(this, e);
        }

        private void OnViewLoading(object sender, EventArgs e)
        {
            ViewLoading.SafeInvoke(this);
        }

        private void OnViewLoaded(object sender, EventArgs e)
        {
            ViewLoaded.SafeInvoke(this);
        }

        private void OnViewUnloading(object sender, EventArgs e)
        {
            ViewUnloading.SafeInvoke(this);
        }

        private void OnViewUnloaded(object sender, EventArgs e)
        {
            ViewUnloaded.SafeInvoke(this);
        }
        #endregion
    }
}
