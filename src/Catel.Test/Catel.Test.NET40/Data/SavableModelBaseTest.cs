// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableModelBaseTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.Test.Runtime.Serialization;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class SavableModelBaseTest
    {
        #region Generic Loads
#if NET
        [TestMethod]
        public void Load_FileName_SerializationMode_Binary()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

            Assert.AreEqual(originalObject, loadedObject);
        }
#endif

        [TestMethod]
        public void Load_FileName_SerializationMode_Xml()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

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