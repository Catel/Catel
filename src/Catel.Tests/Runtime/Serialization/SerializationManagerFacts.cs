namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Linq;
    using Catel.Data;
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

                Assert.AreEqual(3, modifiers.Length);
                Assert.AreEqual(typeof(TestModels.ModelASerializerModifier), modifiers[0].GetType());
                Assert.AreEqual(typeof(TestModels.ModelBSerializerModifier), modifiers[1].GetType());
                Assert.AreEqual(typeof(TestModels.ModelCSerializerModifier), modifiers[2].GetType());
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

                Assert.AreEqual(1, fieldsToSerialize.Length);
                Assert.AreEqual("_includedField", fieldsToSerialize[0].Key);
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

                Assert.AreEqual(1, propertiesToSerialize.Length);
                Assert.AreEqual("IncludedRegularProperty", propertiesToSerialize[0].Key);
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

                Assert.AreEqual(2, propertiesToSerialize.Length);
                Assert.AreEqual("DateTimeProperty", propertiesToSerialize[0].Key);
                Assert.AreEqual("IncludedCatelProperty", propertiesToSerialize[1].Key);
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

                Assert.AreEqual(4, properties.Length);

                Assert.AreEqual("DateTimeProperty", properties[0]);
                Assert.AreEqual("IncludedCatelProperty", properties[1]);
                Assert.AreEqual("ExcludedCatelProperty", properties[2]);
                Assert.AreEqual("ExcludedProtectedCatelProperty", properties[3]);
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

                Assert.AreEqual(4, properties.Length);

                Assert.AreEqual("DateTimeProperty", properties[0].Key);
                Assert.AreEqual(SerializationMemberGroup.CatelProperty, properties[0].Value.MemberGroup);

                Assert.AreEqual("IncludedCatelProperty", properties[1].Key);
                Assert.AreEqual(SerializationMemberGroup.CatelProperty, properties[1].Value.MemberGroup);

                Assert.AreEqual("ExcludedCatelProperty", properties[2].Key);
                Assert.AreEqual(SerializationMemberGroup.CatelProperty, properties[2].Value.MemberGroup);

                Assert.AreEqual("ExcludedProtectedCatelProperty", properties[3].Key);
                Assert.AreEqual(SerializationMemberGroup.CatelProperty, properties[3].Value.MemberGroup);
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

                Assert.AreEqual(2, properties.Length);
                Assert.AreEqual("ExcludedRegularProperty", properties[0]);
                Assert.AreEqual("IncludedRegularProperty", properties[1]);
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

                Assert.AreEqual(2, properties.Length);
                Assert.AreEqual("ExcludedRegularProperty", properties[0].Key);
                Assert.AreEqual(SerializationMemberGroup.RegularProperty, properties[0].Value.MemberGroup);
                Assert.AreEqual("IncludedRegularProperty", properties[1].Key);
                Assert.AreEqual(SerializationMemberGroup.RegularProperty, properties[1].Value.MemberGroup);
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

                Assert.AreEqual(2, fields.Length);
                Assert.AreEqual("_excludedField", fields[0]);
                Assert.AreEqual("_includedField", fields[1]);
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

                Assert.AreEqual(2, fields.Length);
                Assert.AreEqual("_excludedField", fields[0].Key);
                Assert.AreEqual(SerializationMemberGroup.Field, fields[0].Value.MemberGroup);
                Assert.AreEqual("_includedField", fields[1].Key);
                Assert.AreEqual(SerializationMemberGroup.Field, fields[1].Value.MemberGroup);
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

                Assert.AreEqual(0, modifiers.Length);

                serializationManager.AddSerializerModifier<DynamicSerializerModifierModel, DynamicSerializerModifier>();

                modifiers = serializationManager.GetSerializerModifiers(typeof(DynamicSerializerModifierModel));

                Assert.AreEqual(1, modifiers.Length);
                Assert.AreEqual(typeof(DynamicSerializerModifier), modifiers[0].GetType());
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

                Assert.AreEqual(1, modifiers.Length);
                Assert.AreEqual(typeof(DynamicSerializerModifier), modifiers[0].GetType());

                serializationManager.RemoveSerializerModifier<DynamicSerializerModifierModel, DynamicSerializerModifier>();

                modifiers = serializationManager.GetSerializerModifiers(typeof(DynamicSerializerModifierModel));

                Assert.AreEqual(0, modifiers.Length);
            }
        }
    }
}