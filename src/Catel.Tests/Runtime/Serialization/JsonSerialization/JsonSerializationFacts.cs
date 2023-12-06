namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Globalization;
    using System.IO;
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
            [TestCase]
            public void CorrectlySerializesEnumsToString()
            {
                var serviceLocator = ServiceLocator.Default;
                var serializer = serviceLocator.ResolveType<IJsonSerializer>();

                var model = new CustomJsonSerializationModelWithEnum
                {
                    Name = "Test model with enum",
                    EnumWithAttribute = CustomSerializationEnum.SecondValue,
                    EnumWithoutAttribute = CustomSerializationEnum.SecondValue,
                };

                using (var memoryStream = new MemoryStream())
                {
                    serializer.Serialize(model, memoryStream);

                    memoryStream.Position = 0L;

                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        var streamAsText = streamReader.ReadToEnd();

                        Assert.That(streamAsText.Contains($"{nameof(CustomJsonSerializationModelWithEnum.EnumWithAttribute)}\":\"{CustomSerializationEnum.SecondValue}"), Is.True);
                        Assert.That(streamAsText.Contains($"{nameof(CustomJsonSerializationModelWithEnum.EnumWithoutAttribute)}\":{(int)CustomSerializationEnum.SecondValue}"), Is.True);
                    }
                }
            }

            [TestCase]
            public void CorrectlyDeserializesEnumsFromString()
            {
                var serviceLocator = ServiceLocator.Default;
                var serializer = serviceLocator.ResolveType<IJsonSerializer>();

                var model = new CustomJsonSerializationModelWithEnum
                {
                    Name = "Test model with enum",
                    EnumWithAttribute = CustomSerializationEnum.SecondValue,
                    EnumWithoutAttribute = CustomSerializationEnum.SecondValue,

                };

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, null);

                // Note: yes, the *model* is serialized, the *clonedModel* is deserialized

                Assert.That(clonedModel.EnumWithAttribute, Is.EqualTo(model.EnumWithAttribute));
                Assert.That(clonedModel.EnumWithoutAttribute, Is.EqualTo(model.EnumWithoutAttribute));
            }
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
                Assert.That(model.IsCustomSerialized, Is.True);
                Assert.That(clonedModel.IsCustomDeserialized, Is.True);

                Assert.That(clonedModel.FirstName, Is.EqualTo(model.FirstName));
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

                Assert.That(clonedModel.NestedModel, Is.Not.Null);

                // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                Assert.That(model.NestedModel.IsCustomSerialized, Is.True);
                Assert.That(clonedModel.NestedModel.IsCustomDeserialized, Is.True);

                Assert.That(clonedModel.Name, Is.EqualTo(model.Name));
                Assert.That(clonedModel.NestedModel.FirstName, Is.EqualTo(model.NestedModel.FirstName));
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

                var configuration = new JsonSerializationConfiguration
                {
                    UseBson = true
                };

                var json = testModel.ToJson(configuration);

                Assert.That(json.Contains("Excluded"), Is.False);
            }

            [TestCase]
            public void CorrectlySerializesToBsonString()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                var configuration = new JsonSerializationConfiguration
                {
                    UseBson = true
                };

                var json = testModel.ToJson(configuration);

                Assert.That(json.Contains("Excluded"), Is.False);
            }

            [TestCase]
            public void CorrectlySerializesObjectsWithFormattedIndents()
            {
                var serviceLocator = ServiceLocator.Default;
                var serializer = serviceLocator.ResolveType<IJsonSerializer>();

                var model = new CustomJsonSerializationModel
                {
                    FirstName = "Geert"
                };

                var configuration = new JsonSerializationConfiguration
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented
                };

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, configuration);

                // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                Assert.That(model.IsCustomSerialized, Is.True);
                Assert.That(clonedModel.IsCustomDeserialized, Is.True);

                Assert.That(clonedModel.FirstName, Is.EqualTo(model.FirstName));
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

                Assert.That(json.Contains(currentDateTime.ToString(culture)), Is.True);
            }
        }
    }
}
