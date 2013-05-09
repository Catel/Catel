// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToViewConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data.Converters
{
    using System;
    using System.Windows.Data;
    using IoC;
    using MVVM;
    using Environment = Catel.Environment;

    /// <summary>
    /// Converts a view model to a view. This converter is very useful to dynamically load 
    /// view content.
    /// </summary>
#if NET
    [ValueConversion(typeof(object), typeof(object))]
#endif
    public class ViewModelToViewConverter : ValueConverterBase
    {
        private static readonly IViewLocator _viewLocator = ServiceLocator.Default.ResolveType<IViewLocator>();

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (Environment.IsInDesignMode || (value == null))
            {
                return ConverterHelper.DoNothingBindingValue;
            }

            var viewType = _viewLocator.ResolveView(value.GetType());
            return (viewType != null) ? ViewHelper.ConstructViewWithViewModel(viewType, value) : ConverterHelper.DoNothingBindingValue;
        }
    }
}