namespace Catel.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.Reflection;
    using Catel.Tests.Runtime.Serialization;

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
}