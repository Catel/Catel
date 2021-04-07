// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToViewConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.MVVM.Converters
{
    using System;
    using IoC;
    using MVVM;

    /// <summary>
    /// Converts a view model to a view. This converter is very useful to dynamically load 
    /// view content.
    /// </summary>
#if NET || NETCORE
    [System.Windows.Data.ValueConversion(typeof(object), typeof(object))]
#endif
    public class ViewModelToViewConverter : ValueConverterBase
    {
        private static readonly IViewLocator _viewLocator;

        /// <summary>
        /// Initializes static members of the <see cref="ViewModelToViewConverter"/> class.
        /// </summary>
        static ViewModelToViewConverter()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            _viewLocator = dependencyResolver.Resolve<IViewLocator>();
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (CatelEnvironment.IsInDesignMode || (value is null))
            {
                return ConverterHelper.UnsetValue;
            }

            var viewType = _viewLocator.ResolveView(value.GetType());
            return (viewType is not null) ? ViewHelper.ConstructViewWithViewModel(viewType, value) : ConverterHelper.UnsetValue;
        }
    }
}

#endif
