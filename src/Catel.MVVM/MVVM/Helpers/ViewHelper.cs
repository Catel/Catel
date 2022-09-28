// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.MVVM
{
    using System;
    using System.Windows;
    using Collections;
    using Logging;
    using Reflection;

#if UWP
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// View helper class for MVVM scenarios.
    /// </summary>
    public static class ViewHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructs the view with the view model. First, this method tries to inject the specified DataContext into the
        /// view. If the view does not contain a constructor with this parameter type, it will try to use the default constructor
        /// and set the DataContext manually.
        /// </summary>
        /// <typeparam name="T">The type of the view to return.</typeparam>
        /// <param name="viewType">Type of the view to instantiate.</param>
        /// <param name="dataContext">The data context to inject into the view. In most cases, this will be a view model.</param>
        /// <returns>
        /// The constructed view or <c>null</c> if it was not possible to construct the view.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType" /> is <c>null</c>.</exception>
        /// <remarks>
        /// Internally uses the <see cref="ConstructViewWithViewModel" /> method and casts the result.
        /// </remarks>
        public static T ConstructViewWithViewModel<T>(Type viewType, object dataContext)
            where T : FrameworkElement
        {
            return ConstructViewWithViewModel(viewType, dataContext) as T;
        }

        /// <summary>
        /// Constructs the view with the view model. First, this method tries to inject the specified DataContext into the
        /// view. If the view does not contain a constructor with this parameter type, it will try to use the default constructor
        /// and set the DataContext manually.
        /// </summary>
        /// <param name="viewType">Type of the view to instantiate.</param>
        /// <param name="dataContext">The data context to inject into the view. In most cases, this will be a view model.</param>
        /// <returns>
        /// The constructed view or <c>null</c> if it was not possible to construct the view.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType" /> is <c>null</c>.</exception>
        public static FrameworkElement ConstructViewWithViewModel(Type viewType, object dataContext)
        {
            Argument.IsNotNull("viewType", viewType);

            Log.Debug("Constructing view for view type '{0}'", viewType.Name);

            FrameworkElement view;

            // First, try to constructor directly with the data context
            if (dataContext is not null)
            {
                var injectionConstructor = viewType.GetConstructorEx(new[] { dataContext.GetType() });
                if (injectionConstructor is not null)
                {
                    view = (FrameworkElement)injectionConstructor.Invoke(new[] { dataContext });

                    Log.Debug("Constructed view using injection constructor");

                    return view;
                }
            }

            Log.Debug("No constructor with data (of type '{0}') injection found, trying default constructor", ObjectToStringHelper.ToTypeString(dataContext));

            // Try default constructor
            var defaultConstructor = viewType.GetConstructorEx(Array.Empty<Type>());
            if (defaultConstructor is null)
            {
                Log.Error("View '{0}' does not have an injection or default constructor thus cannot be constructed", viewType.Name);
                return null;
            }

            try
            {
                view = (FrameworkElement)defaultConstructor.Invoke(null);
            }
            catch (Exception ex)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>(ex, "Failed to construct view '{0}' with both injection and empty constructor", viewType.Name);
            }

            view.DataContext = dataContext;

            Log.Debug("Constructed view using default constructor and setting DataContext afterwards");

            return view;
        }
    }
}

#endif
