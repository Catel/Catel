// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Extensions.DynamicObjects
{
    using System;
    using System.IO;
    using System.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;
    using System.Linq.Expressions;

    public class DynamicModelBaseFacts
    {
        public class DynamicModel : DynamicModelBase
        {
            public string ExistingProperty
            {
                get { return GetValue<string>(ExistingPropertyProperty); }
                set { SetValue(ExistingPropertyProperty, value); }
            }
            public static readonly PropertyData ExistingPropertyProperty = RegisterProperty("ExistingProperty", typeof(string), null);
        }

        [TestFixture]
        public class TheGetDynamicMemberNamesMethod
        {
            [TestCase]
            public void CorrectlyReturnsDynamicMemberNames()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                model.NonExistingProperty = "test";
                model.NonExistingProperty2 = "test";
                model.NonExistingProperty3 = "test";
                model.NonExistingProperty4 = "test";
                model.NonExistingProperty5 = "test";

                // Get dynamic member names and sort (we get keys from dictionary where order is unspecified, so it's better to sort by names).
                var memberNames = dynamicModel.GetMetaObject(Expression.Constant(dynamicModel)).GetDynamicMemberNames().ToList();
                memberNames.Sort();
                Assert.AreEqual(5, memberNames.Count);
                Assert.AreEqual("NonExistingProperty", memberNames[0]);
                Assert.AreEqual("NonExistingProperty2", memberNames[1]);
                Assert.AreEqual("NonExistingProperty3", memberNames[2]);
                Assert.AreEqual("NonExistingProperty4", memberNames[3]);
                Assert.AreEqual("NonExistingProperty5", memberNames[4]);
            }
        }

        [TestFixture]
        public class TheGetValueProperties
        {
            [TestCase]
            public void CorrectlyReturnsTheRightValue()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                model.NonExistingProperty = "test";

                Assert.IsTrue(dynamicModel.IsPropertyRegistered("NonExistingProperty"));

                Assert.AreEqual("test", model.NonExistingProperty);
            }
        }

        [TestFixture]
        public class TheSetValueProperties
        {
            [TestCase]
            public void DynamicPropertiesAreRegisteredAsDynamic()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                model.NonExistingProperty = "test";

                Assert.IsTrue(dynamicModel.IsPropertyRegistered("NonExistingProperty"));
                Assert.IsTrue(dynamicModel.IsPropertyRegisteredAsDynamic("NonExistingProperty"));
            }

            [TestCase]
            public void NonDynamicPropertiesAreNotRegisteredAsDynamic()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                Assert.IsTrue(dynamicModel.IsPropertyRegistered("ExistingProperty"));
                Assert.IsFalse(dynamicModel.IsPropertyRegisteredAsDynamic("ExistingProperty"));
            }
        }

        [TestFixture]
        public class TheModelBaseFunctionality
        {
            [TestCase]
            public void SupportsSerialization()
            {
                dynamic model = new DynamicModel();
                model.NonExistingProperty = "a dynamic value";

                var serializer = SerializationFactory.GetXmlSerializer();

                using (var memoryStream = new MemoryStream())
                {
                    var dynamicModel = (DynamicModel)model;
                    serializer.Serialize(dynamicModel, memoryStream);

                    memoryStream.Position = 0L;

                    dynamic deserializedModel = serializer.Deserialize(typeof(DynamicModel), memoryStream);
                    var deserializedDynamicModel = (DynamicModel) deserializedModel;

                    Assert.IsTrue(deserializedDynamicModel.IsPropertyRegistered("NonExistingProperty"));
                    Assert.AreEqual("a dynamic value", deserializedModel.NonExistingProperty);
                }
            }
        }
    }
}