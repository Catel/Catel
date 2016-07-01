// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfigurationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SILVERLIGHT

namespace Catel.Test.Configuration
{
    using System.IO;
    using System.Runtime.Serialization;
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
"  <ComplexSetting IsNull=\"true\" />\r\n" +
"  <KeyX type=\"System.String\">Value X</KeyX>\r\n" +
"  <KeyY type=\"System.String\">Value Y</KeyY>\r\n" +
"  <KeyZ.SomeAddition type=\"System.String\">Value Z</KeyZ.SomeAddition>\r\n" +
"</DynamicConfiguration>";

        public class ComplexSetting : ModelBase
        {
            public ComplexSetting()
                : base()
            {
                
            }

            public ComplexSetting(SerializationInfo info, StreamingContext context) 
                : base(info, context)
            {
            }

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            /// <summary>
            /// Register the FirstName property so it is known in the class.
            /// </summary>
            public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public string MiddleName
            {
                get { return GetValue<string>(MiddleNameProperty); }
                set { SetValue(MiddleNameProperty, value); }
            }

            /// <summary>
            /// Register the MiddleName property so it is known in the class.
            /// </summary>
            public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof(string), string.Empty);

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public string LastName
            {
                get { return GetValue<string>(LastNameProperty); }
                set { SetValue(LastNameProperty, value); }
            }

            /// <summary>
            /// Register the LastName property so it is known in the class.
            /// </summary>
            public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), string.Empty);
        }

        [TestCase]
        public void KnowsWhatPropertiesAreSetUsingSetConfigurationValue()
        {
            var configuration = new DynamicConfiguration();

            configuration.SetConfigurationValue("A", "1");
            configuration.SetConfigurationValue("B", "2");

            Assert.IsTrue(configuration.IsConfigurationValueSet("A"));
            Assert.IsTrue(configuration.IsConfigurationValueSet("B"));
            Assert.IsFalse(configuration.IsConfigurationValueSet("C"));
        }

        [TestCase]
        public void KnowsWhatPropertiesAreSetUsingDeserialization()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(ExpectedXml);
                    streamWriter.Flush();

                    memoryStream.Position = 0L;

                    var configuration = ModelBase.Load<DynamicConfiguration>(memoryStream, SerializationMode.Xml);

                    Assert.IsTrue(configuration.IsConfigurationValueSet("KeyX"));
                    Assert.IsTrue(configuration.IsConfigurationValueSet("KeyY"));
                    Assert.IsFalse(configuration.IsConfigurationValueSet("C"));
                }
            }
        }

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

                    var configuration = ModelBase.Load<DynamicConfiguration>(memoryStream, SerializationMode.Xml);

                    Assert.IsTrue(configuration.IsConfigurationValueSet("KeyX"));
                    Assert.IsTrue(configuration.IsConfigurationValueSet("KeyY"));

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

        [TestCase]
        public void CorrectlySerializesComplexObjects()
        {
            var dynamicConfiguration = new DynamicConfiguration();

            var complexSetting = new ComplexSetting
            {
                FirstName = "Geert",
                MiddleName = "van",
                LastName = "Horrik"
            };

            dynamicConfiguration.SetConfigurationValue("ComplexSetting", complexSetting);

            using (var memoryStream = new MemoryStream())
            {
                dynamicConfiguration.SaveAsXml(memoryStream);

                memoryStream.Position = 0L;

                var newDynamicConfiguration = ModelBase.Load<DynamicConfiguration>(memoryStream, SerializationMode.Xml);
                var newComplexSetting = newDynamicConfiguration.GetConfigurationValue<ComplexSetting>("ComplexSetting", null);

                Assert.AreEqual(newComplexSetting.FirstName, complexSetting.FirstName);
                Assert.AreEqual(newComplexSetting.MiddleName, complexSetting.MiddleName);
                Assert.AreEqual(newComplexSetting.LastName, complexSetting.LastName);
            }
        }
    }
}

#endif