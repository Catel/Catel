// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerModifierModels.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    [SerializerModifier(typeof(ModelASerializerModifier))]
    public class ModelA : ModelBase
    {
        public string ModelAProperty
        {
            get { return GetValue<string>(ModelAPropertyProperty); }
            set { SetValue(ModelAPropertyProperty, value); }
        }

        public static readonly IPropertyData ModelAPropertyProperty = RegisterProperty("ModelAProperty", string.Empty);
    }

    [SerializerModifier(typeof(ModelBSerializerModifier))]
    public class ModelB : ModelA
    {
        public string ModelBProperty
        {
            get { return GetValue<string>(ModelBPropertyProperty); }
            set { SetValue(ModelBPropertyProperty, value); }
        }

        public static readonly IPropertyData ModelBPropertyProperty = RegisterProperty("ModelBProperty", string.Empty);
    }

    [SerializerModifier(typeof(ModelCSerializerModifier))]
    public class ModelC : ModelB
    {
        public string ModelCProperty
        {
            get { return GetValue<string>(ModelCPropertyProperty); }
            set { SetValue(ModelCPropertyProperty, value); }
        }

        public static readonly IPropertyData ModelCPropertyProperty = RegisterProperty("ModelCProperty", string.Empty);

        public string IgnoredMember
        {
            get { return GetValue<string>(IgnoredMemberProperty); }
            set { SetValue(IgnoredMemberProperty, value); }
        }

        public static readonly IPropertyData IgnoredMemberProperty = RegisterProperty("IgnoredMember", string.Empty);
    }

    [SerializerModifier(typeof(CTL550ModelSerializerModifier))]
    public class CTL550Model : ModelBase
    {
        [IncludeInSerialization]
        public Color Color { get; set; }
    }

    [SerializerModifier(typeof(ChangingTypeSerializerModifier))]
    public class ChangingType : ModelBase
    {
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<int> CustomizedCollection
        {
            get { return GetValue<ObservableCollection<int>>(CustomizedCollectionProperty); }
            set { SetValue(CustomizedCollectionProperty, value); }
        }

        /// <summary>
        /// Register the CustomizedCollection property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData CustomizedCollectionProperty = RegisterProperty("CustomizedCollection", () => new ObservableCollection<int>());
    }

    public class DynamicSerializerModifierModel : ModelBase
    {
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);
    }

    public class DynamicSerializerModifier : SerializerModifierBase<DynamicSerializerModifierModel>
    {
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "Name"))
            {
                memberValue.Value = "modified";
            }
        }
    }

    public class ModelASerializerModifier : SerializerModifierBase<ModelA>
    {
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "ModelAProperty"))
            {
                memberValue.Value = "ModifiedA";
            }
        }
    }

    public class ModelBSerializerModifier : SerializerModifierBase<ModelB>
    {
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "ModelBProperty"))
            {
                memberValue.Value = "ModifiedB";
            }
        }
    }

    public class ModelCSerializerModifier : SerializerModifierBase<ModelC>
    {
        public override bool ShouldIgnoreMember(ISerializationContext context, object model, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "IgnoredMember"))
            {
                return true;
            }

            return false;
        }

        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "ModelCProperty"))
            {
                memberValue.Value = "ModifiedC";
            }
        }
    }

    public class CTL550ModelSerializerModifier : SerializerModifierBase<TestModel>
    {
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "Color"))
            {
                var color = (Color)memberValue.Value;

                memberValue.Value = string.Format("{0}|{1}|{2}|{3}", color.A, color.R, color.G, color.B);
            }
        }

        public override void DeserializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "Color"))
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

    public class ChangingTypeSerializerModifier : SerializerModifierBase<ChangingType>
    {
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "CustomizedCollection"))
            {
                var originalCollection = memberValue.Value as ObservableCollection<int>;
                if (originalCollection is not null)
                {
                    var customizedCollection = new List<string>();
                    foreach (var item in originalCollection)
                    {
                        customizedCollection.Add(string.Format("Item {0}", item));
                    }

                    memberValue.Value = customizedCollection;
                }
            }
        }

        public override void DeserializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "CustomizedCollection"))
            {
                var customizedCollection = memberValue.Value as List<string>;
                if (customizedCollection is not null)
                {
                    var originalCollection = new ObservableCollection<int>();
                    foreach (var item in customizedCollection)
                    {
                        originalCollection.Add(int.Parse(item.Replace("Item ", string.Empty)));
                    }

                    memberValue.Value = originalCollection;
                }
            }
        }
    }
}
