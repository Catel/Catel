// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Reflection
{
    using System;
    using Catel.Collections;
    using Catel.Reflection;
    using Catel.Test.Runtime.Serialization;

    using NUnit.Framework;

    public class TypeExtensionsFacts
    {
        #region Nested type: TheIsBasicTypeMethod
        [TestFixture]
        public class TheIsBasicTypeMethod
        {
            [TestCase]
            public void ThrowsNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => TypeExtensions.IsBasicType(null));
            }

            [TestCase]
            public void ReturnsTrueForIntType()
            {
                Assert.IsTrue(typeof(int).IsBasicType());
            }

            [TestCase]
            public void ReturnsTrueForNullableIntType()
            {
                Assert.IsTrue(typeof(int?).IsBasicType());
            }

            [TestCase]
            public void ReturnsFalseForFastObservableCollectionType()
            {
                Assert.IsFalse(typeof(FastObservableCollection<TestModel>).IsBasicType());
            }
        }
        #endregion

        #region Nested type: TheIsClassTypeMethod
        [TestFixture]
        public class TheIsClassTypeMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsFalseForNullType()
            {
                Assert.IsFalse(TypeExtensions.IsClassType(null));
            }

            [TestCase]
            public void ReturnsFalseForValueType()
            {
                Assert.IsFalse(TypeExtensions.IsClassType(typeof(int)));
            }

            [TestCase]
            public void ReturnsFalseForStringType()
            {
                Assert.IsFalse(TypeExtensions.IsClassType(typeof(string)));
            }

            [TestCase]
            public void ReturnsTrueForClassType()
            {
                Assert.IsTrue(TypeExtensions.IsClassType(typeof(TypeHelper)));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsTypeNullableMethod
        [TestFixture]
        public class TheIsNullableTypeMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsTrueForReferenceType()
            {
                Assert.IsTrue(TypeExtensions.IsNullableType(typeof(object)));
            }

            [TestCase]
            public void ReturnsFalseForValueType()
            {
                Assert.IsFalse(TypeExtensions.IsNullableType(typeof(int)));
            }

            [TestCase]
            public void ReturnsFalseForNullType()
            {
                Assert.IsFalse(TypeExtensions.IsNullableType(null));
            }

            [TestCase]
            public void ReturnsTrueForNullableValueType()
            {
                Assert.IsTrue(TypeExtensions.IsNullableType(typeof(bool?)));
            }
            #endregion
        }
        #endregion
    }
}