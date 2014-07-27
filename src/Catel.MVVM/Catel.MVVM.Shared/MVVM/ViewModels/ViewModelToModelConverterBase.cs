// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToModelConverterBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.ViewModels
{
    using System;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public abstract class ViewModelToModelConverterBase : IViewModelToModelConverter
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyNames">Name of property on which attribute was setted</param>
        protected ViewModelToModelConverterBase(string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string[] PropertyNames { get; private set; }
        #endregion

        #region IViewModelToModelConverter Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">The name of changed property</param>
        /// <returns></returns>
        public bool ShouldConvert(string propertyName)
        {
            foreach (string x in PropertyNames)
            {
                if (string.CompareOrdinal(propertyName, x) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="outType"></param>
        /// <param name="viewModelType">Owner VM type</param>
        /// <returns></returns>
        public abstract bool CanConvert(Type[] types, Type outType, Type viewModelType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public abstract object Convert(object[] values, IViewModel viewModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inType"></param>
        /// <param name="outTypes"></param>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public abstract bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Property value</param>
        /// <param name="viewModel">Owner VM</param>
        /// <returns></returns>
        public abstract object[] ConvertBack(object value, IViewModel viewModel);
        
        #endregion
    }
}