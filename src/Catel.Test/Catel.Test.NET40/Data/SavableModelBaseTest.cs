// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableModelBaseTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class SavableModelBaseTest
    {
        #region Fields
#if !NETFX_CORE
        private static FilesHelper _filesHelper;
#endif
        #endregion

        #region Initialization and cleanup
#if !NETFX_CORE
        [TestInitialize]
        public void Initialize()
        {
            if (_filesHelper == null)
            {
                _filesHelper = new FilesHelper();
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (_filesHelper != null)
            {
                _filesHelper.CleanUp();
                _filesHelper = null;
            }
        }
#endif
        #endregion

        #region Serialization tests
        /// <summary>
        /// Serializes and deserializes an object using the specified mode. Finally, it will check whether the original object is equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testObject">The test object.</param>
        /// <param name="mode">The mode.</param>
        public static void SerializeAndDeserializeObject<T>(T testObject, SerializationMode mode)
            where T : SavableModelBase<T>
        {
            if (_filesHelper == null)
            {
                _filesHelper = new FilesHelper();
            }

            var file = _filesHelper.GetTempFile();
            testObject.Save(file, mode);

#if SILVERLIGHT
            file.Position = 0L;
#endif
            var loadedObject = SavableModelBase<T>.Load(file, mode);

            Assert.AreEqual(testObject, loadedObject);
        }

#if NET
        [TestMethod]
        public void BinarySerializationLevel1()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateIniEntryObject(), SerializationMode.Binary);
        }

        [TestMethod]
        public void BinarySerializationLevel2()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateIniFileObject(), SerializationMode.Binary);
        }

        [TestMethod]
        public void BinarySerializationLevel3()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateComputerSettingsObject(), SerializationMode.Binary);
        }

        [TestMethod]
        public void BinarySerializationComplexGraphWithInheritance()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance(), SerializationMode.Binary);
        }

        [TestMethod]
        public void BinarySerializationWithPrivateMembers()
        {
            // Create new object
            var objectWithPrivateMembers = new ObjectWithPrivateMembers("My private member");
            objectWithPrivateMembers.PublicMember = "My public member";

            // Test
            SerializeAndDeserializeObject(objectWithPrivateMembers, SerializationMode.Binary);
        }

        [TestMethod]
        public void BinarySerializationWithPrivateParameterlessConstructor()
        {
            // Create new object
            var objectWithPrivateConstructor = new ObjectWithPrivateConstructor("My private constructor test");

            // Test
            SerializeAndDeserializeObject(objectWithPrivateConstructor, SerializationMode.Binary);
        }
#endif

        [TestMethod]
        public void XmlSerializationLevel1()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateIniEntryObject(), SerializationMode.Xml);
        }

        [TestMethod]
        public void XmlSerializationLevel2()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateIniFileObject(), SerializationMode.Xml);
        }

        [TestMethod]
        public void XmlSerializationLevel3()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateComputerSettingsObject(), SerializationMode.Xml);
        }

        [TestMethod]
        public void XmlSerializationComplexGraphWithInheritance()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance(), SerializationMode.Xml);
        }

        [TestMethod]
        public void XmlSerializationWithXmlIgnore()
        {
            var obj = new ObjectWithXmlMappings();

            string xml = obj.ToXml().ToString();

            Assert.IsFalse(xml.Contains("IgnoredProperty"));
        }

        [TestMethod]
        public void XmlSerializationWithXmlMappings()
        {
            SerializeAndDeserializeObject(ModelBaseTestHelper.CreateComputerSettingsWithXmlMappingsObject(), SerializationMode.Xml);
        }

        [TestMethod]
        public void XmlSerializationWithCustomTypes()
        {
            // Create object
            var obj = new ObjectWithCustomType();
            obj.FirstName = "Test";
            obj.Gender = Gender.Female;

            // Serialize and deserialize
            SerializeAndDeserializeObject(obj, SerializationMode.Xml);
        }

        [TestMethod]
        public void XmlSerialization_FixedCheck()
        {
            const string ExpectedXmlContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                              @"
<IniFile xmlns:ctl=" + "\"http://catel.codeplex.com\">" + @"
  <FileName>MyIniFile</FileName>
  <IniEntryCollection xmlns:d1p1=" + "\"http://schemas.datacontract.org/2004/07/Catel.Test.Data\"" + " xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"" + @">
    <d1p1:IniEntry>
      <Group>Group 0</Group>
      <Key>Key 0</Key>
      <Value>Value 0</Value>
      <IniEntryType>Public</IniEntryType>
    </d1p1:IniEntry>
    <d1p1:IniEntry>
      <Group>Group 1</Group>
      <Key>Key 1</Key>
      <Value>Value 1</Value>
      <IniEntryType>Private</IniEntryType>
    </d1p1:IniEntry>
    <d1p1:IniEntry>
      <Group>Group 2</Group>
      <Key>Key 2</Key>
      <Value>Value 2</Value>
      <IniEntryType>Public</IniEntryType>
    </d1p1:IniEntry>
  </IniEntryCollection>
</IniFile>";

            IniFile iniFile = ModelBaseTestHelper.CreateIniFileObject();
            using (var memoryStream = new MemoryStream())
            {
                iniFile.Save(memoryStream, SerializationMode.Xml);

                memoryStream.Position = 0L;

                TextReader reader = new StreamReader(memoryStream);
                string xmlContent = reader.ReadToEnd();

                Assert.AreEqual(ExpectedXmlContent, xmlContent);
            }
        }

