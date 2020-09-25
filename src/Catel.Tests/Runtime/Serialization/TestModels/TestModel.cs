// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    public class TestModel : ModelBase
    {
        #region Fields
        public string _excludedField;

        [IncludeInSerialization]
        public string _includedField;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public DateTime DateTimeProperty
        {
            get { return GetValue<DateTime>(DateTimePropertyProperty); }
            set { SetValue(DateTimePropertyProperty, value); }
        }

        /// <summary>
        /// Register the DateTimeProperty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData DateTimePropertyProperty = RegisterProperty("DateTimeProperty", typeof(DateTime), DateTime.Now);


        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string IncludedCatelProperty
        {
            get { return GetValue<string>(IncludedCatelPropertyProperty); }
            set { SetValue(IncludedCatelPropertyProperty, value); }
        }

        /// <summary>
        /// Register the IncludedCatelProperty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData IncludedCatelPropertyProperty = RegisterProperty("IncludedCatelProperty", typeof(string), null);


        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [ExcludeFromSerialization]
        public string ExcludedCatelProperty
        {
            get { return GetValue<string>(ExcludedCatelPropertyProperty); }
            set { SetValue(ExcludedCatelPropertyProperty, value); }
        }

        /// <summary>
        /// Register the ExcludedCatelProperty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ExcludedCatelPropertyProperty = RegisterProperty("ExcludedCatelProperty", typeof(string), null);


        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [ExcludeFromSerialization]
        protected string ExcludedProtectedCatelProperty
        {
            get { return GetValue<string>(ExcludedProtectedCatelPropertyProperty); }
            set { SetValue(ExcludedProtectedCatelPropertyProperty, value); }
        }

        /// <summary>
        /// Register the ExcludedProtectedCatelProperty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ExcludedProtectedCatelPropertyProperty = RegisterProperty("ExcludedProtectedCatelProperty", typeof(string), null);


        public string ExcludedRegularProperty { get; set; }

        [IncludeInSerialization]
        public string IncludedRegularProperty { get; set; }
        #endregion
    }
}
