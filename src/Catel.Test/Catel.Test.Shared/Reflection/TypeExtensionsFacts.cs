// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Reflection
{
    using System;
    using System.Collections.Generic;
    using Catel.Collections;
    using Catel.Reflection;
    using Catel.Test.Runtime.Serialization;

    using NUnit.Framework;

    [TestFixture]
    public class TypeExtensionsFacts
    {
        [TestCase(typeof(int), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(FastObservableCollection<TestModel>), false)]
        public void TheIsBasicTypeMethod(Type type, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, type.IsBasicType());
        }

        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(string), false)]
        [TestCase(typeof(TypeHelper), true)]
        public void TheIsClassTypeMethod(Type type, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, type.IsClassType());
        }

        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(FastObservableCollection<int>), true)]
        public void TheIsCollectionMethod(Type type, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, type.IsCollection());
        }

        [TestCase(null, false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(FastObservableCollection<int>), false)]
        [TestCase(typeof(Dictionary<int, bool>), true)]
        public void TheIsDictionaryMethod(Type type, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, type.IsDictionary());
        }

        [TestCase(null, false)]
        [TestCase(typeof(object), true)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(TypeHelper), true)]
        public void TheIsNullableTypeMethod(Type type, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, type.IsNullableType());
        }
    }
}