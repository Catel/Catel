// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Controls
{
    using System;

    using Catel.IoC;
    using Catel.MVVM;
    using Catel.MVVM.Views;

    /// <summary>
    /// The content page.
    /// </summary>
    public class ContentPage : global::Xamarin.Forms.ContentPage, IView
    {
        /// <summary>
        /// The view mananger.
        /// </summary>
        private IViewManager _viewManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPage"/> class.
        /// </summary>
        public ContentPage()
        {
            _viewManager = this.GetDependencyResolver().Resolve<IViewManager>();
            DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public IViewModel ViewModel => DataContext as IViewModel;

        /// <summary>
        /// Occurs when the view model has changed.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        public object DataContext
        {
            get { return this.BindingContext; }
            set { this.BindingContext = value; }
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
                    base.BindingContext = value;
                    DataContextChanged.SafeInvoke(this, new DataContextChangedEventArgs(oldContext, BindingContext));
                }
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public object Tag { get; set; }

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> Unloaded;

        /// <summary>
        /// Occurs when the back button is pressed.
        /// </summary>
        public event EventHandler<EventArgs> BackButtonPressed;

        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        public event EventHandler<DataContextChangedEventArgs> DataContextChanged;

        /// <summary>
        /// Occurs immediately prior to the <see cref="T:Xamarin.Forms.Page"/> becoming visible.
        /// </summary>
        protected sealed override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.InitializeViewModelAsync();
        }

        /// <summary>
        /// Occurs when the <see cref="T:Xamarin.Forms.Page"/> disappears.
        /// </summary>
        protected sealed override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel?.CloseViewModelAsync(true);
        }

        /// <summary>
        ///     Occurs when the back button is pressed.
        /// </summary>
        /// <returns>
        ///     To be added.
        /// </returns>
        /// <remarks>
        ///     TODO: This implementation requires improvements.
        /// </remarks>
        protected override sealed bool OnBackButtonPressed()
        {
            BackButtonPressed.SafeInvoke(this);
            var popupLayout = Content as PopupLayout;
            //// TODO: Lookup for top most popup layout.
            return (popupLayout != null && popupLayout.IsPopupActive) || base.OnBackButtonPressed();
        }

        /// <summary>
        ///     Occurs when the data context has changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="eventArgs">The data context changed event args.</param>
        private void OnDataContextChanged(object sender, DataContextChangedEventArgs eventArgs)
        {
            _viewManager.RegisterView(this);
            ViewModelChanged.SafeInvoke(this);
        }
    }
}