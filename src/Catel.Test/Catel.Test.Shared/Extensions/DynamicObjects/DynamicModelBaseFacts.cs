// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Extensions.DynamicObjects
{
    using System;
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;

    public class DynamicModelBaseFacts
    {
        public class DynamicModel : DynamicModelBase
        {
        }

        [TestFixture]
        public class TheGetValueProperties
        {
            [TestCase]
            public void CorrectlyReturnsTheRightValue()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                model.NonExistingGetProperty = "test";

                Assert.IsTrue(dynamicModel.IsPropertyRegistered("NonExistingGetProperty"));

                Assert.AreEqual("test", model.NonExistingGetProperty);
            }
        }

        [TestFixture]
        public class TheSetValueProperties
        {
            [TestCase]
            public void AutomaticallyRegistersNonExistingProperty()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                Assert.IsFalse(dynamicModel.IsPropertyRegistered("NonExistingSetProperty"));

                model.NonExistingSetProperty = "test";

                Assert.IsTrue(dynamicModel.IsPropertyRegistered("NonExistingSetProperty"));
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