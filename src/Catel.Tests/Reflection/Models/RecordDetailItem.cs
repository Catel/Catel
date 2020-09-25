namespace Catel.Tests.Reflection.Models
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    public class RecordDetailItem : ModelBase
    {
        public RecordDetailItem()
        {
            Children = new List<RecordDetailItem>();
        }


        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string), null);


        public bool BoolValue
        {
            get { return GetValue<bool>(BoolValueProperty); }
            set { SetValue(BoolValueProperty, value); }
        }

        public static readonly IPropertyData BoolValueProperty = RegisterProperty(nameof(BoolValue), typeof(bool), null);


        public int IntValue
        {
            get { return GetValue<int>(IntValueProperty); }
            set { SetValue(IntValueProperty, value); }
        }

        public static readonly IPropertyData IntValueProperty = RegisterProperty(nameof(IntValue), typeof(int), null);


        public Type Type
        {
            get { return GetValue<Type>(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly IPropertyData TypeProperty = RegisterProperty(nameof(Type), typeof(Type), null);


        public object Value
        {
            get { return GetValue<object>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly IPropertyData ValueProperty = RegisterProperty(nameof(Value), typeof(object), null);


        public RecordDetailItemValue ComparedValue1
        {
            get { return GetValue<RecordDetailItemValue>(ComparedValue1Property); }
            set { SetValue(ComparedValue1Property, value); }
        }

        public static readonly IPropertyData ComparedValue1Property = RegisterProperty(nameof(ComparedValue1), typeof(RecordDetailItemValue), null);


        public RecordDetailItemValue ComparedValue2
        {
            get { return GetValue<RecordDetailItemValue>(ComparedValue2Property); }
            set { SetValue(ComparedValue2Property, value); }
        }

        public static readonly IPropertyData ComparedValue2Property = RegisterProperty(nameof(ComparedValue2), typeof(RecordDetailItemValue), null);


        public List<RecordDetailItem> Children
        {
            get { return GetValue<List<RecordDetailItem>>(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        public static readonly IPropertyData ChildrenProperty = RegisterProperty(nameof(Children), typeof(List<RecordDetailItem>), null);

        public override string ToString()
        {
            return $"{Name}: '{Value ?? "NULL"}'";
        }
    }
}
