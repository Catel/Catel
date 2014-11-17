// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;

    public class SerializationManagerFacts
    {
        [TestFixture]
        public class TheGetSerializerModifiersMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetSerializerModifiers(null));
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

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetFieldsToSerialize(null));
            }

            [TestCase]
            public void ReturnsCorrectFields()
            {
                var serializationManager = new SerializationManager();

                var fieldsToSerialize = serializationManager.GetFieldsToSerialize(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, fieldsToSerialize.Length);
                Assert.AreEqual("_includedField", fieldsToSerialize[0]);
            }
        }

        [TestFixture]
        public class TheGetPropertiesToSerializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetPropertiesToSerialize(null));
            }

            [TestCase]
            public void ReturnsCorrectProperties()
            {
                var serializationManager = new SerializationManager();

                var propertiesToSerialize = serializationManager.GetPropertiesToSerialize(typeof(TestModel)).ToArray();

                Assert.AreEqual(2, propertiesToSerialize.Length);
                Assert.AreEqual("IncludedCatelProperty", propertiesToSerialize[0]);
                Assert.AreEqual("IncludedRegularProperty", propertiesToSerialize[1]);
            }
        }

        [TestFixture]
        public class TheGetCatelPropertyNamesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetCatelPropertyNames(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetCatelPropertyNames(typeof(TestModel)).ToArray();

                Assert.AreEqual(3, properties.Length);
                Assert.AreEqual("IncludedCatelProperty", properties[0]);
                Assert.AreEqual("ExcludedCatelProperty", properties[1]);
                Assert.AreEqual("ExcludedProtectedCatelProperty", properties[2]);
            }
        }

        [TestFixture]
        public class TheGetCatelPropertiesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetCatelProperties(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetCatelProperties(typeof(TestModel)).ToArray();

                Assert.AreEqual(3, properties.Length);
                Assert.AreEqual("IncludedCatelProperty", properties[0].Key);
                Assert.AreEqual(SerializationMemberGroup.CatelProperty, properties[0].Value.MemberGroup);

                Assert.AreEqual("ExcludedCatelProperty", properties[1].Key);
                Assert.AreEqual(SerializationMemberGroup.CatelProperty, properties[1].Value.MemberGroup);

                Assert.AreEqual("ExcludedProtectedCatelProperty", properties[2].Key);
                Assert.AreEqual(SerializationMemberGroup.CatelProperty, properties[2].Value.MemberGroup);
            }
        }

        [TestFixture]
        public class TheGetRegularPropertyNamesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetRegularPropertyNames(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetRegularPropertyNames(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, properties.Length);
                Assert.AreEqual("IncludedRegularProperty", properties[0]);
            }
        }

        [TestFixture]
        public class TheGetRegularPropertiesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetRegularProperties(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetRegularProperties(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, properties.Length);
                Assert.AreEqual("IncludedRegularProperty", properties[0].Key);
                Assert.AreEqual(SerializationMemberGroup.RegularProperty, properties[0].Value.MemberGroup);
            }
        }

        [TestFixture]
        public class TheGetFieldNamesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetFieldNames(null));
            }


            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var fields = serializationManager.GetFieldNames(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, fields.Length);
                Assert.AreEqual("_includedField", fields[0]);
            }
        }

        [TestFixture]
        public class TheGetFieldsMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetFields(null));
            }

            [TestCase]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var fields = serializationManager.GetFields(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, fields.Length);
                Assert.AreEqual("_includedField", fields[0].Key);
                Assert.AreEqual(SerializationMemberGroup.Field, fields[0].Value.MemberGroup);
            }
        }
    }
}