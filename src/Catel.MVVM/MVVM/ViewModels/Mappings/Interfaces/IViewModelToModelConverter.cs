namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Defines view model to model converter.
    /// </summary>
    public interface IViewModelToModelConverter
    {
        /// <summary>
        /// Determines whether the property name should be converted.
        /// </summary>
        /// <param name="propertyName">The name of changed property</param>
        /// <returns><c>true</c> if the property name should be converted, <c>false</c> otherwise.</returns>
        bool ShouldConvert(string? propertyName);

        /// <summary>
        /// Determines whether this instance can convert the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="outType">Type of the out.</param>
        /// <param name="viewModelType">Owner VM type</param>
        /// <returns><c>true</c> if this instance can convert the specified types; otherwise, <c>false</c>.</returns>
        bool CanConvert(Type[] types, Type outType, Type viewModelType);

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns>System.Object.</returns>
        object? Convert(object?[] values, IViewModel viewModel);

        /// <summary>
        /// Determines whether this instance can convert back the specified in type.
        /// </summary>
        /// <param name="inType">Type of the in.</param>
        /// <param name="outTypes">The out types.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns><c>true</c> if this instance can convert back the specified in type; otherwise, <c>false</c>.</returns>
        bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType);

        /// <summary>
        /// Converts the specified values back.
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns>System.Object[].</returns>
        object?[] ConvertBack(object? value, IViewModel viewModel);
    }
}
