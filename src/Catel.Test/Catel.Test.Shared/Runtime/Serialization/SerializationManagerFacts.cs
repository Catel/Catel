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

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class SerializationManagerFacts
    {
        [TestClass]
        public class TheGetSerializerModifiersMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetSerializerModifiers(null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheGetFieldsToSerializeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetFieldsToSerialize(null));
            }

            [TestMethod]
            public void ReturnsCorrectFields()
            {
                var serializationManager = new SerializationManager();

                var fieldsToSerialize = serializationManager.GetFieldsToSerialize(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, fieldsToSerialize.Length);
                Assert.AreEqual("_includedField", fieldsToSerialize[0]);
            }
        }

        [TestClass]
        public class TheGetPropertiesToSerializeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetPropertiesToSerialize(null));
            }

            [TestMethod]
            public void ReturnsCorrectProperties()
            {
                var serializationManager = new SerializationManager();

                var propertiesToSerialize = serializationManager.GetPropertiesToSerialize(typeof(TestModel)).ToArray();

                Assert.AreEqual(2, propertiesToSerialize.Length);
                Assert.AreEqual("IncludedCatelProperty", propertiesToSerialize[0]);
                Assert.AreEqual("IncludedRegularProperty", propertiesToSerialize[1]);
            }
        }

        [TestClass]
        public class TheGetCatelPropertyNamesMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetCatelPropertyNames(null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheGetCatelPropertiesMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetCatelProperties(null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheGetRegularPropertyNamesMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetRegularPropertyNames(null));
            }

            [TestMethod]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetRegularPropertyNames(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, properties.Length);
                Assert.AreEqual("IncludedRegularProperty", properties[0]);
            }
        }

        [TestClass]
        public class TheGetRegularPropertiesMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetRegularProperties(null));
            }

            [TestMethod]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var properties = serializationManager.GetRegularProperties(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, properties.Length);
                Assert.AreEqual("IncludedRegularProperty", properties[0].Key);
                Assert.AreEqual(SerializationMemberGroup.RegularProperty, properties[0].Value.MemberGroup);
            }
        }

        [TestClass]
        public class TheGetFieldNamesMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetFieldNames(null));
            }


            [TestMethod]
            public void ReturnsCorrectValue()
            {
                var serializationManager = new SerializationManager();

                var fields = serializationManager.GetFieldNames(typeof(TestModel)).ToArray();

                Assert.AreEqual(1, fields.Length);
                Assert.AreEqual("_includedField", fields[0]);
            }
        }

        [TestClass]
        public class TheGetFieldsMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serializationManager = new SerializationManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serializationManager.GetFields(null));
            }

            [TestMethod]
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