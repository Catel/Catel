// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableModelBaseTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.IO;
    using Catel.Runtime.Serialization;
    using Catel.Test.Runtime.Serialization;

    using NUnit.Framework;

    [TestFixture]
    public class SavableModelBaseTest
    {
        #region Generic Loads
#if NET
        [TestCase]
        public void Load_FileName_SerializationMode_Binary()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

            Assert.AreEqual(originalObject, loadedObject);
        }
#endif

        [TestCase]
        public void Load_FileName_SerializationMode_Xml()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

            Assert.AreEqual(originalObject, loadedObject);
        }

#if NET
        [TestCase]
        public void Load_Stream_SerializationMode_Binary()
        {
            using (var memoryStream = new MemoryStream())
            {
                var serializer = SerializationFactory.GetBinarySerializer();

                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream, serializer, null);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream, serializer, null);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }
#endif

        [TestCase]
        public void Load_Stream_SerializationMode_Xml()
        {
            using (var memoryStream = new MemoryStream())
            {
                var serializer = SerializationFactory.GetBinarySerializer();

                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                originalObject.Save(memoryStream, serializer, null);

                memoryStream.Position = 0L;
                var loadedObject = IniFile.Load(memoryStream, serializer, null);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }
        #endregion
    }
}