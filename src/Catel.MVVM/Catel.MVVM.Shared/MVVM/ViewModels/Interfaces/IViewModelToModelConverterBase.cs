namespace Catel.MVVM.ViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IViewModelToModelConverter
    {
        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">The name of changed property</param>
        /// <returns></returns>
        bool ShouldConvert(string propertyName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="outType"></param>
        /// <param name="viewModelType">Owner VM type</param>
        /// <returns></returns>
        bool CanConvert(Type[] types, Type outType, Type viewModelType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        object Convert(object[] values, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inType"></param>
        /// <param name="outTypes"></param>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        object[] ConvertBack(object value, IViewModel viewModel);
        #endregion
    }
}