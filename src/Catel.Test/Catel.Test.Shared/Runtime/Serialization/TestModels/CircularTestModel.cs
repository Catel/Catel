// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularTestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using Catel.Data;

#if NET
    using System.Runtime.Serialization;
#endif

#if NET
    [Serializable]
#endif
    public class CircularTestModel : ModelBase
    {
#if NET
        protected CircularTestModel(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif

        public CircularTestModel()
        {
            Name = UniqueIdentifierHelper.GetUniqueIdentifier<CircularTestModel>().ToString();
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), "Test name");

        public CircularTestModel CircularModel
        {
            get { return GetValue<CircularTestModel>(CircularModelProperty); }
            set { SetValue(CircularModelProperty, value); }
        }

        public static readonly PropertyData CircularModelProperty = RegisterProperty("CircularModel", typeof(CircularTestModel), null);
    }
}