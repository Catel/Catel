// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIVisualizerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Catel.IoC;
using Catel.MVVM;
using Catel.Windows.Control;
using Xamarin.Forms;
using ContentPage = Catel.Windows.Controls.ContentPage;

namespace Catel.Services
{
    /// <summary>
    ///     Defines a UI controller which can be used to display dialogs in either modal or modaless form from a ViewModel
    /// </summary>
    public sealed class UIVisualizerService : IUIVisualizerService
    {
        /// <summary>
        ///     The type factory.
        /// </summary>
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        ///     The view locator.
        /// </summary>
        private readonly IViewLocator _viewLocator;

        private readonly Dictionary<ContentPage, EventHandler<UICompletedEventArgs>> _callbacks = new Dictionary<ContentPage, EventHandler<UICompletedEventArgs>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="UIVisualizerService" /> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewLocator" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public UIVisualizerService(IViewLocator viewLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => viewLocator);
            Argument.IsNotNull(() => typeFactory);

            _viewLocator = viewLocator;
            _typeFactory = typeFactory;
        }

        /// <summary>
        ///     Registers the specified view model and the window type. This way, Catel knowns what
        ///     window to show when a specific view model window is requested.
        /// </summary>
        /// <param name="name">Name of the registered window.</param>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="throwExceptionIfExists">
        ///     if set to <c>true</c>, this method will throw an exception when already
        ///     registered.
        /// </param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="windowType" /> is not of the right type.</exception>
        public void Register(string name, Type windowType, bool throwExceptionIfExists = true)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        ///     This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered window.</param>
        /// <returns>
        ///     <c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public bool Unregister(string name)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        ///     Determines whether the specified name is registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public bool IsRegistered(string name)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="viewModel" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public bool? Show(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            return ShowAsync(viewModel, completedProc).Result;
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="viewModel" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public async Task<bool?> ShowAsync(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNull(() => viewModel);

            PopupLayout[] popupLayout = {null};

            bool? result = null;
            var viewModelType = viewModel.GetType();
            var resolvedView = _viewLocator.ResolveView(viewModelType);
            var viewModelView = (View) _typeFactory.CreateInstance(resolvedView);
            viewModelView.BindingContext = viewModel;

            var okButton = new Button {Text = "OK"};
            okButton.Clicked += async (sender, args) =>
            {
                await viewModel.SaveAndCloseViewModel();
                result = true;
                completedProc?.SafeInvoke(this, new UICompletedEventArgs(viewModel, result));
                if (popupLayout[0] != null)
                {
                    await popupLayout[0].DismissPopup();
                }
                else
                {
                    await Application.Current.CurrentPage().Navigation.PopModalAsync();
                }
            };

            var dataErrorInfo = viewModel as INotifyDataErrorInfo;
            if (dataErrorInfo != null)
            {
                Action checkIfViewModelHasErrors = () => okButton.IsEnabled = !dataErrorInfo.HasErrors;
                viewModel.PropertyChanged += (sender, args) => checkIfViewModelHasErrors();
                checkIfViewModelHasErrors();
            }

            var cancelButton = new Button {Text = "Cancel"};
            cancelButton.Clicked += async (sender, args) =>
            {
                await viewModel.CancelAndCloseViewModel();
                result = false;
                // TODO: Review why the viewmodel have all changes even after be cancelled.
                completedProc?.SafeInvoke(this, new UICompletedEventArgs(viewModel, result));

                if (popupLayout[0] != null)
                {
                    await popupLayout[0].DismissPopup();
                }
                else
                {
                    await Application.Current.CurrentPage().Navigation.PopModalAsync();
                }
            };

            ////TODO: Improve the layout.
            var buttonsStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center
            };

            buttonsStackLayout.Children.Add(okButton);
            buttonsStackLayout.Children.Add(cancelButton);

            var contentLayout = new StackLayout();
            contentLayout.Children.Add(viewModelView);
            contentLayout.Children.Add(buttonsStackLayout);

            var currentPage = Application.Current.CurrentPage() as ContentPage;
            if (currentPage != null)
            {
                _callbacks[currentPage] = completedProc;
                currentPage.BackButtonPressed += OnBackButtonPressed;

                // TODO: Look for the top must popup layout inactive
                popupLayout[0] = currentPage.Content as PopupLayout;
                if (popupLayout[0] != null && !popupLayout[0].IsPopupActive)
                {
                    contentLayout.HeightRequest = viewModelView.HeightRequest + buttonsStackLayout.HeightRequest;
                    contentLayout.WidthRequest = viewModelView.WidthRequest;
                    await popupLayout[0].ShowPopup(contentLayout);
                }
            }

            if (popupLayout[0] == null)
            {
                var contentPage = new ContentPage
                {
                    Content = contentLayout
                };

                _callbacks[contentPage] = completedProc;
                contentPage.BackButtonPressed += OnBackButtonPressed;
                await Application.Current.CurrentPage().Navigation.PushModalAsync(contentPage);
            }

            return result;
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="name" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public bool? Show(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="name" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public Task<bool?> ShowAsync(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="viewModel" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public bool? ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            return ShowDialogAsync(viewModel, completedProc).Result;
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="viewModel" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public Task<bool?> ShowDialogAsync(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            // TODO: Create a floating dialog instead this?
            return ShowAsync(viewModel, completedProc);
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="name" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public bool? ShowDialog(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        ///     Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">
        ///     The callback procedure that will be invoked as soon as the window is closed. This value can
        ///     be <c>null</c>.
        /// </param>
        /// <returns>
        ///     Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">
        ///     The <paramref name="name" /> is not registered by the
        ///     <see cref="Register(string,System.Type,bool)" /> method first.
        /// </exception>
        public Task<bool?> ShowDialogAsync(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBackButtonPressed(object sender, EventArgs e)
        {
            var contentPage = sender as ContentPage;
            if (contentPage != null)
            {
                var popupLayout = contentPage.Content as PopupLayout;
                if (popupLayout != null && popupLayout.IsPopupActive)
                {
                    _callbacks[contentPage]?.SafeInvoke(this, new UICompletedEventArgs(contentPage.ViewModel, null));
                    await popupLayout.DismissPopup();
                }

                contentPage.BackButtonPressed -= OnBackButtonPressed;

                if (popupLayout == null)
                {
                    _callbacks[contentPage]?.SafeInvoke(this, new UICompletedEventArgs(contentPage.ViewModel, null));
                    await Application.Current.CurrentPage().Navigation.PopModalAsync();
                }
            }
        }
    }
}