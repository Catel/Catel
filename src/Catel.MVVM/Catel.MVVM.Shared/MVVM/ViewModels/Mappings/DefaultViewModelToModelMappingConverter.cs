// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultViewModelToModelMappingConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using Reflection;

    /// <summary>
    /// ViewModelToModelMapping Copy Converter
    /// </summary>
    public class DefaultViewModelToModelMappingConverter : ViewModelToModelConverterBase
    {
        #region Constructors
        /// <summary>
        /// Creates an instance of converter
        /// </summary>
        /// <param name="propertyNames">All properties to watch</param>
        public DefaultViewModelToModelMappingConverter(string[] propertyNames)
            : base(propertyNames)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether this instance can convert the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="outType">Type of the out.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns><c>true</c> if this instance can convert the specified types; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
        {
            return types.Length == 1 && outType.IsAssignableFromEx(types[0]);
        }

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns>System.Object.</returns>
        public override object Convert(object[] values, IViewModel viewModel)
        {
            return values[0];
        }

        /// <summary>
        /// Determines whether this instance can convert back the specified in type.
        /// </summary>
        /// <param name="inType">Type of the in.</param>
        /// <param name="outTypes">The out types.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns><c>true</c> if this instance can convert back the specified in type; otherwise, <c>false</c>.</returns>
        public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
        {
            return outTypes.Length == 1 && outTypes[0].IsAssignableFromEx(inType);
        }

        /// <summary>
        /// Converts the specified values back.
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns>System.Object[].</returns>
        public override object[] ConvertBack(object value, IViewModel viewModel)
        {
            return new[] {value};
        }
        #endregion
    }
}