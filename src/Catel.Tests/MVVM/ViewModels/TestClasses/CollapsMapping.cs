// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollapsMapping.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System;
    using System.Linq;
    using Catel.MVVM;

    public class CollapsMapping : DefaultViewModelToModelMappingConverter
    {
        #region Fields
        private readonly char _separator;
        #endregion

        #region Constructors
        // Note: keep the constructor, it's used in the tests
        public CollapsMapping(string[] propertyNames)
            : this(propertyNames, ' ')
        {
        }

        public CollapsMapping(string[] propertyNames, char separator = ' ')
            : base(propertyNames)
        {
            _separator = separator;
        }
        #endregion

        #region Properties
        public char Separator
        {
            get { return _separator; }
        }
        #endregion

        #region Methods
        public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
        {
            return types.All(x => x == typeof(string)) && outType == typeof(string);
        }

        public override object Convert(object[] values, IViewModel viewModel)
        {
            return string.Join(Separator.ToString(), values.Where(x => !string.IsNullOrWhiteSpace((string) x)));
        }

        public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
        {
            return outTypes.All(x => x == typeof(string)) && inType == typeof(string);
        }

        public override object[] ConvertBack(object value, IViewModel viewModel)
        {
            return ((string) value).Split(Separator);
        }
        #endregion
    }
}