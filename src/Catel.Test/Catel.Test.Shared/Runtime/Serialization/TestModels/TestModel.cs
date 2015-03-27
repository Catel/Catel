// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Windows.Media;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    [SerializerModifier(typeof(TestModelSerializerModifier))]
    public class TestModel : ModelBase
    {
        #region Constants
        /// <summary>
        /// Register the IncludedCatelProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IncludedCatelPropertyProperty = RegisterProperty("IncludedCatelProperty", typeof(string), null);

        /// <summary>
        /// Register the ExcludedCatelProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ExcludedCatelPropertyProperty = RegisterProperty("ExcludedCatelProperty", typeof(string), null);

        /// <summary>
        /// Register the ExcludedProtectedCatelProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ExcludedProtectedCatelPropertyProperty = RegisterProperty("ExcludedProtectedCatelProperty", typeof(string), null);
        #endregion

        #region Fields
        public string _excludedField;

        [IncludeInSerialization]
        public string _includedField;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string IncludedCatelProperty
        {
            get { return GetValue<string>(IncludedCatelPropertyProperty); }
            set { SetValue(IncludedCatelPropertyProperty, value); }
        }

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
        /// Gets or sets the property value.
        /// </summary>
        [ExcludeFromSerialization]
        protected string ExcludedProtectedCatelProperty
        {
            get { return GetValue<string>(ExcludedProtectedCatelPropertyProperty); }
            set { SetValue(ExcludedProtectedCatelPropertyProperty, value); }
        }

        public string ExcludedRegularProperty { get; set; }

        [IncludeInSerialization]
        public string IncludedRegularProperty { get; set; }

        [IncludeInSerialization]
        public Color NonSerializableProperty { get; set; }
        #endregion
    }

    public class TestModelSerializerModifier : SerializerModifierBase<TestModel>
    {
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            // Should see the Color property here, but I'm not.
            if (string.Equals(memberValue.Name, "NonSerializableProperty"))
            {
                var color = (Color)memberValue.Value;

                memberValue.Value = string.Format("{0}|{1}|{2}|{3}", color.A, color.R, color.G, color.B);
            }
        }

        public override void DeserializeMember(ISerializationContext context, MemberValue memberValue)
        {
            // Should see the Color property here, but I'm not.
            if (string.Equals(memberValue.Name, "NonSerializableProperty"))
            {
                var stringValue = memberValue.Value as string;
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    var splittedStringValue = stringValue.Split(new[] { '|' }, StringSplitOptions.None);
                    var a = (byte)int.Parse(splittedStringValue[0]);
                    var r = (byte)int.Parse(splittedStringValue[1]);
                    var g = (byte)int.Parse(splittedStringValue[2]);
                    var b = (byte)int.Parse(splittedStringValue[3]);

                    memberValue.Value = Color.FromArgb(a, r, g, b);
                }
            }
        }
    }
}