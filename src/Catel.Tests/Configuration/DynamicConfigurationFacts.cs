namespace Catel.Tests.Configuration
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.IO;
    using NUnit.Framework;

    using static VerifyNUnit.Verifier;
    using System.Threading.Tasks;

    [TestFixture, Explicit]
    public class DynamicConfigurationFacts
    {
        private const string ExpectedXml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<DynamicConfiguration xmlns:ctl=""http://schemas.catelproject.com"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
  <KeyX ctl:type=""System.String"">Value X</KeyX>
  <KeyY ctl:type=""System.String"">Value Y</KeyY>
  <KeyZ.SomeAddition ctl:type=""System.String"">Value Z</KeyZ.SomeAddition>
</DynamicConfiguration>";

        public class ComplexSetting : ModelBase
        {
            public ComplexSetting()
                : base()
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
            public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>("FirstName", string.Empty);

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
            public static readonly IPropertyData MiddleNameProperty = RegisterProperty<string>("MiddleName", string.Empty);

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
            public static readonly IPropertyData LastNameProperty = RegisterProperty<string>("LastName", string.Empty);
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

                    var configuration = SavableModelBase<DynamicConfiguration>.Load(memoryStream, SerializationFactory.GetXmlSerializer());

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
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    streamWriter.Write(ExpectedXml);
                    streamWriter.Flush();

                    memoryStream.Position = 0L;

                    var configuration = SavableModelBase<DynamicConfiguration>.Load(memoryStream, SerializationFactory.GetXmlSerializer());

                    Assert.IsTrue(configuration.IsConfigurationValueSet("KeyX"));
                    Assert.IsTrue(configuration.IsConfigurationValueSet("KeyY"));

                    Assert.AreEqual("Value X", configuration.GetConfigurationValue("KeyX"));
                    Assert.AreEqual("Value Y", configuration.GetConfigurationValue("KeyY"));
                }
            }
        }

        [Test, Explicit]
        public async Task CorrectlySerializesConfigurationAsync()
        {
            var dynamicConfiguration = new DynamicConfiguration();
            dynamicConfiguration.SetConfigurationValue("KeyX", "Value X");
            dynamicConfiguration.SetConfigurationValue("KeyY", "Value Y");
            dynamicConfiguration.SetConfigurationValue("KeyZ.SomeAddition", "Value Z");

            using (var memoryStream = new MemoryStream())
            {
                dynamicConfiguration.SaveAsXml(memoryStream);

                var outputXml = memoryStream.GetUtf8String();

                await Verify(outputXml);
            }
        }

        [TestCase]
        public void CorrectlySerializesComplexObjects()
        {
            var dynamicConfiguration = new DynamicConfiguration();

            var complexSetting = new ComplexSetting
            {
                FirstName = "John",
                MiddleName = "",
                LastName = "Doe"
            };

            dynamicConfiguration.SetConfigurationValue("ComplexSetting", complexSetting);

            using (var memoryStream = new MemoryStream())
            {
                dynamicConfiguration.SaveAsXml(memoryStream);

                memoryStream.Position = 0L;

                var newDynamicConfiguration = SavableModelBase<DynamicConfiguration>.Load(memoryStream, SerializationFactory.GetXmlSerializer());
                var newComplexSetting = newDynamicConfiguration.GetConfigurationValue<ComplexSetting>("ComplexSetting", null);

                Assert.AreEqual(newComplexSetting.FirstName, complexSetting.FirstName);
                Assert.AreEqual(newComplexSetting.MiddleName, complexSetting.MiddleName);
                Assert.AreEqual(newComplexSetting.LastName, complexSetting.LastName);
            }
        }
    }
}

