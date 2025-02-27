﻿namespace Catel.Tests.MVVM.Converters
{
    using System;
    using System.Globalization;
    using NUnit.Framework;
    using Catel.ComponentModel;
    using Catel.MVVM;
    using Catel.Reflection;
    using Catel.Services;
    using Services.Fixtures;

#if !FINISHED_MIGRATION
    [TestFixture]
    public class ObjectToDisplayNameConverterFacts
    {
        [DisplayName("MyClass")]
        public class ClassWithDisplayNameClass
        {
            [DisplayName("MyField")]
            public string Field;

            [DisplayName("MyProperty")]
            public string Property { get; set; }
        }

        public class ClassWithoutDisplayNameClass
        {
            public string Field;

            public string Property { get; set; }
        }

        [DisplayName("MyEnum")]
        public enum EnumWithDisplayNameClass
        {
            [DisplayName("MyEnumValue")]
            EnumValue
        }

        public enum EnumWithoutDisplayNameClass
        {
            EnumValue
        }

        [TestCase(typeof(ClassWithDisplayNameClass), null, "My class")]
        [TestCase(typeof(ClassWithDisplayNameClass), "Field", "My field")]
        [TestCase(typeof(ClassWithDisplayNameClass), "Property", "My property")]
        [TestCase(typeof(ClassWithoutDisplayNameClass), null, "ClassWithoutDisplayNameClass")]
        [TestCase(typeof(ClassWithoutDisplayNameClass), "Field", "Field")]
        [TestCase(typeof(ClassWithoutDisplayNameClass), "Property", "Property")]
        [TestCase(typeof(EnumWithDisplayNameClass), null, "My enum")]
        [TestCase(typeof(EnumWithDisplayNameClass), "EnumValue", "My enum value")]
        [TestCase(typeof(EnumWithoutDisplayNameClass), null, "EnumWithoutDisplayNameClass")]
        [TestCase(typeof(EnumWithoutDisplayNameClass), "EnumValue", "EnumValue")]
        public void TheConvertMethod(Type type, string? memberName, string expectedValue)
        {
            var actualValue = string.Empty;

            if (string.IsNullOrWhiteSpace(memberName))
            {
                actualValue = ConvertValue(type);
                Assert.That(actualValue, Is.EqualTo(expectedValue));
                return;
            }

            var propertyInfo = type.GetPropertyEx(memberName);
            if (propertyInfo is not null)
            {
                actualValue = ConvertValue(propertyInfo);
                Assert.That(actualValue, Is.EqualTo(expectedValue));
                return;
            }

            var fieldInfo = type.GetFieldEx(memberName);
            if (fieldInfo is not null)
            {
                actualValue = ConvertValue(fieldInfo);
                Assert.That(actualValue, Is.EqualTo(expectedValue));
                return;
            }

            // This must be an enum
            var enumValue = Enum.Parse(type, memberName);
            actualValue = ConvertValue(enumValue);
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        private static string ConvertValue(object value)
        {
            var converter = new ObjectToDisplayNameConverter();
            converter.LanguageService = CreateLanguageServiceFixture();

            return converter.Convert(value, typeof(string), null, CultureInfo.CurrentCulture) as string;
        }

        private static ILanguageService CreateLanguageServiceFixture()
        {
            var languageService = new LanguageServiceFixture();
            languageService.RegisterValue("MyClass", "My class");
            languageService.RegisterValue("MyEnum", "My enum");
            languageService.RegisterValue("MyEnumValue", "My enum value");
            languageService.RegisterValue("MyField", "My field");
            languageService.RegisterValue("MyProperty", "My property");

            return languageService;
        }
    }
#endif
}
