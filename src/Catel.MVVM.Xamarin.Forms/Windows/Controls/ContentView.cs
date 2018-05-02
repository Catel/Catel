// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable CS0067

namespace Catel.Windows.Controls
{
    using System;

    using Catel.IoC;
    using Catel.MVVM;
    using Catel.MVVM.Views;

    /// <summary>
    ///     The content page.
    /// </summary>
    public class ContentView : global::Xamarin.Forms.ContentView, IView
    {
        /// <summary>
        ///     The view mananger.
        /// </summary>
        private readonly IViewManager _viewManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentView" /> class.
        /// </summary>
        protected ContentView()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _viewManager = dependencyResolver.Resolve<IViewManager>();

            // TODO: Subscribe to Loaded / Unloaded from XF
            // TODO: Shouldn't this be BindingContextChanged?
            DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        ///     Gets or sets object that contains the properties that will be targeted by the bound properties that belong to this
        ///     <see cref="T:Xamarin.Forms.BindableObject" />.
        /// </summary>
        /// <value>
        ///     An <see cref="T:System.Object" /> that contains the properties that will be targeted by the bound properties that
        ///     belong to this <see cref="T:Xamarin.Forms.BindableObject" />. This is a bindable property.
        /// </value>
        /// <remarks>
        ///     <block subset="none" type="note">
        ///         Typically, the runtime performance is better if
        ///         <see cref="P:Xamarin.Forms.BindableObject.BindingContext" /> is set after all calls to
        ///         <see cref="M:Xamarin.Forms.BindableObject.SetBinding" /> have been made.
        ///     </block>
        ///     <para>
        ///         The following example shows how to apply a BindingContext and a Binding to a Label (inherits from
        ///         BindableObject):
        ///     </para>
        ///     <example>
        ///         <code lang="C#">
        /// <![CDATA[
        /// var label = new Label ();
        /// label.SetBinding (Label.TextProperty, "Name");
        /// label.BindingContext = new {Name = "John Doe", Company = "Xamarin"};
        /// Debug.WriteLine (label.Text); //prints "John Doe"
        ///         ]]>
        /// </code>
        ///     </example>
        /// </remarks>
        public new object BindingContext
        {
            get { return base.BindingContext; }

            set
            {
                if (!Equals(base.BindingContext, value))
                {
                    _viewManager.UnregisterView(this);
                    var oldContext = base.BindingContext;

                    RemoveParentChildRelationship();

                    base.BindingContext = value;
                    DataContextChanged.SafeInvoke(this, new DataContextChangedEventArgs(oldContext, BindingContext));
                }
            }
        }

        /// <summary>
        ///     Gets the view model.
        /// </summary>
        public IViewModel ViewModel
        {
            get { return DataContext as IViewModel; }
        }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        /// <value>
        ///     The data context.
        /// </value>
        public object DataContext
        {
            get { return BindingContext; }
            set { BindingContext = value; }
        }

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        ///     Occurs when the view model has changed.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        ///     Occurs when the view is loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        /// <summary>
        ///     Occurs when the view is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> Unloaded;

        /// <summary>
        ///     Occurs when the data context has changed.
        /// </summary>
        public event EventHandler<DataContextChangedEventArgs> DataContextChanged;

        

        /// <summary>
        /// Removes the parent-child relationship
        /// </summary>
        private void RemoveParentChildRelationship()
        {
            var parentViewModel = FindParentViewModel();
            if (parentViewModel != null)
            {
                (BindingContext as IRelationalViewModel)?.SetParentViewModel(null);
                (parentViewModel as IRelationalViewModel)?.UnregisterChildViewModel(base.BindingContext as IViewModel);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataContextChangedEventArgs"></param>
        private void OnDataContextChanged(object sender, DataContextChangedEventArgs dataContextChangedEventArgs)
        {
            _viewManager.RegisterView(this);

            EnsureParentChildRelationship(dataContextChangedEventArgs.NewContext);

            ViewModelChanged.SafeInvoke(this);
        }

        /// <summary>
        /// Ensures the parent-child relationship.
        /// </summary>
        /// <param name="dataContext"></param>
        private void EnsureParentChildRelationship(object dataContext)
        {
            var parentViewModel = FindParentViewModel() as IViewModel;
            if (parentViewModel != null)
            {
                // Note: note sure if the DataContext can be used, we should use a bit different technique to retrieve the
                // child view model
                var childViewModel = dataContext as IRelationalViewModel;
                if (childViewModel != null)
                {
                    childViewModel.SetParentViewModel(parentViewModel);
                }

                var parentRelationalViewModel = parentViewModel as IRelationalViewModel;
                if (parentRelationalViewModel != null)
                {
                    parentRelationalViewModel.RegisterChildViewModel(dataContext as IViewModel);
                }
            }
        }

        /// <summary>
        /// Finds the parent viewmodel.
        /// </summary>
        /// <returns>
        /// The parent viewmodel.
        /// </returns>
        private object FindParentViewModel()
        {
            var viewModel = BindingContext as IViewModel;
            var relationalViewModel = BindingContext as IRelationalViewModel;

            object parentViewModel = null;
            if (viewModel != null && relationalViewModel != null)
            {
                var parent = Parent;
                while (parentViewModel == null && parent != null)
                {
                    var parentViewModelAsRelationalViewModel = parent.BindingContext as IRelationalViewModel;
                    var parentViewModelAsViewModel = parent.BindingContext as IViewModel;
                    if (parentViewModelAsViewModel != null && parentViewModelAsRelationalViewModel != null && !ReferenceEquals(parent.BindingContext, viewModel))
                    {
                        parentViewModel = parentViewModelAsViewModel;
                    }
                    else
                    {
                        parent = parent.Parent;
                    }
                }
            }

            return parentViewModel;
        }
    }
}