// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerModifierModels.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization.TestModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

        public static readonly PropertyData ModelAPropertyProperty = RegisterProperty("ModelAProperty", typeof(string), null);
    }

    [SerializerModifier(typeof(ModelBSerializerModifier))]
    public class ModelB : ModelA
    {
        public string ModelBProperty
        {
            get { return GetValue<string>(ModelBPropertyProperty); }
            set { SetValue(ModelBPropertyProperty, value); }
        }

        public static readonly PropertyData ModelBPropertyProperty = RegisterProperty("ModelBProperty", typeof(string), null);
    }

    [SerializerModifier(typeof(ModelCSerializerModifier))]
    public class ModelC : ModelB
    {
        public string ModelCProperty
        {
            get { return GetValue<string>( ModelCPropertyProperty); }
            set { SetValue( ModelCPropertyProperty, value); }
        }

        public static readonly PropertyData  ModelCPropertyProperty = RegisterProperty("ModelCProperty", typeof(string), null);

        public string IgnoredMember
        {
            get { return GetValue<string>(IgnoredMemberProperty); }
            set { SetValue(IgnoredMemberProperty, value); }
        }

        public static readonly PropertyData IgnoredMemberProperty = RegisterProperty("IgnoredMember", typeof(string), null);
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
        public static readonly PropertyData CustomizedCollectionProperty = RegisterProperty("CustomizedCollection", typeof(ObservableCollection<int>), () => new ObservableCollection<int>());
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
        public override bool ShouldIgnoreMember(ISerializationContext context, IModel model, MemberValue memberValue)
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

    public class ChangingTypeSerializerModifier : SerializerModifierBase<ChangingType>
    {
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "CustomizedCollection"))
            {
                var originalCollection = memberValue.Value as ObservableCollection<int>;
                if (originalCollection != null)
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
                if (customizedCollection != null)
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