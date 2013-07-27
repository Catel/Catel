// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationObjectFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Runtime.Serialization
{
    using System;
    using Catel.Runtime.Serialization;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class SerializationObjectFacts
    {
        [TestClass]
        public class TheFailedToDeserializeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => SerializationObject.FailedToDeserialize(null, SerializationMemberGroup.CatelProperty,  "property"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, string.Empty));
            }
        }

        [TestClass]
        public class TheSucceededToDeserializeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => SerializationObject.SucceededToDeserialize(null, SerializationMemberGroup.CatelProperty, "property", null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, null, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, string.Empty, null));
            }
        }

        [TestClass]
        public class ThePropertyValueProperty
        {
            [TestMethod]
            public void ThrowsInvalidOperationExceptionForFailedDeserialization()
            {
                var serializationObject = SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, "property");
                object propertyValue = null;

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => propertyValue = serializationObject.MemberValue);

                Assert.IsNull(propertyValue);
            }

            [TestMethod]
            public void ReturnsPropertyValueForSucceededDeserialization()
            {
                var serializationObject = SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, "property", 42);
                object propertyValue = serializationObject.MemberValue;

                Assert.AreEqual(42, propertyValue);
            }
        }
    }
}