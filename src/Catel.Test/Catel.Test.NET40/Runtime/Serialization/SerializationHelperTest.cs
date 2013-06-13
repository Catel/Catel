// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Catel.Runtime.Serialization;

#if NET
    using System.Runtime.Serialization.Formatters.Binary;
#endif

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class Super
    {
        public class Sub
        {
            
        }
    }

    /// <summary>
    /// Summary description for SerializationHelperTest
    /// </summary>
    [TestClass]
    public class SerializationHelperTest
    {
        #region Test classes
#if NET
        [Serializable]
#else
        [DataContract]
#endif
        public class SerializableObject
#if NET
            : ISerializable
#endif
        {
            #region Fields
            private string _stringValue;
            private int _intValue;
            private bool _boolValue;
            private DateTime _objectValue;
            #endregion

            #region Constructors
            public SerializableObject()
            {
                // Set default values
                _stringValue = "default string";
                _intValue = 5;
                _boolValue = true;
                _objectValue = DateTime.Now;
            }
            #endregion

            #region Properties
            public string StringValue
            {
                get { return _stringValue; }
                set { _stringValue = value; }
            }

            public int IntValue
            {
                get { return _intValue; }
                set { _intValue = value; }
            }

            public bool BoolValue
            {
                get { return _boolValue; }
                set { _boolValue = value; }
            }

            public DateTime ObjectValue
            {
                get { return _objectValue; }
                set { _objectValue = value; }
            }
            #endregion

            #region Methods
            #endregion

            #region Serialization
#if NET
            public SerializableObject(SerializationInfo info, StreamingContext context)
            {
                // Read values
                _stringValue = SerializationHelper.GetString(info, "StringValue", "default string");
                _intValue = SerializationHelper.GetInt(info, "IntValue", 5);
                _boolValue = SerializationHelper.GetBool(info, "BoolValue", true);
                _objectValue = (DateTime)SerializationHelper.GetObject(info, "ObjectValue", typeof(DateTime), DateTime.Now);
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                // Store values
                info.AddValue("StringValue", _stringValue);
                info.AddValue("IntValue", _intValue);
                info.AddValue("BoolValue", _boolValue);
                info.AddValue("ObjectValue", _objectValue);
            }
#endif
            #endregion
        }
        #endregion

        [TestMethod]
        public void TestSerialization()
        {
            // Declare variables
            MemoryStream stream = null;

            // Create object
            var obj = new SerializableObject();

            // Set some values
            obj.StringValue = "SerializationHelperTest";
            obj.IntValue = 1;
            obj.BoolValue = false;
            obj.ObjectValue = DateTime.MinValue;

            // Create formatter
#if NET
            var serializer = new BinaryFormatter();
#else
            var serializer = SerializationHelper.GetDataContractSerializer(typeof (SerializableObject), obj.GetType(), "test", obj, false);
#endif

            #region Serialize to disk
            // Create stream
            using (stream = new MemoryStream())
            {
                // Serialize
#if NET
                serializer.Serialize(stream, obj);
#else
                serializer.WriteObject(stream, obj);
#endif
                #endregion

                #region Deserialize from disk
                // Reset stream position
                stream.Position = 0L;

                // Serialize
#if NET
                obj = (SerializableObject)serializer.Deserialize(stream);
#else
                obj = (SerializableObject) serializer.ReadObject(stream);
#endif
            }
            #endregion

            // Test values
            Assert.AreEqual("SerializationHelperTest", obj.StringValue);
            Assert.AreEqual(1, obj.IntValue, 1);
            Assert.AreEqual(false, obj.BoolValue);
            Assert.AreEqual(DateTime.MinValue, obj.ObjectValue);
        }

        #region DeserializeXml
        // TODO: Write unit test
        #endregion

        #region DeserializeXml
        // TODO: Write unit test
        #endregion
    }
}