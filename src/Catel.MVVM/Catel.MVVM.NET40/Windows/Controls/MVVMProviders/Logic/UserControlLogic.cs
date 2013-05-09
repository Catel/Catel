// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserControlLogic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls.MVVMProviders.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Data;
    using Logging;
    using MVVM;
    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Data;

    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows.Controls;
    using System.Windows.Data;

    using UIEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// MVVM Provider behavior implementation for a user control.
    /// </summary>
    public class UserControlLogic : LogicBase
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The grid that is injected into every user control to use as data context container
        /// with the view model.
        /// </summary>
        private Grid _viewModelGrid;

        private IViewModelContainer _parentViewModelContainer;
        private IViewModel _parentViewModel;

#if NET || SL4 || SL5
        private InfoBarMessageControl _infoBarMessageControl;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="UserControlLogic" /> class.
        /// </summary>
        static UserControlLogic()
        {
#if NET || SL4 || SL5
            DefaultCreateWarningAndErrorValidatorForViewModelValue = true;
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControlLogic"/> class.
        /// </summary>
        /// <param name="targetControl">The target control.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetControl"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> does not implement interface <see cref="IViewModel"/>.</exception>
        public UserControlLogic(UserControl targetControl, Type viewModelType, IViewModel viewModel = null)
            : base(targetControl, viewModelType, viewModel)
        {
            SupportParentViewModelContainers = true;
            CloseViewModelOnUnloaded = true;

#if NET || SL4 || SL5
            SkipSearchingForInfoBarMessageControl = DefaultSkipSearchingForInfoBarMessageControlValue;
            CreateWarningAndErrorValidatorForViewModel = DefaultCreateWarningAndErrorValidatorForViewModelValue;
#endif

            if (targetControl.Content == null)
            {
                targetControl.SubscribeToDependencyProperty("Content", OnTargetControlContentChanged);
            }
            else
            {
                CreateViewModelGrid();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target control.
        /// </summary>
        /// <value>The target control.</value>
        public new UserControl TargetControl
        {
            get { return (UserControl)base.TargetControl; }
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
        public bool CloseViewModelOnUnloaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether parent view model containers are supported. If supported,
        /// the user control will search for a <see cref="DependencyObject"/> that implements the <see cref="IViewModelContainer"/>
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
        public bool SupportParentViewModelContainers { get; set; }

#if NET || SL4 || SL5
        /// <summary>
        /// Gets or sets a value indicating whether to skip the search for an info bar message control. If not skipped,
        /// the user control will search for a the first <see cref="InfoBarMessageControl"/> that can be found. 
        /// During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any <see cref="InfoBarMessageControl"/>, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>true</c>.
        /// <para />
        /// The default value is determined by the <see cref="DefaultSkipSearchingForInfoBarMessageControlValue"/> property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the search for an info bar message control should be skipped; otherwise, <c>false</c>.
        /// </value>
        public bool SkipSearchingForInfoBarMessageControl { get; set; }

        /// <summary>
        /// Gets or sets a value for the <see cref="SkipSearchingForInfoBarMessageControl"/> property. This way, the behavior
        /// can be changed an entire application to prevent disabling it on every control.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        public static bool DefaultSkipSearchingForInfoBarMessageControlValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create a <see cref="WarningAndErrorValidator"/> for the current
        /// control.
        /// <para />
        /// If a user control does not have any <see cref="InfoBarMessageControl"/> or equivalent control, it is useless to create
        /// a <see cref="WarningAndErrorValidator"/> for the current control.
        /// <para />
        /// The default value is determined by the <see cref="DefaultCreateWarningAndErrorValidatorForViewModelValue"/> property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="WarningAndErrorValidator"/> should be created; otherwise, <c>false</c>.
        /// </value>
        public bool CreateWarningAndErrorValidatorForViewModel { get; set; }

        /// <summary>
        /// Gets or sets a value for the <see cref="CreateWarningAndErrorValidatorForViewModel"/> property. This way, the behavior
        /// can be changed an entire application to prevent disabling it on every control.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        public static bool DefaultCreateWarningAndErrorValidatorForViewModelValue { get; set; }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether the user control should automatically be disabled when there is no
        /// active view model.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user control should automatically be disabled when there is no active view model; otherwise, <c>false</c>.
        /// </value>
        public bool DisableWhenNoViewModel { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a parent view model container available.
        /// </summary>
        /// <value>
        /// <c>true</c> if there is a parent view model container available; otherwise, <c>false</c>.
        /// </value>
        protected bool HasParentViewModelContainer
        {
            get { return _parentViewModelContainer != null; }
        }

        /// <summary>
        /// Gets the parent view model container.
        /// </summary>
        /// <value>The parent view model container.</value>
        /// <remarks>
        /// For internal usage only.
        /// </remarks>
        internal IViewModelContainer ParentViewModelContainer
        {
            get { return _parentViewModelContainer; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is subscribed to a parent view model.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is subscribed to a parent view model; otherwise, <c>false</c>.
        /// </value>
        protected bool IsSubscribedToParentViewModel
        {
            get { return (_parentViewModel != null); }
        }
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
            // not required, the VM grid automatically binds to the ViewModel property
        }

        /// <summary>
        /// Called when the content of the target control has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.Windows.Data.DependencyPropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void OnTargetControlContentChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            TargetControl.UnsubscribeFromDependencyProperty("Content", OnTargetControlContentChanged);

            CreateViewModelGrid();
        }

        /// <summary>
        /// Creates the grid that contains the view model as a data context.
        /// </summary>
        private void CreateViewModelGrid()
        {
            var content = TargetControl.Content as FrameworkElement;
            if (content == null)
            {
                return;
            }

            Log.Debug("Creating view model grid that will serve as view model container");

            _viewModelGrid = new Grid();
            _viewModelGrid.SetBinding(FrameworkElement.DataContextProperty, new Binding { Path = new PropertyPath("ViewModel"), Source = this });

#if NET || SL4 || SL5
            if (CreateWarningAndErrorValidatorForViewModel)
            {
                var warningAndErrorValidator = new WarningAndErrorValidator();
                warningAndErrorValidator.SetBinding(WarningAndErrorValidator.SourceProperty, new Binding());

                _viewModelGrid.Children.Add(warningAndErrorValidator);
            }
#endif

            TargetControl.Content = null;
            _viewModelGrid.Children.Add(content);
            TargetControl.Content = _viewModelGrid;

            Log.Debug("Created view model grid");
        }

        /// <summary>
        /// Called when the <see cref="TargetControl"/> has just been loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetControlLoaded(object sender, UIEventArgs e)
        {
            // Do not call base because it will create a VM. We will create the VM ourselves
            //base.OnTargetControlLoaded(sender, e);

#if NET || SL4 || SL5
            if (!SkipSearchingForInfoBarMessageControl)
            {
                Log.Debug("Searching for an instance of the InfoBarMessageControl");

                _infoBarMessageControl = FindParentByPredicate(TargetControl, o => o is InfoBarMessageControl) as InfoBarMessageControl;

                Log.Debug("Finished searching for an instance of the InfoBarMessageControl");

                if (_infoBarMessageControl == null)
                {
                    Log.Warning("No InfoBarMessageControl is found in the visual tree of '{0}', consider using the SkipSearchingForInfoBarMessageControl property to improve performance", GetType().Name);
                }
            }
            else
            {
                Log.Debug("Skipping the search for an instance of the InfoBarMessageControl");
            }
#endif

            if (!CloseViewModelOnUnloaded && (ViewModel != null))
            {
                // Re-use view model
                Log.Debug("Re-using existing view model");
            }

            if (ViewModel == null)
            {
                // Try to create view model based on data context
                ViewModel = CreateViewModelByUsingDataContextOrConstructor();
            }

            if (DisableWhenNoViewModel)
            {
                TargetControl.IsEnabled = (ViewModel != null);
            }
        }

        /// <summary>
        /// Called when the <see cref="TargetControl"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetControlUnloaded(object sender, UIEventArgs e)
        {
            base.OnTargetControlUnloaded(sender, e);

            if (ViewModel != null)
            {
                ClearWarningsAndErrorsForObject(ViewModel);
            }

            UnsubscribeFromParentViewModelContainer();

            if (CloseViewModelOnUnloaded)
            {
                CloseAndDiposeViewModel(true);
            }
            else
            {
                Log.Debug("Skipping 'CloseAndDisposeViewModel' because 'CloseViewModelOnUnloaded' is set to true");
            }
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.ViewModel"/> property is about to change.
        /// </summary>
        protected override void OnViewModelChanging()
        {
            UnregisterViewModelAsChild();
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.ViewModel"/> property has just been changed.
        /// </summary>
        protected override void OnViewModelChanged()
        {
            RegisterViewModelAsChild();

            if (DisableWhenNoViewModel)
            {
                TargetControl.IsEnabled = (ViewModel != null);
            }
        }

        /// <summary>
        /// Called when the <c>DataContext</c> property of the <see cref="TargetControl"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public override void OnTargetControlDataContextChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            // Fix in WinRT to make sure inner grid is created
            if (_viewModelGrid == null)
            {
                if (TargetControl.Content != null)
                {
                    TargetControl.UnsubscribeFromDependencyProperty("Content", OnTargetControlContentChanged);

                    CreateViewModelGrid();
                }
                else
                {
                    Log.Error("Content of control '{0}' is not yet loaded, but the DataContext has changed. The control will bind directly to the DataContext instead of the view model", TargetControl.GetType().FullName);
                }
            }

            base.OnTargetControlDataContextChanged(sender, e);

            var oldDataContext = e.OldValue;
            var dataContext = e.NewValue;

            if (BindingHelper.IsSentinelObject(dataContext))
            {
                return;
            }

            if (oldDataContext != null)
            {
                ClearWarningsAndErrorsForObject(oldDataContext);
            }

            UpdateDataContextToUseViewModel(dataContext);
        }

        /// <summary>
        /// Subscribes to the parent view model container.
        /// </summary>
        private void SubscribeToParentViewModelContainer()
        {
            if (!SupportParentViewModelContainers)
            {
                return;
            }

            if (HasParentViewModelContainer)
            {
                return;
            }

            _parentViewModelContainer = FindParentByPredicate(TargetControl, o => o is IViewModelContainer) as IViewModelContainer;
            if (_parentViewModelContainer != null)
            {
                Log.Debug("Found the parent view model container '{0}' for '{1}'", _parentViewModelContainer.GetType().Name, TargetControl.GetType().Name);
            }
            else
            {
                Log.Debug("Couldn't find parent view model container");
            }

            if (_parentViewModelContainer != null)
            {
                _parentViewModelContainer.ViewModelChanged += OnParentViewModelContainerViewModelChanged;

                SubscribeToParentViewModel(_parentViewModelContainer.ViewModel);
            }
        }

        /// <summary>
        /// Unsubscribes from the parent view model container.
        /// </summary>
        private void UnsubscribeFromParentViewModelContainer()
        {
            if (_parentViewModelContainer != null)
            {
                _parentViewModelContainer.ViewModelChanged -= OnParentViewModelContainerViewModelChanged;

                _parentViewModelContainer = null;
            }
        }

        /// <summary>
        /// Subscribes to a parent view model.
        /// </summary>
        /// <param name="parentViewModel">The parent view model.</param>
        private void SubscribeToParentViewModel(IViewModel parentViewModel)
        {
            if ((parentViewModel != null) && !ObjectHelper.AreEqualReferences(parentViewModel, ViewModel))
            {
                _parentViewModel = parentViewModel;

                RegisterViewModelAsChild();

                _parentViewModel.Saving += OnParentViewModelSaving;
                _parentViewModel.Canceling += OnParentViewModelCanceling;

                Log.Debug("Subscribed to parent view model '{0}'", parentViewModel.GetType());
            }
        }

        /// <summary>
        /// Unsubscribes from a parent view model.
        /// </summary>
        private void UnsubscribeFromParentViewModel()
        {
            if (_parentViewModel != null)
            {
                UnregisterViewModelAsChild();

                _parentViewModel.Saving -= OnParentViewModelSaving;
                _parentViewModel.Canceling -= OnParentViewModelCanceling;

                _parentViewModel = null;

                Log.Debug("Unsubscribed from parent view model");
            }
        }

        /// <summary>
        /// Registers the view model as child on the parent view model.
        /// </summary>
        private void RegisterViewModelAsChild()
        {
            var parentViewModel = _parentViewModel as IRelationalViewModel;
            var viewModel = ViewModel as IRelationalViewModel;

            if ((parentViewModel != null) && (viewModel != null) && !ReferenceEquals(parentViewModel, viewModel))
            {
                parentViewModel.RegisterChildViewModel(ViewModel);
                viewModel.SetParentViewModel(_parentViewModel);
            }
        }

        /// <summary>
        /// Unregisters the view model as child on the parent view model.
        /// </summary>
        private void UnregisterViewModelAsChild()
        {
            var parentViewModel = _parentViewModel as IRelationalViewModel;
            var viewModel = ViewModel as IRelationalViewModel;

            if ((parentViewModel != null) && (viewModel != null) && !ObjectHelper.AreEqualReferences(parentViewModel, viewModel))
            {
                viewModel.SetParentViewModel(null);
                parentViewModel.UnregisterChildViewModel(ViewModel);
            }
        }

        /// <summary>
        /// Updates the data context to use view model.
        /// </summary>
        /// <param name="newDataContext">The new data context.</param>
        private void UpdateDataContextToUseViewModel(object newDataContext)
        {
            SubscribeToParentViewModelContainer();

            if (ViewModelBehavior == LogicViewModelBehavior.Injected)
            {
                Log.Debug("View model behavior is set to 'Injected' thus view model will not be updated");
                return;
            }

            if (newDataContext != null)
            {
                var dataContextAsViewModel = newDataContext as IViewModel;
                if (dataContextAsViewModel != null)
                {
                    // If the DataContext is a view model, only create a new view model if required
                    if (ViewModel == null)
                    {
                        ViewModel = ConstructViewModelUsingArgumentOrDefaultConstructor(newDataContext);
                    }
                }
                else if (!(newDataContext.GetType().IsAssignableFromEx(ViewModelType)))
                {
                    if (ViewModel != null)
                    {
                        CancelViewModel();
                        CloseAndDiposeViewModel(false);
                    }

                    ViewModel = ConstructViewModelUsingArgumentOrDefaultConstructor(newDataContext);
                }
            }
            else
            {
                if (ViewModel != null)
                {
                    CancelViewModel();
                    CloseAndDiposeViewModel(false);

                    ViewModel = null;
                }

                // We closed our previous view-model, but it might be possible to construct a new view-model
                // with an empty constructor, so try that now
                ViewModel = ConstructViewModelUsingArgumentOrDefaultConstructor(null);
            }
        }

        /// <summary>
        /// Closes and diposes the current view model.
        /// </summary>
        private void CloseAndDiposeViewModel(bool? result)
        {
            if (ViewModel != null)
            {
                ViewModel.CloseViewModel(result);
                ViewModel = null;
            }
        }

        /// <summary>
        /// Handles the ViewModelChanged event of the parent ViewModel container.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnParentViewModelContainerViewModelChanged(object sender, EventArgs e)
        {
            UnsubscribeFromParentViewModel();

            IViewModelContainer viewModelContainer = null;

            var senderAsLogic = sender as LogicBase;
            if (senderAsLogic != null)
            {
                viewModelContainer = senderAsLogic.TargetControl as IViewModelContainer;
            }
            else
            {
                viewModelContainer = sender as IViewModelContainer;
            }

            if (viewModelContainer != null)
            {
                SubscribeToParentViewModel(viewModelContainer.ViewModel);
            }
        }

        /// <summary>
        /// Handles the Canceling event of the parent ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelingEventArgs"/> instance containing the event data.</param>
        private void OnParentViewModelCanceling(object sender, CancelingEventArgs e)
        {
            // The parent view model is canceled, cancel our view model as well
            if (ViewModel != null)
            {
                if (ReferenceEquals(sender, ViewModel))
                {
                    Log.Warning("Parent view model '{0}' is exactly the same instance as the current view model, ignore Canceling event", sender.GetType().FullName);
                    return;
                }

                if (e.Cancel)
                {
                    Log.Info("Parent view model '{0}' is canceling, but canceling is canceled by another view model, canceling of view model '{1}' will not continue", _parentViewModel.GetType(), ViewModel.GetType());
                    return;
                }

                Log.Info("Parent view model '{0}' is canceled, cancelling view model '{1}' as well", _parentViewModel.GetType(), ViewModel.GetType());

                if (!ViewModel.IsClosed)
                {
                    e.Cancel = !ViewModel.CancelViewModel();
                }
            }
        }

        /// <summary>
        /// Handles the Saving event of the parent ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SavingEventArgs"/> instance containing the event data.</param>
        private void OnParentViewModelSaving(object sender, SavingEventArgs e)
        {
            // The parent view model is saved, save our view model as well
            if (ViewModel != null)
            {
                if (ReferenceEquals(sender, ViewModel))
                {
                    Log.Warning("Parent view model '{0}' is exactly the same instance as the current view model, ignore Saving event", sender.GetType().FullName);
                    return;
                }

                if (e.Cancel)
                {
                    Log.Info("Parent view model '{0}' is saving, but saving is canceled by another view model, saving of view model '{1}' will not continue", _parentViewModel.GetType(), ViewModel.GetType());
                    return;
                }

                Log.Info("Parent view model '{0}' is saving, saving view model '{1}' as well", _parentViewModel.GetType(), ViewModel.GetType());

                if (!ViewModel.IsClosed)
                {
                    e.Cancel = !ViewModel.SaveViewModel();
                }
            }
        }

        /// <summary>
        /// Clears the warnings and errors for the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <remarks>
        /// Since there is a "bug" in the .NET Framework (DataContext issue), this method clears the current
        /// warnings and errors in the InfoBarMessageControl if available.
        /// </remarks>
        private void ClearWarningsAndErrorsForObject(object obj)
        {
            if (obj == null)
            {
                return;
            }

#if NET || SL4 || SL5
            if (_infoBarMessageControl != null)
            {
                _infoBarMessageControl.ClearObjectMessages(obj);

                Log.Debug("Cleared all warnings and errors caused by '{0}' since this is caused by a DataContext issue in the .NET Framework", obj);
            }
#endif
        }

        /// <summary>
        /// Gets the parent of the specified element, both for Silverlight and WPF.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The parent <see cref="FrameworkElement"/> or <c>null</c> if there is no parent.</returns>
        private static FrameworkElement GetParent(FrameworkElement element)
        {
#if NET
            return (element.Parent ?? element.TemplatedParent) as FrameworkElement;
#else
            return element.Parent as FrameworkElement;
#endif
        }

        /// <summary>
        /// Finds a parent by predicate. It first tries to find the parent via the <c>UserControl.Parent</c> property, and if that
        /// doesn't satisfy, it uses the <c>UserControl.TemplatedParent</c> property.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// 	<see cref="DependencyObject"/> or <c>null</c> if no parent is found that matches the predicate.
        /// </returns>
        private static DependencyObject FindParentByPredicate(UserControl control, Predicate<object> predicate)
        {
            return FindParentByPredicate(control, predicate, -1);
        }

        /// <summary>
        /// Finds a parent by predicate. It first tries to find the parent via the <c>UserControl.Parent</c> property, and if that
        /// doesn't satisfy, it uses the <c>UserControl.TemplatedParent</c> property.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>
        /// 	<see cref="DependencyObject"/> or <c>null</c> if no parent is found that matches the predicate.
        /// </returns>
        private static DependencyObject FindParentByPredicate(UserControl control, Predicate<object> predicate, int maxDepth)
        {
            Argument.IsNotNull("control", control);
            Argument.IsNotNull("predicate", predicate);

            object foundParent = null;

            var parents = new List<DependencyObject>();
            if (control.Parent != null)
            {
                parents.Add(control.Parent);
            }
#if NET
            if (control.TemplatedParent != null)
            {
                parents.Add(control.TemplatedParent);
            }
#endif
            foreach (DependencyObject parent in parents)
            {
                foundParent = parent.FindLogicalOrVisualAncestor(predicate, maxDepth);
                if (foundParent != null)
                {
                    break;
                }
            }

            return foundParent as DependencyObject;
        }
        #endregion
    }
}
