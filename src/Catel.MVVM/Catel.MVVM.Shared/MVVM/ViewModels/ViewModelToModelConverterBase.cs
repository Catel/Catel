// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToModelConverterBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ViewModelToModelConverter : IViewModelToModelConverter
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">Name of property on which attribute was setted</param>
        protected ViewModelToModelConverter(string propertyName)
        {
            PropertyName = propertyName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; private set; }
        #endregion

        #region IViewModelToModelConverterBase Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">The name of changed property</param>
        /// <returns></returns>
        public bool ShouldConvert(string propertyName)
        {
            return string.CompareOrdinal(propertyName, PropertyName) == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public abstract bool CanConvert(object value, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public abstract object Convert(object value, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public abstract bool CanConvertBack(object value, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, IViewModel viewModel);
        #endregion
    }
}