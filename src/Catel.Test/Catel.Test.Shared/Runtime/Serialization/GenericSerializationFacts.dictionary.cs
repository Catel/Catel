// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Media;
    using Catel.Collections;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Binary;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;
    using Data;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TestModels;
    using JsonSerializer = Catel.Runtime.Serialization.Json.JsonSerializer;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public class TheKeyValuePairSerializerModifier
        {
            [TestCase]
            public void SerializesAndDeserializesKeyValuePairs()
            {
                var originalObject = new TestModelWithKeyValuePair();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject.KeyValuePair, clonedObject.KeyValuePair, description);
                    Assert.AreEqual(originalObject.KeyValuePairAsObject, clonedObject.KeyValuePairAsObject, description);
                });
            }
        }
    }
}