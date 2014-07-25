namespace Catel.MVVM.ViewModels
{
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
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        bool CanConvert(object value, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        object Convert(object value, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        bool CanConvertBack(object value, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        object ConvertBack(object value, IViewModel viewModel);
        #endregion
    }
}