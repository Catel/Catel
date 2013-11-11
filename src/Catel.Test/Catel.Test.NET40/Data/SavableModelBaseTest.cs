// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableModelBaseTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
        #region Serialization tests
#if NET
        [TestMethod]
        public void BinarySerializationLevel1()
        {
            var originalObject = ModelBaseTestHelper.CreateIniEntryObject();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Binary);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void BinarySerializationLevel2()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Binary);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void BinarySerializationLevel3()
        {
            var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Binary);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void BinarySerializationComplexGraphWithInheritance()
        {
            var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Binary);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void BinarySerializationWithPrivateMembers()
        {
            // Create new object
            var originalObject = new ObjectWithPrivateMembers("My private member");
            originalObject.PublicMember = "My public member";

            // Test
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Binary);

            Assert.AreEqual(originalObject, clonedObject);
        }
#endif

        [TestMethod]
        public void XmlSerializationLevel1()
        {
            var originalObject = ModelBaseTestHelper.CreateIniEntryObject();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void XmlSerializationLevel2()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void XmlSerializationLevel3()
        {
            var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void XmlSerializationComplexGraphWithInheritance()
        {
            var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, clonedObject);
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
            var originalObject = ModelBaseTestHelper.CreateComputerSettingsWithXmlMappingsObject();
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, clonedObject);
        }

        [TestMethod]
        public void XmlSerializationWithCustomTypes()
        {
            // Create object
            var originalObject = new ObjectWithCustomType();
            originalObject.FirstName = "Test";
            originalObject.Gender = Gender.Female;

            // Serialize and deserialize
            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, clonedObject);
        }

#if NET
        [TestMethod]
        public void XmlSerializationWithPrivateMembers()
        {
            var originalObject = new ObjectWithPrivateMembers("My private member");
            originalObject.PublicMember = "My public member";

            var clonedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, clonedObject);
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
#if NET
        [TestMethod]
        public void Load_FileName_SerializationMode_Binary()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Binary);

            Assert.AreEqual(originalObject, loadedObject);
        }
#endif

        [TestMethod]
        public void Load_FileName_SerializationMode_Xml()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserializeObject(originalObject, SerializationMode.Xml);

            Assert.AreEqual(originalObject, loadedObject);
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