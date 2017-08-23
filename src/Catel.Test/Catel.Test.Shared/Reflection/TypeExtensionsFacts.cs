// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.Reflection;
    using Catel.Test.Runtime.Serialization;

    using NUnit.Framework;
    using JetBrains.dotMemoryUnit;

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

        public class MyModel : ModelBase
        {
            
        }

        [TestCase(typeof(object), false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), false)]
        [TestCase(typeof(TypeHelper), false)]
        [TestCase(typeof(ModelBase), true)]
        [TestCase(typeof(MyModel), true)]
        public void TheIsModelBaseMethod(Type type, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, type.IsModelBase());
        }

        public class MyCollection : ObservableCollection<int>
        {
            
        }

        [TestCase(typeof(int[]), typeof(int))]
        [TestCase(typeof(object), null)]
        [TestCase(typeof(ModelBase), null)]
        [TestCase(typeof(List<ModelBase>), typeof(ModelBase))]
        [TestCase(typeof(List<int>), typeof(int))]
        [TestCase(typeof(Collection<ModelBase>), typeof(ModelBase))]
        [TestCase(typeof(Collection<int>), typeof(int))]
        [TestCase(typeof(MyCollection), typeof(int))]
        public void TheGetCollectionElementTypeMethod(Type type, Type expectedElementType)
        {
            Assert.AreEqual(expectedElementType, type.GetCollectionElementType());
        }
    }

    [TestFixture]
    public class TheMakeGenericExMethod
    {
        class ClassA<T>
        {
        }

        [Test]
        [DotMemoryUnit(CollectAllocations = true)]
        [Ignore("Requires dotMemory")]
        public void Allocates_Less_Type_Arrays_Than_MakeGenericType()
        {
            int count = int.MaxValue;

            var memoryCheckPoint1 = dotMemory.Check();
            var makeGenericTypeEx1 = typeof(ClassA<>).MakeGenericTypeEx(typeof(int));
            var makeGenericTypeEx2 = typeof(ClassA<>).MakeGenericTypeEx(typeof(int));
            var makeGenericTypeEx3 = typeof(ClassA<>).MakeGenericTypeEx(typeof(int));
            var makeGenericTypeEx4 = typeof(ClassA<>).MakeGenericTypeEx(typeof(int));
            var makeGenericTypeEx5 = typeof(ClassA<>).MakeGenericTypeEx(typeof(int));
            dotMemory.Check(memory => count = memory.GetDifference(memoryCheckPoint1).GetNewObjects(where => where.Type.Is<Type[]>()).ObjectsCount);

            var memoryCheckPoint2 = dotMemory.Check();
            var makeGenericType1 = typeof(ClassA<>).MakeGenericType(typeof(int));
            var makeGenericType2 = typeof(ClassA<>).MakeGenericType(typeof(int));
            var makeGenericType3 = typeof(ClassA<>).MakeGenericType(typeof(int));
            var makeGenericType4 = typeof(ClassA<>).MakeGenericType(typeof(int));
            var makeGenericType5 = typeof(ClassA<>).MakeGenericType(typeof(int));
            dotMemory.Check(memory => Assert.Less(count, memory.GetDifference(memoryCheckPoint2).GetNewObjects(where => where.Type.Is<Type[]>()).ObjectsCount));
        }
    }
}