// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.MVVM.Providers;
    using Catel.MVVM.Views;
    using Catel.Threading;

    /// <summary>
    ///     The content page.
    /// </summary>
    public class ContentPage : global::Xamarin.Forms.ContentPage, IView
    {
        /// <summary>
        ///     The view mananger.
        /// </summary>
        // private readonly IViewManager _viewManager;
        private readonly UserControlLogic _userControlLogic;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentPage" /> class.
        /// </summary>
        public ContentPage() : this(null)
        {
            BindingContextChanged += OnBindingContextChanged;
        }

        private object _oldbindingContext;

        private void OnBindingContextChanged(object o, EventArgs eventArgs)
        {
            DataContextChanged.SafeInvoke(this, () => new DataContextChangedEventArgs(_oldbindingContext, BindingContext));
            _oldbindingContext = BindingContext;
            RaiseViewModelChanged();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentPage" /> class.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        public ContentPage(IViewModel viewModel)
        {
            _userControlLogic = new UserControlLogic(this, null, viewModel);

            _userControlLogic.TargetViewPropertyChanged += (sender, e) =>
            {
                OnPropertyChanged(e.PropertyName);
            };

            _userControlLogic.ViewModelClosedAsync += OnViewModelClosedAsync;

            _userControlLogic.ViewModelChanged += (sender, args) =>
            {
                if (BindingContext != _userControlLogic.ViewModel)
                {
                    BindingContext = _userControlLogic.ViewModel;
                }
            };

            _userControlLogic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);
                ViewModelPropertyChanged.SafeInvoke(this, e);
            };

            Loaded += (sender, e) =>
            {
                // _viewLoaded.SafeInvoke(this);
                // OnLoaded(e);
            };

            Unloaded += (sender, e) =>
            {
                // _viewUnloaded.SafeInvoke(this);
                // OnUnloaded(e);
            };

            DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        ///     Gets the view model.
        /// </summary>
        public IViewModel ViewModel => DataContext as IViewModel;

        /// <summary>
        ///     Occurs when the view model has changed.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        /// Occurs when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        /// <value>
        ///     The data context.
        /// </value>
        public object DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag.
        /// </value>
        public object Tag { get; set; }

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
        /// 
        /// </summary>
        /// <param name="propertyChangedEventArgs"></param>
        private void OnViewModelPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private void RaiseViewModelChanged()
        {
            OnViewModelChanged();
            ViewModelChanged.SafeInvoke(this);
        }

        /// <summary>
        ///     Called when the <see cref="ViewModel" /> has changed.
        /// </summary>
        /// <remarks>
        ///     This method does not implement any logic and saves a developer from subscribing/unsubscribing
        ///     to the <see cref="ViewModelChanged" /> event inside the same user control.
        /// </remarks>
        protected virtual void OnViewModelChanged()
        {
        }

        /// <summary>
        ///     Called when the <see cref="ViewModel" /> has been closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected virtual Task OnViewModelClosedAsync(object sender, ViewModelClosedEventArgs e)
        {
            return TaskHelper.Completed;
        }

        /// <summary>
        ///     Occurs when the back button is pressed.
        /// </summary>
        public event EventHandler<EventArgs> BackButtonPressed;


        /// <summary>
        ///     Occurs immediately prior to the <see cref="T:Xamarin.Forms.Page" /> becoming visible.
        /// </summary>
        protected override void OnAppearing()
        {
            Loaded.SafeInvoke(this, EventArgs.Empty);
            base.OnAppearing();
        }

        /// <summary>
        ///     Occurs when the <see cref="T:Xamarin.Forms.Page" /> disappears.
        /// </summary>
        protected sealed override void OnDisappearing()
        {
            base.OnDisappearing();
            Unloaded.SafeInvoke(this, EventArgs.Empty);
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
        protected sealed override bool OnBackButtonPressed()
        {
            /*BackButtonPressed.SafeInvoke(this);

            var popupLayout = Content as PopupLayout;
            //// TODO: Lookup for top most popup layout.
            return popupLayout != null && popupLayout.IsPopupActive || base.OnBackButtonPressed();*/
            return base.OnBackButtonPressed();
        }

        /// <summary>
        ///     Occurs when the data context has changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="eventArgs">The data context changed event args.</param>
        private void OnDataContextChanged(object sender, DataContextChangedEventArgs eventArgs)
        {
            ViewModelChanged.SafeInvoke(this);
        }
    }
}