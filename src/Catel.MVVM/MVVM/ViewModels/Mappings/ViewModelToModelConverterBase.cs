namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Base class for view model to model converters.
    /// </summary>
    public abstract class ViewModelToModelConverterBase : IViewModelToModelConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelToModelConverterBase"/> class.
        /// </summary>
        /// <param name="propertyNames">Name of property on which attribute was setted</param>
        protected ViewModelToModelConverterBase(string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <value>The property names.</value>
        public string[] PropertyNames { get; private set; }

        /// <summary>
        /// Determines whether the property name should be converted.
        /// </summary>
        /// <param name="propertyName">The name of changed property</param>
        /// <returns><c>true</c> if the property name should be converted, <c>false</c> otherwise.</returns>
        public bool ShouldConvert(string? propertyName)
        {
            if (propertyName is null)
            {
                return false;
            }

            foreach (var x in PropertyNames)
            {
                if (string.CompareOrdinal(propertyName, x) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="outType">Type of the out.</param>
        /// <param name="viewModelType">Owner VM type</param>
        /// <returns><c>true</c> if this instance can convert the specified types; otherwise, <c>false</c>.</returns>
        public abstract bool CanConvert(Type[] types, Type outType, Type viewModelType);

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns>System.Object.</returns>
        public abstract object? Convert(object?[] values, IViewModel viewModel);

        /// <summary>
        /// Determines whether this instance can convert back the specified in type.
        /// </summary>
        /// <param name="inType">Type of the in.</param>
        /// <param name="outTypes">The out types.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns><c>true</c> if this instance can convert back the specified in type; otherwise, <c>false</c>.</returns>
        public abstract bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType);

        /// <summary>
        /// Converts the specified values back.
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns>System.Object[].</returns>
        public abstract object?[] ConvertBack(object? value, IViewModel viewModel);
    }
}
