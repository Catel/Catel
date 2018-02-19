// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------


#if XAMARIN_FORMS

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.IoC;
    using Catel.MVVM;
    using global::Xamarin.Forms;
    using Page = global::Xamarin.Forms.Page;

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService
    {
        /// <summary>
        /// The type factory
        /// </summary>
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        /// The view locator.
        /// </summary>
        private readonly IViewLocator _viewLocator;


        /// <summary>
        /// The view model factory.
        /// </summary>
        private readonly IViewModelFactory _viewModelFactory;

        /// <summary>
        /// The view model locator.
        /// </summary>
        private readonly IViewModelLocator _viewModelLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService" /> class.
        /// </summary>
        /// <param name="typeFactory">The type factory</param>
        /// <param name="viewLocator">The view locator</param>
        /// <param name="viewModelFactory">The viewmodel factory</param>
        public NavigationService(ITypeFactory typeFactory, IViewLocator viewLocator, IViewModelLocator viewModelLocator, IViewModelFactory viewModelFactory)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => viewLocator);
            Argument.IsNotNull(() => viewModelLocator);

            _typeFactory = typeFactory;
            _viewLocator = viewLocator;
            _viewModelFactory = viewModelFactory;
            _viewModelLocator = viewModelLocator;

            Initialize();
        }

        #region Properties
        /// <summary>
        /// Gets the can go back.
        /// </summary>
        /// <value>The can go back.</value>
        public override bool CanGoBack
        {
            get { return GetBackStackCount() > 0; }
        }

        /// <summary>
        /// Gets the can go forward.
        /// </summary>
        /// <value>The can go forward.</value>
        public override bool CanGoForward
        {
            get
        {
                throw new MustBeImplementedException();
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Resolves the navigation target.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The target to navigate to.</returns>
        protected override string ResolveNavigationTarget(Type viewModelType)
        {
            return _viewLocator.ResolveView(viewModelType).AssemblyQualifiedName;
        }

        /// <summary>
        /// Returns the number of total back entries (which is the navigation history).
        /// </summary>
        public override int GetBackStackCount()
        {
            var currentApplication = Application.Current;
            var activePage = currentApplication.GetActivePage();
            if(activePage == null)
            {
                return 0;
            }

            var navigation = activePage.Navigation;
            if (navigation == null)
            {
                return 0;
            }

            var navigationStack = navigation.NavigationStack;
            if (navigationStack == null)
            {
                return 0;
            }
            return navigationStack.Count;
        }


        /// <summary>
        /// Removes the back entry.
        /// </summary>
        public override void RemoveBackEntry()
        {
            if (CanGoBack)
            {
                var currentApplication = Application.Current;
                var activePage = currentApplication.GetActivePage();
                if(activePage == null)
                {
                    return;
                }

                var navigation = activePage.Navigation;
                if(navigation == null)
                {
                    return;
                }

                var navigationStack  = navigation.NavigationStack;
                if(navigationStack == null || navigationStack.Count == 0)
                {
                    return;
                }

                navigation.RemovePage(navigationStack.Last());
            }
        }

        /// <summary>
        /// Removes all back entries.
        /// </summary>
        public override void RemoveAllBackEntries()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Closes the main window
        /// </summary>
        partial void CloseMainWindow()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Navigates Back
        /// </summary>
#pragma warning disable AvoidAsyncVoid
        async partial void NavigateBack()
#pragma warning restore AvoidAsyncVoid
        {
            var currentApplication = Application.Current;
            var activePage = currentApplication.GetActivePage();
            if (activePage == null)
            {
                return;
            }

            var navigation = activePage.Navigation;
            if (navigation == null)
            {
                return;
            }

            await navigation.PopAsync();
        }

        /// <summary>
        /// Navigates forward
        /// </summary>
        partial void NavigateForward()
        {
            throw new MustBeImplementedException();
        }

#pragma warning disable AvoidAsyncVoid
        async partial void NavigateWithParameters(string uri, Dictionary<string, object> parameters)
#pragma warning restore AvoidAsyncVoid
        {
            var viewType = Type.GetType(uri);
            var viewModelType = _viewModelLocator.ResolveViewModel(viewType);
            var view = (Page)_typeFactory.CreateInstance(viewType);
            var viewModel = _viewModelFactory.CreateViewModel(viewModelType, parameters.Count > 0 ? parameters.Values.ToArray() : null);
            view.BindingContext = viewModel;
            
            var currentApplication = Application.Current;
            var activePage = currentApplication.GetActivePage();
            if (activePage == null)
            {
                return;
            }

            var navigation = activePage.Navigation;
            if (navigation == null)
            {
                return;
            }

            await navigation.PushAsync(view);
        }

        /// <summary>
        /// Navigates to an Uri.
        /// </summary>
        /// <param name="uri">The uri.</param>
        partial void NavigateToUri(Uri uri)
        {
            throw new MustBeImplementedException();
        }
        #endregion
    }
}

#endif