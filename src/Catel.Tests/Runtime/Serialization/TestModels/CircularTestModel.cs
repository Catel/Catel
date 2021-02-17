// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularTestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using Catel.Data;

#if NET || NETCORE
    [Serializable]
#endif
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
