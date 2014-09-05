// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfigurationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SILVERLIGHT

namespace Catel.Test.Configuration
{
    using System.IO;
    using System.Text;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.IO;
    using NUnit.Framework;

    [TestFixture, Explicit]
    public class DynamicConfigurationFacts
    {
        private const string ExpectedXml = "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
"<DynamicConfiguration graphid=\"1\" xmlns:ctl=\"http://catel.codeplex.com\">\r\n" +
"  <KeyX>Value X</KeyX>\r\n" +
"  <KeyY>Value Y</KeyY>\r\n" +
"  <KeyZ.SomeAddition>Value Z</KeyZ.SomeAddition>\r\n" +
"</DynamicConfiguration>";

        [TestCase]
        public void RegistersPropertiesFromSerialization()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(ExpectedXml);
                    streamWriter.Flush();

                    memoryStream.Position = 0L;

                    var configuration = DynamicConfiguration.Load<DynamicConfiguration>(memoryStream, SerializationMode.Xml);

                    Assert.IsTrue(configuration.IsConfigurationKeyAvailable("KeyX"));
                    Assert.IsTrue(configuration.IsConfigurationKeyAvailable("KeyY"));

                    Assert.AreEqual("Value X", configuration.GetConfigurationValue("KeyX"));
                    Assert.AreEqual("Value Y", configuration.GetConfigurationValue("KeyY"));
                }
            }
        }

        [TestCase]
        public void CorrectlySerializesConfiguration()
        {
            var dynamicConfiguration = new DynamicConfiguration();
            dynamicConfiguration.SetConfigurationValue("KeyX", "Value X");
            dynamicConfiguration.SetConfigurationValue("KeyY", "Value Y");
            dynamicConfiguration.SetConfigurationValue("KeyZ.SomeAddition", "Value Z");

            using (var memoryStream = new MemoryStream())
            {
                dynamicConfiguration.SaveAsXml(memoryStream);

                var outputXml = memoryStream.GetUtf8String();

                Assert.AreEqual(ExpectedXml, outputXml);
            }
        }
    }
}

#endif