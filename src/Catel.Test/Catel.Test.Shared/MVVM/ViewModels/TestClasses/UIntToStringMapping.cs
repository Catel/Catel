// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIntToStringMapping.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using System;
    using Catel.MVVM;

    public class UIntToStringMapping : DefaultViewModelToModelMappingConverter
    {
        #region Constructors
        public UIntToStringMapping(string[] propertyNames)
            : base(propertyNames)
        {
        }
        #endregion

        #region Methods
        public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
        {
            return types.Length == 1 && types[0] == typeof(uint) && outType == typeof(string);
        }

        public override object Convert(object[] values, IViewModel viewModel)
        {
            return ((uint) values[0]).ToString();
        }

        public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
        {
            return outTypes.Length == 1 && outTypes[0] == typeof(uint) && inType == typeof(string);
        }

        public override object[] ConvertBack(object value, IViewModel viewModel)
        {
            uint res = 0;
            uint.TryParse((string) value, out res);
            return new object[] {res};
        }
        #endregion
    }
}