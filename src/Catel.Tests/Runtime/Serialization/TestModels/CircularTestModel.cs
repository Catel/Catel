namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using Catel.Data;

    [Serializable]
    public class CircularTestModel : ModelBase
    {
        public CircularTestModel()
        {
            Name = UniqueIdentifierHelper.GetUniqueIdentifier<CircularTestModel>().ToString();
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", "Test name");

        public CircularTestModel CircularModel
        {
            get { return GetValue<CircularTestModel>(CircularModelProperty); }
            set { SetValue(CircularModelProperty, value); }
        }

        public static readonly IPropertyData CircularModelProperty = RegisterProperty<CircularTestModel>("CircularModel");
    }
}
