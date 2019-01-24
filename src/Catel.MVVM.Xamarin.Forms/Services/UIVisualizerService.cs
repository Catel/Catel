// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIVisualizerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;

    using global::Xamarin.Forms;

    using IoC;
    using MVVM;
    using MVVM.Views;
    using Windows.Controls;
    
    using ContentPage = Windows.Controls.ContentPage;

    /// <summary>
    ///     Defines a UI controller which can be used to display dialogs in either modal or modaless form from a ViewModel
    /// </summary>
    public sealed class UIVisualizerService : IUIVisualizerService
    {
        /// <summary>
        /// The callbacks. 
        /// </summary>
        private readonly Dictionary<ContentPage, Tuple<IViewModel, EventHandler<UICompletedEventArgs>>> _callbacks = new Dictionary<ContentPage, Tuple<IViewModel, EventHandler<UICompletedEventArgs>>>();

        /// <summary>
        /// The language services. 
        /// </summary>
        private readonly ILanguageService _languageService;

        /// <summary>
        ///     The type factory.
        /// </summary>
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        ///     The view locator.
        /// </summary>
        private readonly IViewLocator _viewLocator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UIVisualizerService" /> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewLocator" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="languageService" /> is <c>null</c>.</exception>
        public UIVisualizerService(IViewLocator viewLocator, ITypeFactory typeFactory, ILanguageService languageService)
        {
            Argument.IsNotNull(() => viewLocator);
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => languageService);

            _viewLocator = viewLocator;
            _typeFactory = typeFactory;
            _languageService = languageService;
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

            bool? result = null;
            var viewModelType = viewModel.GetType();
            var resolvedView = _viewLocator.ResolveView(viewModelType);
            var view = (View) _typeFactory.CreateInstance(resolvedView);

            if (view is IView)
            {
                (view as IView).DataContext = viewModel;
            }
            else
            {
                view.BindingContext = viewModel;
            }

            var okButton = new Button
            {
                Text = _languageService.GetString("OK")
            };

            okButton.Clicked += (sender, args) =>
            {
                result = true;
                OnOkButtonClicked(viewModel, completedProc);
            };

            var cancelButton = new Button
            {
                Text = _languageService.GetString("Cancel")
            };

            cancelButton.Clicked += (sender, args) =>
            {
                result = false;
                OnCancelButtonClicked(viewModel, completedProc);
            };

            var dataErrorInfo = viewModel as INotifyDataErrorInfo;
            if (dataErrorInfo != null)
            {
                Action checkIfViewModelHasErrors = () => okButton.IsEnabled = !dataErrorInfo.HasErrors;
                viewModel.PropertyChanged += (sender, args) => checkIfViewModelHasErrors();
                checkIfViewModelHasErrors();
            }

            ////TODO: Improve the layout.
            var buttonsStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center
            };

            buttonsStackLayout.Children.Add(okButton);
            buttonsStackLayout.Children.Add(cancelButton);

            var contentLayout = new StackLayout();
            contentLayout.Children.Add(view);
            contentLayout.Children.Add(buttonsStackLayout);

            if (!await TryDisplayAsPopupAsync(viewModel, completedProc, contentLayout))
            {
                await DisplayUsingNavigationAsync(viewModel, completedProc, contentLayout);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="completedProc"></param>
        /// <param name="contentLayout"></param>
        /// <returns></returns>
        private async Task DisplayUsingNavigationAsync(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc, StackLayout contentLayout)
        {
            var contentPage = new ContentPage
            {
                Content = contentLayout
            };

            _callbacks[contentPage] = new Tuple<IViewModel, EventHandler<UICompletedEventArgs>>(viewModel, completedProc);
            contentPage.BackButtonPressed += OnBackButtonPressed;

            await NavigationHelper.PushModalAsync(contentPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="completedProc"></param>
        /// <param name="contentLayout"></param>
        /// <returns></returns>
        private async Task<bool> TryDisplayAsPopupAsync(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc, StackLayout contentLayout)
        {
            bool result = false;
            if (Application.Current.GetActivePage() is ContentPage currentPage)
            {
                _callbacks[currentPage] = new Tuple<IViewModel, EventHandler<UICompletedEventArgs>>(viewModel, completedProc);
                // TODO: Look for the top must popup layout inactive
                if (currentPage.Content is PopupLayout popupLayout && !popupLayout.IsPopupActive)
                {
                    currentPage.BackButtonPressed += OnBackButtonPressed;
                    contentLayout.HeightRequest = contentLayout.Children[0].HeightRequest + contentLayout.Children[1].HeightRequest;
                    contentLayout.WidthRequest = contentLayout.Children[0].WidthRequest;
                    result = true;
                    await popupLayout.ShowPopupAsync(contentLayout);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="completedProc"></param>
#pragma warning disable AvoidAsyncVoid // Avoid async void
        private async void OnOkButtonClicked(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            await viewModel.SaveAndCloseViewModelAsync();
            completedProc?.Invoke(this, new UICompletedEventArgs(viewModel, true));
            await CloseModalAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task CloseModalAsync()
        {
            if (Application.Current.GetActivePage() is ContentPage currentPage)
            {
                if (currentPage.Content is PopupLayout popupLayout && popupLayout.IsPopupActive)
                {
                    await popupLayout.DismissPopupAsync();
                }
                else
                {
                    await NavigationHelper.PopModalAsync();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="completedProc"></param>
#pragma warning disable AvoidAsyncVoid // Avoid async void
        private async void OnCancelButtonClicked(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            await viewModel.CancelAndCloseViewModelAsync();
            completedProc?.Invoke(this, new UICompletedEventArgs(viewModel, false));
            await CloseModalAsync();
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
            throw new NotSupportedInPlatformException();
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
            throw new NotSupportedInPlatformException();
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

#pragma warning disable AvoidAsyncVoid // Avoid async void
        private async void OnBackButtonPressed(object sender, EventArgs e)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            var contentPage = sender as ContentPage;
            if (contentPage != null)
            {
                var popupLayout = contentPage.Content as PopupLayout;
                if (popupLayout != null && popupLayout.IsPopupActive)
                {
                    _callbacks[contentPage]?.Item2?.Invoke(this, new UICompletedEventArgs(_callbacks[contentPage].Item1, null));
                    await popupLayout.DismissPopupAsync();
                }

                if (popupLayout is null)
                {
                    _callbacks[contentPage]?.Item2?.Invoke(this, new UICompletedEventArgs(_callbacks[contentPage].Item1, null));
                    await NavigationHelper.PopModalAsync(); 
                }
                
                contentPage.BackButtonPressed -= OnBackButtonPressed;
            }
        }
    }
}
