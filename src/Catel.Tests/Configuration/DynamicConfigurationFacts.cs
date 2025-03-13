namespace Catel.Tests.Configuration
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.IO;
    using Catel.Runtime.Serialization;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using static VerifyNUnit.Verifier;

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
            public ComplexSetting(ISerializer serializer)
                : base(serializer)
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

            Assert.That(configuration.IsConfigurationValueSet("A"), Is.True);
            Assert.That(configuration.IsConfigurationValueSet("B"), Is.True);
            Assert.That(configuration.IsConfigurationValueSet("C"), Is.False);
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

                    Assert.That(configuration.IsConfigurationValueSet("KeyX"), Is.True);
                    Assert.That(configuration.IsConfigurationValueSet("KeyY"), Is.True);
                    Assert.That(configuration.IsConfigurationValueSet("C"), Is.False);
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

                    Assert.That(configuration.IsConfigurationValueSet("KeyX"), Is.True);
                    Assert.That(configuration.IsConfigurationValueSet("KeyY"), Is.True);

                    Assert.That(configuration.GetConfigurationValue("KeyX"), Is.EqualTo("Value X"));
                    Assert.That(configuration.GetConfigurationValue("KeyY"), Is.EqualTo("Value Y"));
                }
            }
        }

        [Test, Explicit]
        public async Task CorrectlySerializesConfigurationAsync()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddCatelSerializationJsonServices();

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var serializer = serviceProvider.GetRequiredService<ISerializer>();

                var dynamicConfiguration = new DynamicConfiguration(serializer);
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
        }

        [TestCase]
        public void CorrectlySerializesComplexObjects()
        {
            var dynamicConfiguration = new DynamicConfiguration();

            var complexSetting = new ComplexSetting
            {
                FirstName = "John",
                MiddleName = string.Empty,
                LastName = "Doe"
            };

            dynamicConfiguration.SetConfigurationValue("ComplexSetting", complexSetting);

            using (var memoryStream = new MemoryStream())
            {
                dynamicConfiguration.SaveAsXml(memoryStream);

                memoryStream.Position = 0L;

                var newDynamicConfiguration = SavableModelBase<DynamicConfiguration>.Load(memoryStream, SerializationFactory.GetXmlSerializer());
                var newComplexSetting = newDynamicConfiguration.GetConfigurationValue<ComplexSetting>("ComplexSetting", null);

                Assert.That(complexSetting.FirstName, Is.EqualTo(newComplexSetting.FirstName));
                Assert.That(complexSetting.MiddleName, Is.EqualTo(newComplexSetting.MiddleName));
                Assert.That(complexSetting.LastName, Is.EqualTo(newComplexSetting.LastName));
            }
        }
    }
}

