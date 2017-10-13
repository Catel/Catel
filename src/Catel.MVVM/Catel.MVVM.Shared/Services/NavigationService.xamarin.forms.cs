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
        #region Properties
        /// <summary>
        /// Gets the can go back.
        /// </summary>
        /// <value>The can go back.</value>
        public override bool CanGoBack
        {
            get { return this.GetBackStackCount() > 0; }
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

        public override int GetBackStackCount()
        {
            return Application.Current.GetActivePage().Navigation.NavigationStack.Count;
        }


        /// <summary>
        /// Removes the back entry.
        /// </summary>
        public override void RemoveBackEntry()
        {
            if (CanGoBack)
            {
                var navigation = Application.Current.GetActivePage().Navigation;
                navigation.RemovePage(navigation.NavigationStack.Last());
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
            await Application.Current.GetActivePage().Navigation.PopAsync();
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
            var viewModel = viewModelFactory.CreateViewModel(viewModelType, parameters.Count > 0 ? parameters.Values.ToArray() : null);
            view.BindingContext = viewModel;

            await Application.Current.GetActivePage().Navigation.PushAsync(view);
        }

        partial void NavigateToUri(Uri uri)
        {
            throw new MustBeImplementedException();
        }
        #endregion
    }
}

#endif