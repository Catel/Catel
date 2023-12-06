namespace Catel.Tests.Data
{
    using Catel.Data;

    using NUnit.Framework;

    [TestFixture]
    public class CatelTypeInfoFacts
    {
        public class CatelTypeInfoTestModel : ModelBase
        {
            public string NormalProperty { get; set; }

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public string CatelProperty
            {
                get { return GetValue<string>(CatelPropertyProperty); }
                set { SetValue(CatelPropertyProperty, value); }
            }

            /// <summary>
            /// Register the CatelProperty property so it is known in the class.
            /// </summary>
            public static readonly IPropertyData CatelPropertyProperty = RegisterProperty("CatelProperty", typeof(string), null);
        }

        [TestCase]
        public void CorrectlyRegistersCatelProperties()
        {
            var catelTypeInfo = new CatelTypeInfo(typeof(CatelTypeInfoTestModel));

            var properties = catelTypeInfo.GetCatelProperties();
            Assert.That(properties.Count, Is.Not.EqualTo(0));
            Assert.That(properties.Keys.Contains("CatelProperty"), Is.True);
        }

        [TestCase]
        public void CorrectlyRegistersNonCatelProperties()
        {
            var catelTypeInfo = new CatelTypeInfo(typeof(CatelTypeInfoTestModel));

            var properties = catelTypeInfo.GetNonCatelProperties();
            Assert.That(properties.Count, Is.Not.EqualTo(0));
            Assert.That(properties.Keys.Contains("NormalProperty"), Is.True);
        }
    }
}
