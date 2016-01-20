// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------

using Catel.MVVM.Views;

#if XAMARIN_FORMS

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using Catel.IoC;
    using Catel.MVVM;

    using Application = global::Xamarin.Forms.Application;
    using Page = global::Xamarin.Forms.Page;

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService
    {
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
            var dependencyResolver = this.GetDependencyResolver();
            var viewLocator = dependencyResolver.Resolve<IViewLocator>();

            var navigationTarget = viewLocator.ResolveView(viewModelType).AssemblyQualifiedName;
            return navigationTarget;
        }

        /// <summary>
        /// Gets the back stack count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public override int GetBackStackCount()
        {
            var backStackCount = 0;
            var currentPage = Application.Current.CurrentPage();
            if (currentPage != null)
            {
                backStackCount = currentPage.Navigation.ModalStack.Count - 1;
            }

            return backStackCount;
        }

        /// <summary>
        /// Removes the back entry.
        /// </summary>
        public override void RemoveBackEntry()
        {
            if (CanGoBack)
            {
                var currentPage = Application.Current.CurrentPage();
                var page = currentPage.Navigation.ModalStack[currentPage.Navigation.ModalStack.Count - 1];
                currentPage.Navigation.RemovePage(page);
            }
        }

        /// <summary>
        /// Removes all back entries.
        /// </summary>
        public override void RemoveAllBackEntries()
        {
            throw new MustBeImplementedException();
        }

        partial void Initialize()
        {
        }

        partial void CloseMainWindow()
        {
            throw new MustBeImplementedException();
        }

        async partial void NavigateBack()
        {
            var currentPage = Application.Current.CurrentPage();
            if (currentPage != null)
            {
                await currentPage.Navigation.PopModalAsync();
            }
        }

        partial void NavigateForward()
        {
            throw new MustBeImplementedException();
        }

        async partial void NavigateWithParameters(string uri, Dictionary<string, object> parameters)
        {
            var dependencyResolver = this.GetDependencyResolver();

            var viewType = Type.GetType(uri);
            var viewModelLocator = dependencyResolver.Resolve<IViewModelLocator>();
            var viewModelType = viewModelLocator.ResolveViewModel(viewType);
            var typeFactory = dependencyResolver.Resolve<ITypeFactory>();
            var view = (Page)typeFactory.CreateInstance(viewType);
            var viewModelFactory = dependencyResolver.Resolve<IViewModelFactory>();
            var viewModel = viewModelFactory.CreateViewModel(viewModelType, null);
            if (view is IView)
            {
                (view as IView).DataContext = viewModel;
            }
            else
            {
                view.BindingContext = viewModel;
            }

            var currentPage = Application.Current.CurrentPage();
            if (currentPage != null)
            {
                await currentPage.Navigation.PushModalAsync(view);
            }
        }

        partial void NavigateToUri(Uri uri)
        {
            throw new MustBeImplementedException();
        }
        #endregion
    }
}

#endif