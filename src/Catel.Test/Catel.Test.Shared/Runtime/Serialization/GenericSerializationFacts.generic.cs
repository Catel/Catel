// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.generic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using NUnit.Framework;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public class GenericTypesSerializationFacts
        {
            [TestCase]
            public void TestGenericSerialization()
            {
                var a = new A(3);
                var b = new B<A>(a);

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(b, serializer, config);

                    Assert.IsNotNull(clonedObject?.Item);
                    Assert.That(clonedObject.Item.Count, Is.EqualTo(a.Count));
                });
            }

            [Serializable]
            public class A : SavableModelBase<A>
            {
                [IncludeInSerialization]
                private readonly int _count;

                public A(int count)
                {
                    _count = count;
                }

                public A()
                {
                    ; //empty for deserialization
                }

                protected A(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }

                public int Count
                {
                    get { return _count; }
                }
            }

            [Serializable]
            public class B<T> : SavableModelBase<B<T>>
                where T : A
            {
                [IncludeInSerialization]
                private readonly T _item;

                public B(T item)
                {
                    _item = item;
                }

                public B()
                {
                    ; //empty for deserialization
                }

                protected B(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }

                public T Item
                {
                    get { return _item; }
                }
            }
        }
    }
}