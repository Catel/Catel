// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Reflection;
    using global::Android.App;
    using global::Android.Content;
    using global::Android.OS;
    using global::Android.Views;
    using Java.Lang;

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
            get { throw new MustBeImplementedException(); }
        }

        /// <summary>
        /// Gets the can go forward.
        /// </summary>
        /// <value>The can go forward.</value>
        public override bool CanGoForward
        {
            get { throw new MustBeImplementedException(); }
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
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Removes the back entry.
        /// </summary>
        public override void RemoveBackEntry()
        {
            //throw new MustBeImplementedException();
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
            // no implementation needed
        }

        partial void NavigateBack()
        {
            throw new MustBeImplementedException();
        }

        partial void NavigateForward()
        {
            throw new MustBeImplementedException();
        }

        partial void NavigateWithParameters(string uri, Dictionary<string, object> parameters)
        {
            var context = Catel.Android.ContextHelper.CurrentContext;

            var navigationTargetType = TypeCache.GetType(uri);
            var intent = new Intent(context, navigationTargetType);
            foreach (var parameter in parameters)
            {
                intent.Extras.PutString(parameter.Key, parameter.Value.ToString());
            }

            // Required because we are using applicatio context
            intent.SetFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
        }

        partial void NavigateToUri(Uri uri)
        {
            NavigateWithParameters(uri.ToString(), new Dictionary<string, object>());
        }
        #endregion
    }
}

#endif