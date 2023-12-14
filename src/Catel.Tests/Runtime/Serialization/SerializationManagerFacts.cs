namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Linq;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;
    using TestModels;

    public class SerializationManagerFacts
    {
        [TestFixture]
        public class TheGetSerializerModifiersMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetSerializerModifiers(null));
            }

            [TestCase]
            public void ReturnsRightModifiersInRightOrderForInheritedClasses()
            {
                var serializationManager = new SerializationManager();

                var modifiers = serializationManager.GetSerializerModifiers(typeof(TestModels.ModelC));

                Assert.That(modifiers.Length, Is.EqualTo(3));
                Assert.That(modifiers[0].GetType(), Is.EqualTo(typeof(TestModels.ModelASerializerModifier)));
                Assert.That(modifiers[1].GetType(), Is.EqualTo(typeof(TestModels.ModelBSerializerModifier)));
                Assert.That(modifiers[2].GetType(), Is.EqualTo(typeof(TestModels.ModelCSerializerModifier)));
            }
        }

        [TestFixture]
        public class TheGetFieldsToSerializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetFieldsToSerialize(null));
            }

            [TestCase]
            public void ReturnsCorrectFields()
            {
                var serializationManager = new SerializationManager();

                var fieldsToSerialize = serializationManager.GetFieldsToSerialize(typeof(TestModel)).ToArray();

                Assert.That(fieldsToSerialize.Length, Is.EqualTo(1));
                Assert.That(fieldsToSerialize[0].Key, Is.EqualTo("_includedField"));
            }
        }

        [TestFixture]
        public class TheGetRegularPropertiesToSerializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetRegularPropertiesToSerialize(null));
            }

            [TestCase]
            public void ReturnsCorrectProperties()
            {
                var serializationManager = new SerializationManager();

                var propertiesToSerialize = serializationManager.GetRegularPropertiesToSerialize(typeof(TestModel)).ToArray();

                Assert.That(propertiesToSerialize.Length, Is.EqualTo(1));
                Assert.That(propertiesToSerialize[0].Key, Is.EqualTo("IncludedRegularProperty"));
            }
        }

        [TestFixture]
        public class TheGetCatelPropertiesToSerializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetCatelPropertiesToSerialize(null));
            }

            [TestCase]
            public void ReturnsCorrectProperties()
            {
                var serializationManager = new SerializationManager();

                var propertiesToSerialize = serializationManager.GetCatelPropertiesToSerialize(typeof(TestModel)).ToArray();

                Assert.That(propertiesToSerialize.Length, Is.EqualTo(2));
                Assert.That(propertiesToSerialize[0].Key, Is.EqualTo("DateTimeProperty"));
                Assert.That(propertiesToSerialize[1].Key, Is.EqualTo("IncludedCatelProperty"));
            }
        }

        [TestFixture]
        public class TheGetCatelPropertyNamesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetCatelPropertyNames(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetCatelPropertyNames(typeof(TestModel)).ToArray();

                Assert.That(properties.Length, Is.EqualTo(4));

                Assert.That(properties[0], Is.EqualTo("DateTimeProperty"));
                Assert.That(properties[1], Is.EqualTo("IncludedCatelProperty"));
                Assert.That(properties[2], Is.EqualTo("ExcludedCatelProperty"));
                Assert.That(properties[3], Is.EqualTo("ExcludedProtectedCatelProperty"));
            }
        }

        [TestFixture]
        public class TheGetCatelPropertiesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetCatelProperties(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetCatelProperties(typeof(TestModel)).ToArray();

                Assert.That(properties.Length, Is.EqualTo(4));

                Assert.That(properties[0].Key, Is.EqualTo("DateTimeProperty"));
                Assert.That(properties[0].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.CatelProperty));

                Assert.That(properties[1].Key, Is.EqualTo("IncludedCatelProperty"));
                Assert.That(properties[1].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.CatelProperty));

                Assert.That(properties[2].Key, Is.EqualTo("ExcludedCatelProperty"));
                Assert.That(properties[2].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.CatelProperty));

                Assert.That(properties[3].Key, Is.EqualTo("ExcludedProtectedCatelProperty"));
                Assert.That(properties[3].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.CatelProperty));
            }
        }

        [TestFixture]
        public class TheGetRegularPropertyNamesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetRegularPropertyNames(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetRegularPropertyNames(typeof(TestModel)).ToArray();

                Assert.That(properties.Length, Is.EqualTo(2));
                Assert.That(properties[0], Is.EqualTo("ExcludedRegularProperty"));
                Assert.That(properties[1], Is.EqualTo("IncludedRegularProperty"));
            }
        }

        [TestFixture]
        public class TheGetRegularPropertiesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetRegularProperties(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetRegularProperties(typeof(TestModel)).ToArray();

                Assert.That(properties.Length, Is.EqualTo(2));
                Assert.That(properties[0].Key, Is.EqualTo("ExcludedRegularProperty"));
                Assert.That(properties[0].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.RegularProperty));
                Assert.That(properties[1].Key, Is.EqualTo("IncludedRegularProperty"));
                Assert.That(properties[1].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.RegularProperty));
            }
        }

        [TestFixture]
        public class TheGetFieldNamesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetFieldNames(null));
            }


            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var fields = serializationManager.GetFieldNames(typeof(TestModel)).ToArray();

                Assert.That(fields.Length, Is.EqualTo(2));
                Assert.That(fields[0], Is.EqualTo("_excludedField"));
                Assert.That(fields[1], Is.EqualTo("_includedField"));
            }
        }

        [TestFixture]
        public class TheGetFieldsMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.GetFields(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var fields = serializationManager.GetFields(typeof(TestModel)).ToArray();

                Assert.That(fields.Length, Is.EqualTo(2));
                Assert.That(fields[0].Key, Is.EqualTo("_excludedField"));
                Assert.That(fields[0].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.Field));
                Assert.That(fields[1].Key, Is.EqualTo("_includedField"));
                Assert.That(fields[1].Value.MemberGroup, Is.EqualTo(SerializationMemberGroup.Field));
            }
        }

        [TestFixture]
        public class TheAddSerializerModifierMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.AddSerializerModifier(null, typeof(DynamicSerializerModifier)));
                Assert.Throws<ArgumentNullException>(() => serializationManager.AddSerializerModifier(typeof(DynamicSerializerModifierModel), null));
            }

            [TestCase]
            public void AddsSerializerModifier()
            {
                var serializationManager = new SerializationManager();

                var modifiers = serializationManager.GetSerializerModifiers<DynamicSerializerModifierModel>();

                Assert.That(modifiers.Length, Is.EqualTo(0));

                serializationManager.AddSerializerModifier<DynamicSerializerModifierModel, DynamicSerializerModifier>();

                modifiers = serializationManager.GetSerializerModifiers(typeof(DynamicSerializerModifierModel));

                Assert.That(modifiers.Length, Is.EqualTo(1));
                Assert.That(modifiers[0].GetType(), Is.EqualTo(typeof(DynamicSerializerModifier)));
            }
        }

        [TestFixture]
        public class TheRemoveSerializerModifierMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                Assert.Throws<ArgumentNullException>(() => serializationManager.RemoveSerializerModifier(null, typeof(DynamicSerializerModifier)));
                Assert.Throws<ArgumentNullException>(() => serializationManager.RemoveSerializerModifier(typeof(DynamicSerializerModifierModel), null));
            }

            [TestCase]
            public void RemovesSerializerModifier()
            {
                var serializationManager = new SerializationManager();

                serializationManager.AddSerializerModifier<DynamicSerializerModifierModel, DynamicSerializerModifier>();

                var modifiers = serializationManager.GetSerializerModifiers(typeof(DynamicSerializerModifierModel));

                Assert.That(modifiers.Length, Is.EqualTo(1));
                Assert.That(modifiers[0].GetType(), Is.EqualTo(typeof(DynamicSerializerModifier)));

                serializationManager.RemoveSerializerModifier<DynamicSerializerModifierModel, DynamicSerializerModifier>();

                modifiers = serializationManager.GetSerializerModifiers(typeof(DynamicSerializerModifierModel));

                Assert.That(modifiers.Length, Is.EqualTo(0));
            }
        }
    }
}