namespace Catel.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel.Data;
    using Catel.Reflection;

    using NUnit.Framework;

    public class TypeHelperFacts
    {
        #region Test classes
        private class PropertyInfoTestClass
        {
            #region Properties
            private bool PrivateProperty { get; set; }

            public bool PublicProperty { get; set; }
            #endregion
        }
        #endregion

        #region Nested type: TheConvertTypeToVersionIndependentTypeMethod
        [TestFixture]
        public class TheConvertTypeToVersionIndependentTypeMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForNullOrEmptyType(string? type)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.ConvertTypeToVersionIndependentType(type));
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForSimpleVersionIndependentType()
            {
                const string input = "Catel.Data.ObservableObject, Catel.Core";
                const string output = "Catel.Data.ObservableObject, Catel.Core";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.That(realOutput, Is.EqualTo(output));
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForComplexVersionIndependentType()
            {
                string input = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";
                const string output = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.That(realOutput, Is.EqualTo(output));
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForSimpleVersionDependentType()
            {
                string input = typeof(ObservableObject).AssemblyQualifiedName;
                const string output = "Catel.Data.ObservableObject, Catel.Core";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.That(realOutput, Is.EqualTo(output));
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForComplexVersionDependentType()
            {
                // Use a different key token because MS key tokens are not changed
                string input = "System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx";
                const string output = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.That(realOutput, Is.EqualTo(output));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheFormatInnerTypesMethod
        [TestFixture]
        public class TheFormatInnerTypesMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsEmptyStringForEmptyArray()
            {
                Assert.That(TypeHelper.FormatInnerTypes((IEnumerable<string>)Array.Empty<string>()), Is.EqualTo(string.Empty));
            }

            [TestCase]
            public void ReturnsArrayOfTypesForFilledArray()
            {
                string expectedValue = "[string],[string],[int]";

                string actualValue = TypeHelper.FormatInnerTypes((IEnumerable<string>)new[] { "string", "string", "int" });

                Assert.That(actualValue, Is.EqualTo(expectedValue));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheFormatTypeMethod
        [TestFixture]
        public class TheFormatTypeMethod
        {
            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForNullOrEmptyAssemblyName(string? assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.FormatType(assemblyName, "Type"));
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForNullOrEmptyTypeName(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.FormatType("Catel.Core", typeName));
            }

            [TestCase]
            public void ReturnsFormattedType()
            {
                string expectedValue = "Catel.Tests.Helpers.TypeHelperFacts, Catel.Tests";

                string actualValue = TypeHelper.FormatType("Catel.Tests", "Catel.Tests.Helpers.TypeHelperFacts");

                Assert.That(actualValue, Is.EqualTo(expectedValue));
            }
        }
        #endregion

        #region Nested type: TheGetAssemblyNameMethod
        [TestFixture]
        public class TheGetAssemblyNameMethod
        {
            [TestCase(null)]
            [TestCase("")]
            public void ThrowsArgumentExceptionForNullFullTypeName(string? assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetAssemblyName(assemblyName));
            }

            [TestCase("Catel.Tests.TypeHelper", null)]
            [TestCase("Catel.Tests.TypeHelper, Catel.Core", "Catel.Core")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]]", null)]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib", "mscorlib")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
            public void ReturnsAssemblyName(string typeName, string? expectedAssembly)
            {
                Assert.That(TypeHelper.GetAssemblyName(typeName), Is.EqualTo(expectedAssembly));
            }
        }
        #endregion

        #region Nested type: TheGetAssemblyNameWithoutOverheadMethod
        [TestFixture]
        public class TheGetAssemblyNameWithoutOverheadMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForNullOrEmptyAssemblyName(string? assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetAssemblyNameWithoutOverhead(assemblyName));
            }

            [TestCase]
            public void ReturnsAssemblyNameForAssemblyWithoutOverhead()
            {
                var input = "Catel.Core";
                var expected = "Catel.Core";

                Assert.That(TypeHelper.GetAssemblyNameWithoutOverhead(input), Is.EqualTo(expected));
            }

            [TestCase]
            public void ReturnsAssemblyNameForAssemblyWithOverhead()
            {
                var input = "Catel.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1c8163524cbe02e6";
                var expected = "Catel.Core";

                Assert.That(TypeHelper.GetAssemblyNameWithoutOverhead(input), Is.EqualTo(expected));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetInnerTypesMethod
        [TestFixture]
        public class TheGetInnerTypesMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForNullOrEmptyType(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetInnerTypes(typeName));
            }

            [TestCase]
            public void ReturnsEmptyArrayForNonGenericType()
            {
                var input = "Catel.Data.ObservableObject, Catel.Core";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.That(output.Length, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsInnerTypesForSimpleGenericType()
            {
                var input = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx], [System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.That(output.Length, Is.EqualTo(2));
                Assert.That(output[0], Is.EqualTo("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx"));
                Assert.That(output[1], Is.EqualTo("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx"));
            }

            [TestCase]
            public void ReturnsInnerTypesForComplexGenericType()
            {
                var input = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx], [System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.That(output.Length, Is.EqualTo(2));
                Assert.That(output[0], Is.EqualTo("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx"));
                Assert.That(output[1], Is.EqualTo("System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeNameMethod
        [TestFixture]
        public class TheGetTypeNameMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            public void ThrowsArgumentExceptionForNullFullTypeName(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeName(typeName));
            }

            [TestCase("Catel.Tests.TypeHelper", "Catel.Tests.TypeHelper")]
            [TestCase("Catel.Tests.TypeHelper, Catel.Core", "Catel.Tests.TypeHelper")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib", "System.Collections.Generic.List`1[[Catel.Data.PropertyValue]]")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Collections.Generic.List`1[[Catel.Data.PropertyValue]]")]
            public void ReturnsTypeName(string input, string expectedOutput)
            {
                Assert.That(TypeHelper.GetTypeName(input), Is.EqualTo(expectedOutput));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeNameWithAssemblyMethod
        [TestFixture]
        public class TheGetTypeNameWithAssemblyMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            public void ThrowsArgumentExceptionForNullFullTypeName(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeNameWithAssembly(typeName));
            }

            [TestCase]
            public void ReturnsFullTypeNameForTypeWithAssemblyWithoutOverhead()
            {
                Assert.That(TypeHelper.GetTypeNameWithAssembly("Catel.Tests.TypeHelper, Catel.Core"), Is.EqualTo("Catel.Tests.TypeHelper, Catel.Core"));
            }

            [TestCase]
            public void ReturnsFullTypeNameForTypeWithAssembly()
            {
                Assert.That(TypeHelper.GetTypeNameWithAssembly("Catel.Tests.TypeHelper, Catel.Core, Version=1.0.0.0, PublicKeyToken=1234578, Culture=neutral"), Is.EqualTo("Catel.Tests.TypeHelper, Catel.Core"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeNameWithoutNamespaceMethod
        [TestFixture]
        public class TheGetTypeNameWithoutNamespaceMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            public void ThrowsArgumentExceptionForNullFullTypeName(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeNameWithoutNamespace(typeName));
            }

            [TestCase]
            public void ReturnsTypeNameForTypeWithoutAssembly()
            {
                Assert.That(TypeHelper.GetTypeNameWithoutNamespace("Catel.Tests.TypeHelper"), Is.EqualTo("TypeHelper"));
            }

            [TestCase]
            public void ReturnsTypeNameForTypeWithAssembly()
            {
                Assert.That(TypeHelper.GetTypeNameWithoutNamespace("Catel.Tests.TypeHelper, Catel.Core"), Is.EqualTo("TypeHelper"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeNamespaceMethod
        [TestFixture]
        public class TheGetTypeNamespaceMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            public void ThrowsArgumentExceptionForNullFullTypeName(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeNamespace(typeName));
            }

            [TestCase]
            public void ReturnsTypeNamespaceForTypeWithoutAssembly()
            {
                Assert.That(TypeHelper.GetTypeNamespace("Catel.Tests.TypeHelper"), Is.EqualTo("Catel.Tests"));
            }

            [TestCase]
            public void ReturnsTypeNamespaceForTypeWithAssembly()
            {
                Assert.That(TypeHelper.GetTypeNamespace("Catel.Tests.TypeHelper, Catel.Core"), Is.EqualTo("Catel.Tests"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeWithAssemblyMethod
        [TestFixture]
        public class TheGetTypeWithAssemblyMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForInvalidAssemblyName(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeCache.GetTypeWithAssembly(typeName, "Catel.Core"));
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForInvalidTypeName(string? assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", assemblyName));
            }

            [TestCase]
            public void ReturnsNullForUnavailableType()
            {
                Assert.That(TypeCache.GetTypeWithAssembly("Catel.UnknownType", "Catel.Core"), Is.Null);
            }

            [TestCase]
            public void ReturnsTypeForAvailableType()
            {
                Assert.That(TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", "Catel.Core"), Is.EqualTo(typeof(TypeHelper)));
            }

            [TestCase]
            public void ReturnsNullWithoutIgnoringCase()
            {
                Assert.That(TypeCache.GetTypeWithAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", "cAtEl.CoRe", false), Is.EqualTo(null));
            }

            [TestCase]
            public void ReturnsTypeWithIgnoringCase()
            {
                Assert.That(TypeCache.GetTypeWithAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", "cAtEl.CoRe", true), Is.EqualTo(typeof(TypeHelper)));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeWithoutAssemblyMethod
        [TestFixture]
        public class TheGetTypeWithoutAssemblyMethod
        {
            #region Methods
            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForInvalidTypeName(string? typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeCache.GetTypeWithoutAssembly(typeName));
            }

            [TestCase]
            public void ReturnsNullForUnavailableType()
            {
                Assert.That(TypeCache.GetTypeWithoutAssembly("Catel.UnknownType"), Is.Null);
            }

            [TestCase]
            public void ReturnsTypeForAvailableType()
            {
                Assert.That(TypeCache.GetTypeWithoutAssembly("Catel.Reflection.TypeHelper"), Is.EqualTo(typeof(TypeHelper)));
            }

            [TestCase]
            public void ReturnsNullWithoutIgnoringCase()
            {
                Assert.That(TypeCache.GetTypeWithoutAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", false), Is.EqualTo(null));
            }

            [TestCase]
            public void ReturnsTypeWithIgnoringCase()
            {
                Assert.That(TypeCache.GetTypeWithoutAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", true), Is.EqualTo(typeof(TypeHelper)));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypedInstanceMethod
        [TestFixture]
        public class TheGetTypedInstanceMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsNullForNullInstance()
            {
                Assert.That(TypeHelper.GetTypedInstance<List<int>>(null), Is.EqualTo(null));
            }

            [TestCase]
            public void ThrowsNotSupportedExceptionForInvalidCast()
            {
                var list = new List<int>();
                Assert.Throws<NotSupportedException>(() => TypeHelper.GetTypedInstance<List<string>>(list));
            }

            [TestCase]
            public void ReturnsTypedInstanceWhenInstanceIsOfType()
            {
                var list = new List<int>();
                Assert.That(TypeHelper.GetTypedInstance<List<int>>(list), Is.EqualTo(list));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypesMethod
        [TestFixture]
        public class TheGetTypesMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsAllTypesThatMatchThePredicate()
            {
                var allTypes = TypeCache.GetTypes();
                var notifyTypes = TypeCache.GetTypes(t => t.ImplementsInterfaceEx<INotifyPropertyChanged>());

                Assert.That(notifyTypes.Length, Is.Not.EqualTo(allTypes.Length));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsSubclassOfRawGenericMethod
        [TestFixture]
        public class TheIsSubclassOfRawGenericMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullGeneric()
            {
                Assert.Throws<ArgumentNullException>(() => TypeHelper.IsSubclassOfRawGeneric(null, typeof(bool)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullToCheck()
            {
                Assert.Throws<ArgumentNullException>(() => TypeHelper.IsSubclassOfRawGeneric(typeof(bool), null));
            }

            [TestCase]
            public void ReturnsFalseForNonDerivingClass()
            {
                var genericType = typeof(SavableModelBase<>);
                var toCheck = new List<string>();

                Assert.That(TypeHelper.IsSubclassOfRawGeneric(genericType, toCheck.GetType()), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForDerivingClass()
            {
                var genericType = typeof(List<>);
                var toCheck = new List<string>();

                Assert.That(TypeHelper.IsSubclassOfRawGeneric(genericType, toCheck.GetType()), Is.True);
            }
            #endregion
        }
        #endregion
    }
}
