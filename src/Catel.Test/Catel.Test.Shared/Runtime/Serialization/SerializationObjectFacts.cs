// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationObjectFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Runtime.Serialization
{
    using System;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;

    public class SerializationObjectFacts
    {
        [TestFixture]
        public class TheFailedToDeserializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => SerializationObject.FailedToDeserialize(null, SerializationMemberGroup.CatelProperty,  "property"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, string.Empty));
            }
        }

        [TestFixture]
        public class TheSucceededToDeserializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => SerializationObject.SucceededToDeserialize(null, SerializationMemberGroup.CatelProperty, "property", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, null, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, string.Empty, null));
            }
        }

        [TestFixture]
        public class ThePropertyValueProperty
        {
            [TestCase]
            public void ThrowsInvalidOperationExceptionForFailedDeserialization()
            {
                var serializationObject = SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, "property");
                object propertyValue = null;

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => propertyValue = serializationObject.MemberValue);

                Assert.IsNull(propertyValue);
            }

            [TestCase]
            public void ReturnsPropertyValueForSucceededDeserialization()
            {
                var serializationObject = SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, "property", 42);
                object propertyValue = serializationObject.MemberValue;

                Assert.AreEqual(42, propertyValue);
            }
        }
    }
}