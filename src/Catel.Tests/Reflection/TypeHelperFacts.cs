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
            public void ThrowsArgumentExceptionForNullOrEmptyType(string type)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.ConvertTypeToVersionIndependentType(type));
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForSimpleVersionIndependentType()
            {
                const string input = "Catel.Data.ObservableObject, Catel.Core";
                const string output = "Catel.Data.ObservableObject, Catel.Core";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.AreEqual(output, realOutput);
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForComplexVersionIndependentType()
            {
                string input = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";
                const string output = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.AreEqual(output, realOutput);
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForSimpleVersionDependentType()
            {
                string input = typeof(ObservableObject).AssemblyQualifiedName;
                const string output = "Catel.Data.ObservableObject, Catel.Core";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.AreEqual(output, realOutput);
            }

            [TestCase]
            public void ReturnsVersionIndependentTypeForComplexVersionDependentType()
            {
                // Use a different key token because MS key tokens are not changed
                string input = "System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx";
                const string output = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.AreEqual(output, realOutput);
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
                Assert.AreEqual(string.Empty, TypeHelper.FormatInnerTypes((IEnumerable<string>)Array.Empty<string>()));
            }

            [TestCase]
            public void ReturnsArrayOfTypesForFilledArray()
            {
                string expectedValue = "[string],[string],[int]";

                string actualValue = TypeHelper.FormatInnerTypes((IEnumerable<string>)new[] { "string", "string", "int" });

                Assert.AreEqual(expectedValue, actualValue);
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
            public void ThrowsArgumentExceptionForNullOrEmptyAssemblyName(string assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.FormatType(assemblyName, "Type"));
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForNullOrEmptyTypeName(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.FormatType("Catel.Core", typeName));
            }

            [TestCase]
            public void ReturnsFormattedType()
            {
                string expectedValue = "Catel.Tests.Helpers.TypeHelperFacts, Catel.Tests";

                string actualValue = TypeHelper.FormatType("Catel.Tests", "Catel.Tests.Helpers.TypeHelperFacts");

                Assert.AreEqual(expectedValue, actualValue);
            }
        }
        #endregion

        #region Nested type: TheGetAssemblyNameMethod
        [TestFixture]
        public class TheGetAssemblyNameMethod
        {
            [TestCase(null)]
            [TestCase("")]
            public void ThrowsArgumentExceptionForNullFullTypeName(string assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetAssemblyName(assemblyName));
            }

            [TestCase("Catel.Tests.TypeHelper", null)]
            [TestCase("Catel.Tests.TypeHelper, Catel.Core", "Catel.Core")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]]", null)]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib", "mscorlib")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
            public void ReturnsAssemblyName(string typeName, string expectedAssembly)
            {
                Assert.AreEqual(expectedAssembly, TypeHelper.GetAssemblyName(typeName));
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
            public void ThrowsArgumentExceptionForNullOrEmptyAssemblyName(string assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetAssemblyNameWithoutOverhead(assemblyName));
            }

            [TestCase]
            public void ReturnsAssemblyNameForAssemblyWithoutOverhead()
            {
                var input = "Catel.Core";
                var expected = "Catel.Core";

                Assert.AreEqual(expected, TypeHelper.GetAssemblyNameWithoutOverhead(input));
            }

            [TestCase]
            public void ReturnsAssemblyNameForAssemblyWithOverhead()
            {
                var input = "Catel.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1c8163524cbe02e6";
                var expected = "Catel.Core";

                Assert.AreEqual(expected, TypeHelper.GetAssemblyNameWithoutOverhead(input));
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
            public void ThrowsArgumentExceptionForNullOrEmptyType(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetInnerTypes(typeName));
            }

            [TestCase]
            public void ReturnsEmptyArrayForNonGenericType()
            {
                var input = "Catel.Data.ObservableObject, Catel.Core";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.AreEqual(0, output.Length);
            }

            [TestCase]
            public void ReturnsInnerTypesForSimpleGenericType()
            {
                var input = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx], [System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.AreEqual(2, output.Length);
                Assert.AreEqual("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx", output[0]);
                Assert.AreEqual("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx", output[1]);
            }

            [TestCase]
            public void ReturnsInnerTypesForComplexGenericType()
            {
                var input = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx], [System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.AreEqual(2, output.Length);
                Assert.AreEqual("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx", output[0]);
                Assert.AreEqual("System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx", output[1]);
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
            public void ThrowsArgumentExceptionForNullFullTypeName(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeName(typeName));
            }

            [TestCase("Catel.Tests.TypeHelper", "Catel.Tests.TypeHelper")]
            [TestCase("Catel.Tests.TypeHelper, Catel.Core", "Catel.Tests.TypeHelper")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib", "System.Collections.Generic.List`1[[Catel.Data.PropertyValue]]")]
            [TestCase("System.Collections.Generic.List`1[[Catel.Data.PropertyValue, Catel.Core]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Collections.Generic.List`1[[Catel.Data.PropertyValue]]")]
            public void ReturnsTypeName(string input, string expectedOutput)
            {
                Assert.AreEqual(expectedOutput, TypeHelper.GetTypeName(input));
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
            public void ThrowsArgumentExceptionForNullFullTypeName(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeNameWithAssembly(typeName));
            }

            [TestCase]
            public void ReturnsFullTypeNameForTypeWithAssemblyWithoutOverhead()
            {
                Assert.AreEqual("Catel.Tests.TypeHelper, Catel.Core", TypeHelper.GetTypeNameWithAssembly("Catel.Tests.TypeHelper, Catel.Core"));
            }

            [TestCase]
            public void ReturnsFullTypeNameForTypeWithAssembly()
            {
                Assert.AreEqual("Catel.Tests.TypeHelper, Catel.Core", TypeHelper.GetTypeNameWithAssembly("Catel.Tests.TypeHelper, Catel.Core, Version=1.0.0.0, PublicKeyToken=1234578, Culture=neutral"));
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
            public void ThrowsArgumentExceptionForNullFullTypeName(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeNameWithoutNamespace(typeName));
            }

            [TestCase]
            public void ReturnsTypeNameForTypeWithoutAssembly()
            {
                Assert.AreEqual("TypeHelper", TypeHelper.GetTypeNameWithoutNamespace("Catel.Tests.TypeHelper"));
            }

            [TestCase]
            public void ReturnsTypeNameForTypeWithAssembly()
            {
                Assert.AreEqual("TypeHelper", TypeHelper.GetTypeNameWithoutNamespace("Catel.Tests.TypeHelper, Catel.Core"));
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
            public void ThrowsArgumentExceptionForNullFullTypeName(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeHelper.GetTypeNamespace(typeName));
            }

            [TestCase]
            public void ReturnsTypeNamespaceForTypeWithoutAssembly()
            {
                Assert.AreEqual("Catel.Tests", TypeHelper.GetTypeNamespace("Catel.Tests.TypeHelper"));
            }

            [TestCase]
            public void ReturnsTypeNamespaceForTypeWithAssembly()
            {
                Assert.AreEqual("Catel.Tests", TypeHelper.GetTypeNamespace("Catel.Tests.TypeHelper, Catel.Core"));
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
            public void ThrowsArgumentExceptionForInvalidAssemblyName(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeCache.GetTypeWithAssembly(typeName, "Catel.Core"));
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            public void ThrowsArgumentExceptionForInvalidTypeName(string assemblyName)
            {
                Assert.Throws<ArgumentException>(() => TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", assemblyName));
            }

            [TestCase]
            public void ReturnsNullForUnavailableType()
            {
                Assert.IsNull(TypeCache.GetTypeWithAssembly("Catel.UnknownType", "Catel.Core"));
            }

            [TestCase]
            public void ReturnsTypeForAvailableType()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", "Catel.Core"));
            }

            [TestCase]
            public void ReturnsNullWithoutIgnoringCase()
            {
                Assert.AreEqual(null, TypeCache.GetTypeWithAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", "cAtEl.CoRe", false));
            }

            [TestCase]
            public void ReturnsTypeWithIgnoringCase()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", "cAtEl.CoRe", true));
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
            public void ThrowsArgumentExceptionForInvalidTypeName(string typeName)
            {
                Assert.Throws<ArgumentException>(() => TypeCache.GetTypeWithoutAssembly(typeName));
            }

            [TestCase]
            public void ReturnsNullForUnavailableType()
            {
                Assert.IsNull(TypeCache.GetTypeWithoutAssembly("Catel.UnknownType"));
            }

            [TestCase]
            public void ReturnsTypeForAvailableType()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithoutAssembly("Catel.Reflection.TypeHelper"));
            }

            [TestCase]
            public void ReturnsNullWithoutIgnoringCase()
            {
                Assert.AreEqual(null, TypeCache.GetTypeWithoutAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", false));
            }

            [TestCase]
            public void ReturnsTypeWithIgnoringCase()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithoutAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", true));
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
                Assert.AreEqual(null, TypeHelper.GetTypedInstance<List<int>>(null));
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
                Assert.AreEqual(list, TypeHelper.GetTypedInstance<List<int>>(list));
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

                Assert.AreNotEqual(allTypes.Length, notifyTypes.Length);
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

                Assert.IsFalse(TypeHelper.IsSubclassOfRawGeneric(genericType, toCheck.GetType()));
            }

            [TestCase]
            public void ReturnsTrueForDerivingClass()
            {
                var genericType = typeof(List<>);
                var toCheck = new List<string>();

                Assert.IsTrue(TypeHelper.IsSubclassOfRawGeneric(genericType, toCheck.GetType()));
            }
            #endregion
        }
        #endregion
    }
}
