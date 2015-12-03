// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;

namespace Catel.MVVM.Views
{
    /// <summary>
    /// </summary>
    public class ContentPage : Xamarin.Forms.ContentPage, IView
    {
        protected ContentPage()
        {
            BindingContextChanged += OnBindingContextChanged;
            DataContextChanged += OnDataContextChanged;
        }

        public IViewModel ViewModel => DataContext as IViewModel;

        public event EventHandler<EventArgs> ViewModelChanged;

        public object DataContext
        {
            get { return BindingContext; }
            set { BindingContext = value; }
        }

        public object Tag { get; set; }

        public event EventHandler<EventArgs> Loaded;

        public event EventHandler<EventArgs> Unloaded;

        public event EventHandler<DataContextChangedEventArgs> DataContextChanged;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel?.OnDisappearing();
        }

        /// <summary>
        /// Application developers can override this method to provide behavior when the back button is pressed.
        /// </summary>
        /// <returns>
        /// To be added.
        /// </returns>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override bool OnBackButtonPressed()
        {
            if (ViewModel != null)
            {
                return ViewModel.OnBackButtonPressed() || base.OnBackButtonPressed();
            }

            return base.OnBackButtonPressed();
        }

        private void OnDataContextChanged(object sender, DataContextChangedEventArgs dataContextChangedEventArgs)
        {
            ViewModelChanged.SafeInvoke(this);
        }

        private void OnBindingContextChanged(object sender, EventArgs eventArgs)
        {
            DataContextChanged.SafeInvoke(this, new DataContextChangedEventArgs(null, BindingContext));
        }
    }
}