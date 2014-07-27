namespace Catel.MVVM.ViewModels
{
    using System;
    using Reflection;

    /// <summary>
    /// ViewModelToModelMapping Copy Converter
    /// </summary>
    public class ViewModelToModelCopyConverter : ViewModelToModelConverterBase
    {
        /// <summary>
        /// Creates an instanse of converter
        /// </summary>
        /// <param name="propertyNames">All properties to watch</param>
        public ViewModelToModelCopyConverter(string[] propertyNames)
            : base(propertyNames)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="outType"></param>
        /// <param name="viewModelType">Owner VM type</param>
        /// <returns></returns>
        public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
        {
            return types.Length == 1 && outType.IsAssignableFromEx(types[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public override object Convert(object[] values, IViewModel viewModel)
        {
            return values[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inType"></param>
        /// <param name="outTypes"></param>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
        {
            return outTypes.Length == 1 && outTypes[0].IsAssignableFromEx(inType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public override object[] ConvertBack(object value, IViewModel viewModel)
        {
            return new[] {value};
        }
    }
}