#if NET
        [TestMethod]
        public void XmlSerializationWithPrivateMembers()
        {
            // Create new object
            var objectWithPrivateMembers = new ObjectWithPrivateMembers("My private member");
            objectWithPrivateMembers.PublicMember = "My public member";

            // Test
            SerializeAndDeserializeObject(objectWithPrivateMembers, SerializationMode.Xml);
        }

        [TestMethod]
        public void XmlSerializationWithPrivateParameterlessConstructor()
        {
            // Create new object
            var objectWithPrivateConstructor = new ObjectWithPrivateConstructor("My private constructor test");

            // Test
            SerializeAndDeserializeObject(objectWithPrivateConstructor, SerializationMode.Xml);
        }
#endif

        [TestMethod]
        public void ReadXml()
        {
            // Should always return null
            var iniFile = ModelBaseTestHelper.CreateIniFileObject();
            Assert.AreEqual(null, ((IXmlSerializable) iniFile).GetSchema());
        }
        #endregion

        #region Generic Loads
        [TestMethod]
        public void Load_FileName()
        {
            var file = _filesHelper.GetTempFile();
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            originalObject.Save(file);

#if SILVERLIGHT
            file.Position = 0L;
#endif
            var loadedObject = IniFile.Load(file);

            Assert.AreEqual(originalObject, loadedObject);
        }

        [TestMethod]
        public void Load_FileName_EnableRedirects()
        {
            var file = _filesHelper.GetTempFile();
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            originalObject.Save(file);

#if SILVERLIGHT
            file.Position = 0L;
#endif
            var loadedObject = IniFile.Load(file, true);

            Assert.AreEqual(originalObject, loadedObject);
        }

#if NET
        [TestMethod]
        public void Load_FileName_SerializationMode_Binary()
        {
            var file = _filesHelper.GetTempFile();
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            originalObject.Save(file, SerializationMode.Binary);

            var loadedObject = IniFile.Load(file, SerializationMode.Binary);

            Assert.AreEqual(originalObject, loadedObject);
        }
#endif

        [TestMethod]
        public void Load_FileName_SerializationMode_Xml()
        {
            var file = _filesHelper.GetTempFile();
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            originalObject.Save(file, SerializationMode.Xml);

#if SILVERLIGHT
            file.Position = 0L;
#endif
            var loadedObject = IniFile.Load(file, SerializationMode.Xml);

            Assert.AreEqual(originalObject, loadedObject);
        }

#if NET
        [TestMethod]
        public void Load_FileName_SerializationMode_Binary_EnableRedirects()
        {
            var file = _filesHelper.GetTempFile();
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            originalObject.Save(file, SerializationMode.Binary);

            var loadedObject = IniFile.Load(file, SerializationMode.Binary, true);

            Assert.AreEqual(originalObject, loadedObject);
        }
#endif

        [TestMethod]
        public void Load_FileName_SerializationMode_Xml_EnableRedirects()
        {
            var file = _filesHelper.GetTempFile();
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            originalObject.Save(file, SerializationMode.Xml);

#if SILVERLIGHT
            file.Position = 0L;
#endif
            var loadedObject = IniFile.Load(file, SerializationMode.Xml, true);

            Assert.AreEqual(originalObject, loadedObject);
        }

        [TestMethod]
        public void Load_XDocument()
        {
            var file = _filesHelper.GetTempFile();
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            originalObject.Save(file, SerializationMode.Xml);

#if SILVERLIGHT
            file.Position = 0L;
#endif
            XDocument document = XDocument.Load(file);
            var loadedObject = IniFile.Load(document);

            Assert.AreEqual(originalObject, loadedObject);
        }

        [TestMethod]
        public void Load_Bytes()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            byte[] originalBytes = originalObject.Bytes;

            var loadedObject = IniFile.Load(originalBytes);

            Assert.AreEqual(originalObject, loadedObject);
        }

        [TestMethod]
        public void Load_Bytes_EnableRedirects()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            byte[] originalBytes = originalObject.Bytes;

            var loadedObject = IniFile.Load(originalBytes, true);

            Assert.AreEqual(originalObject, loadedObject);
        }

        [TestMethod]
        public void Load_Stream()
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }

        [TestMethod]
        public void Load_Stream_EnableRedirects()
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream, true);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }

#if NET
        [TestMethod]
        public void Load_Stream_SerializationMode_Binary()
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream, SerializationMode.Binary);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream, SerializationMode.Binary);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }
#endif

        [TestMethod]
        public void Load_Stream_SerializationMode_Xml()
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream, SerializationMode.Xml);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream, SerializationMode.Xml);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }

#if NET
        [TestMethod]
        public void Load_Stream_SerializationMode_Binary_EnableRedirects()
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream, SerializationMode.Binary);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream, SerializationMode.Binary, true);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }
#endif

        [TestMethod]
        public void Load_Stream_SerializationMode_Xml_EnableRedirects()
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream, SerializationMode.Xml);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream, SerializationMode.Xml, true);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }
        #endregion

        [TestMethod]
        public void ToXml()
        {
            var iniFile = ModelBaseTestHelper.CreateIniFileObject();

            XDocument document = iniFile.ToXml();
            var loadedObject = IniFile.Load(document);

            Assert.AreEqual(iniFile, loadedObject);
        }
    }
}