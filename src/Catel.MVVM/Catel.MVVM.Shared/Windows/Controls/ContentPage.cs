// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if XAMARIN_FORMS

namespace Catel.Windows.Controls
{
    using IoC;

    using MVVM;
    using MVVM.Views;

    using System;

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
        protected ContentPage()
        {
            _viewManager = this.GetDependencyResolver().Resolve<IViewManager>();

            BindingContextChanged += OnBindingContextChanged;
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
            get { return BindingContext; }

            set { BindingContext = value; }
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
            if (ViewModel != null)
            {
                return ViewModel.CancelAndCloseViewModelAsync().Result || base.OnBackButtonPressed();
            }

            return base.OnBackButtonPressed();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataContextChangedEventArgs"></param>
        private void OnDataContextChanged(object sender, DataContextChangedEventArgs dataContextChangedEventArgs)
        {
            _viewManager.RegisterView(this);
            ViewModelChanged.SafeInvoke(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnBindingContextChanged(object sender, EventArgs eventArgs)
        {
            DataContextChanged.SafeInvoke(this, new DataContextChangedEventArgs(null, BindingContext));
        }
    }
}

#endif