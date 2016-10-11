// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Globalization;
    using Catel.IoC;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Json;
    using NUnit.Framework;
    using TestModels;

    public class JsonSerializationFacts
    {
        [TestFixture]
        public class BasicSerializationFacts
        {
        }

        [TestFixture]
        public class AdvancedSerializationFacts
        {
            [TestCase]
            public void CorrectlySerializesObjectsImplementingICustomJsonSerializable_Simple()
            {
                var serviceLocator = ServiceLocator.Default;
                var serializer = serviceLocator.ResolveType<IJsonSerializer>();

                var model = new CustomJsonSerializationModel
                {
                    FirstName = "Geert"
                };

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, null);

                // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                Assert.IsTrue(model.IsCustomSerialized);
                Assert.IsTrue(clonedModel.IsCustomDeserialized);

                Assert.AreEqual(model.FirstName, clonedModel.FirstName);
            }

            [TestCase]
            public void CorrectlySerializesObjectsImplementingICustomJsonSerializable_Nested()
            {
                var serviceLocator = ServiceLocator.Default;
                var serializer = serviceLocator.ResolveType<IJsonSerializer>();

                var model = new CustomJsonSerializationModelWithNesting
                {
                    Name = "Test model with nesting",
                    NestedModel = new CustomJsonSerializationModel
                    {
                        FirstName = "Geert"
                    }
                };

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, null);

                Assert.IsNotNull(clonedModel.NestedModel);

                // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                Assert.IsTrue(model.NestedModel.IsCustomSerialized);
                Assert.IsTrue(clonedModel.NestedModel.IsCustomDeserialized);

                Assert.AreEqual(model.Name, clonedModel.Name);
                Assert.AreEqual(model.NestedModel.FirstName, clonedModel.NestedModel.FirstName);
            }

            [TestCase]
            public void CorrectlySerializesToJsonString()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                var json = testModel.ToJson(null);

                Assert.IsFalse(json.Contains("Excluded"));
            }

            //[TestCase]
            //public void CorrectlySerializesToJsonStringWithInvariantCulture()
            //{
            //    TestSerializationUsingCulture(CultureInfo.InvariantCulture);
            //}

            //[TestCase]
            //public void CorrectlySerializesToJsonStringWithDutchCulture()
            //{
            //    TestSerializationUsingCulture(new CultureInfo("nl-NL"));
            //}

            //[TestCase]
            //public void CorrectlySerializesToJsonStringWithCustomCulture()
            //{
            //    var cultureInfo = new CultureInfo("en-US", true);
            //    cultureInfo.DateTimeFormat = new DateTimeFormatInfo
            //    {

            //    }

            //    TestSerializationUsingCulture(cultureInfo);
            //}

            private void TestSerializationUsingCulture(CultureInfo culture)
            {
                var testModel = new TestModel();

                var currentDateTime = DateTime.Now;
                testModel.DateTimeProperty = currentDateTime;

                var configuration = new SerializationConfiguration
                {
                    Culture = culture
                };

                var json = testModel.ToJson(configuration);

                Assert.IsTrue(json.Contains(currentDateTime.ToString(culture)));
            }
        }
    }
}