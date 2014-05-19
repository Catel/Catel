// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Reflection
{
    using System;
    using Catel.Collections;
    using Catel.Reflection;
    using Catel.Test.Runtime.Serialization;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class TypeExtensionsFacts
    {
        #region Nested type: TheIsBasicTypeMethod
        [TestClass]
        public class TheIsBasicTypeMethod
        {
            [TestMethod]
            public void ThrowsNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => TypeExtensions.IsBasicType(null));
            }

            [TestMethod]
            public void ReturnsTrueForIntType()
            {
                Assert.IsTrue(typeof(int).IsBasicType());
            }

            [TestMethod]
            public void ReturnsTrueForNullableIntType()
            {
                Assert.IsTrue(typeof(int?).IsBasicType());
            }

            [TestMethod]
            public void ReturnsFalseForFastObservableCollectionType()
            {
                Assert.IsFalse(typeof(FastObservableCollection<TestModel>).IsBasicType());
            }
        }
        #endregion

        #region Nested type: TheIsClassTypeMethod
        [TestClass]
        public class TheIsClassTypeMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsFalseForNullType()
            {
                Assert.IsFalse(TypeExtensions.IsClassType(null));
            }

            [TestMethod]
            public void ReturnsFalseForValueType()
            {
                Assert.IsFalse(TypeExtensions.IsClassType(typeof(int)));
            }

            [TestMethod]
            public void ReturnsFalseForStringType()
            {
                Assert.IsFalse(TypeExtensions.IsClassType(typeof(string)));
            }

            [TestMethod]
            public void ReturnsTrueForClassType()
            {
                Assert.IsTrue(TypeExtensions.IsClassType(typeof(TypeHelper)));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsTypeNullableMethod
        [TestClass]
        public class TheIsNullableTypeMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsTrueForReferenceType()
            {
                Assert.IsTrue(TypeExtensions.IsNullableType(typeof(object)));
            }

            [TestMethod]
            public void ReturnsFalseForValueType()
            {
                Assert.IsFalse(TypeExtensions.IsNullableType(typeof(int)));
            }

            [TestMethod]
            public void ReturnsFalseForNullType()
            {
                Assert.IsFalse(TypeExtensions.IsNullableType(null));
            }

            [TestMethod]
            public void ReturnsTrueForNullableValueType()
            {
                Assert.IsTrue(TypeExtensions.IsNullableType(typeof(bool?)));
            }
            #endregion
        }
        #endregion
    }
}