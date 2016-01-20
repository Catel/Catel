// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Xamarin.Forms;

namespace Catel.Windows.Controls
{
    using IoC;

    using MVVM;
    using MVVM.Views;

    using System;

    /// <summary>
    /// The content page.
    /// </summary>
    public class ContentView : global::Xamarin.Forms.ContentView, IView
    {
        /// <summary>
        /// The view mananger.
        /// </summary>
        private readonly IViewManager _viewManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentView"/> class.
        /// </summary>
        protected ContentView()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _viewManager = dependencyResolver.Resolve<IViewManager>();
            DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public IViewModel ViewModel
        {
            get { return DataContext as IViewModel; }
        }


        /// <summary>
        /// Gets or sets object that contains the properties that will be targeted by the bound properties that belong to this <see cref="T:Xamarin.Forms.BindableObject"/>.
        /// </summary>
        /// 
        /// <value>
        /// An <see cref="T:System.Object"/> that contains the properties that will be targeted by the bound properties that belong to this <see cref="T:Xamarin.Forms.BindableObject"/>. This is a bindable property.
        /// </value>
        /// 
        /// <remarks>
        /// <block subset="none" type="note">Typically, the runtime performance is better if  <see cref="P:Xamarin.Forms.BindableObject.BindingContext"/> is set after all calls to <see cref="M:Xamarin.Forms.BindableObject.SetBinding"/> have been made.</block>
        /// <para>
        /// The following example shows how to apply a BindingContext and a Binding to a Label (inherits from BindableObject):
        /// </para>
        /// 
        /// <example>
        /// 
        /// <code lang="C#">
        /// <![CDATA[
        /// var label = new Label ();
        /// label.SetBinding (Label.TextProperty, "Name");
        /// label.BindingContext = new {Name = "John Doe", Company = "Xamarin"};
        /// Debug.WriteLine (label.Text); //prints "John Doe"
        ///         ]]>
        /// </code>
        /// 
        /// </example>
        /// 
        /// </remarks>
        public new object BindingContext
        {
            get
            {
                return base.BindingContext;
            }

            set
            {
                if (!object.Equals(base.BindingContext, value))
                {
                    _viewManager.UnregisterView(this);
                    var oldContext = base.BindingContext;

                    var viewModel = base.BindingContext as IViewModel;
                    var relationalViewModel = base.BindingContext as IRelationalViewModel;
                    var parentViewModel = FindParentViewModel(this, viewModel, relationalViewModel);
                    if (parentViewModel != null)
                    {
                        relationalViewModel?.SetParentViewModel(null);
                        (parentViewModel as IRelationalViewModel)?.UnregisterChildViewModel(viewModel);
                    }

                    base.BindingContext = value;
                    DataContextChanged.SafeInvoke(this, new DataContextChangedEventArgs(oldContext, BindingContext));
                }
            }
        }


        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        public object DataContext
        {
            get { return BindingContext; }
            set { BindingContext = value; }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Occurs when the view model has changed.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

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

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataContextChangedEventArgs"></param>
        private void OnDataContextChanged(object sender, DataContextChangedEventArgs dataContextChangedEventArgs)
        {
            var view = sender as View;

            _viewManager.RegisterView(this);

            var viewModel = dataContextChangedEventArgs.NewContext as IViewModel;
            var relationalViewModel = dataContextChangedEventArgs.NewContext as IRelationalViewModel;
            var parentViewModel = FindParentViewModel(view, viewModel, relationalViewModel);
            if (parentViewModel != null)
            {
                relationalViewModel?.SetParentViewModel(parentViewModel as IViewModel);
                (parentViewModel as IRelationalViewModel)?.RegisterChildViewModel(viewModel);
            }

            ViewModelChanged.SafeInvoke(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="relationalViewModel"></param>
        /// <returns></returns>
        private static object FindParentViewModel(View view, IViewModel viewModel, IRelationalViewModel relationalViewModel)
        {
            object parentViewModel = null;
            if (view != null && viewModel != null && relationalViewModel != null)
            {
                Element parent = view.Parent;
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