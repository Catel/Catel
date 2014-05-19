// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel.Data;
    using Catel.Reflection;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
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
        [TestClass]
        public class TheConvertTypeToVersionIndependentTypeMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.ConvertTypeToVersionIndependentType(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.ConvertTypeToVersionIndependentType(string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.ConvertTypeToVersionIndependentType(" "));
            }

            [TestMethod]
            public void ReturnsVersionIndependentTypeForSimpleVersionIndependentType()
            {
                const string input = "Catel.Data.ObservableObject, Catel.Core";
                const string output = "Catel.Data.ObservableObject, Catel.Core";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.AreEqual(output, realOutput);
            }

            [TestMethod]
            public void ReturnsVersionIndependentTypeForComplexVersionIndependentType()
            {
                string input = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";
                const string output = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.AreEqual(output, realOutput);
            }

            [TestMethod]
            public void ReturnsVersionIndependentTypeForSimpleVersionDependentType()
            {
                string input = typeof (ObservableObject).AssemblyQualifiedName;
                const string output = "Catel.Data.ObservableObject, Catel.Core";

                var realOutput = TypeHelper.ConvertTypeToVersionIndependentType(input);

                Assert.AreEqual(output, realOutput);
            }

            [TestMethod]
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
        [TestClass]
        public class TheFormatInnerTypesMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsEmptyStringForEmptyArray()
            {
                Assert.AreEqual(string.Empty, TypeHelper.FormatInnerTypes(new string[] {}));
            }

            [TestMethod]
            public void ReturnsArrayOfTypesForFilledArray()
            {
                string expectedValue = "[string],[string],[int]";

                string actualValue = TypeHelper.FormatInnerTypes(new[] {"string", "string", "int"});

                Assert.AreEqual(expectedValue, actualValue);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheFormatTypeMethod
        [TestClass]
        public class TheFormatTypeMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyAssemblyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.FormatType(null, "Type"));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.FormatType(string.Empty, "Type"));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.FormatType(" ", "Type"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.FormatType("Catel.Core", null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.FormatType("Catel.Core", string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.FormatType("Catel.Core", " "));
            }

            [TestMethod]
            public void ReturnsFormattedType()
            {
                string expectedValue = "Catel.Test.Helpers.TypeHelperFacts, Catel.Test";

                string actualValue = TypeHelper.FormatType("Catel.Test", "Catel.Test.Helpers.TypeHelperFacts");

                Assert.AreEqual(expectedValue, actualValue);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetAssemblyNameMethod
        [TestClass]
        public class TheGetAssemblyNameMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullFullTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetAssemblyName(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetAssemblyName(string.Empty));
            }

            [TestMethod]
            public void ReturnsNullForTypeWithoutAssembly()
            {
                Assert.AreEqual(null, TypeHelper.GetAssemblyName("Catel.Test.TypeHelper"));
            }

            [TestMethod]
            public void ReturnsAssemblyForTypeWithAssembly()
            {
                Assert.AreEqual("Catel.Core", TypeHelper.GetAssemblyName("Catel.Test.TypeHelper, Catel.Core"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetAssemblyNameWithoutOverheadMethod
        [TestClass]
        public class TheGetAssemblyNameWithoutOverheadMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyAssemblyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetAssemblyNameWithoutOverhead(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetAssemblyNameWithoutOverhead(string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetAssemblyNameWithoutOverhead(" "));
            }

            [TestMethod]
            public void ReturnsAssemblyNameForAssemblyWithoutOverhead()
            {
                var input = "Catel.Core";
                var expected = "Catel.Core";

                Assert.AreEqual(expected, TypeHelper.GetAssemblyNameWithoutOverhead(input));
            }

            [TestMethod]
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
        [TestClass]
        public class TheGetInnerTypesMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetInnerTypes(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetInnerTypes(string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetInnerTypes(" "));
            }

            [TestMethod]
            public void ReturnsEmptyArrayForNonGenericType()
            {
                var input = "Catel.Data.ObservableObject, Catel.Core";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.AreEqual(0, output.Length);
            }

            [TestMethod]
            public void ReturnsInnerTypesForSimpleGenericType()
            {
                var input = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx], [System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx";
                var output = TypeHelper.GetInnerTypes(input);

                Assert.AreEqual(2, output.Length);
                Assert.AreEqual("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx", output[0]);
                Assert.AreEqual("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=xxxxxxx", output[1]);
            }

            [TestMethod]
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
        [TestClass]
        public class TheGetTypeNameMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullFullTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeName(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeName(string.Empty));
            }

            [TestMethod]
            public void ReturnsTypeNameForTypeWithoutAssembly()
            {
                Assert.AreEqual("Catel.Test.TypeHelper", TypeHelper.GetTypeName("Catel.Test.TypeHelper"));
            }

            [TestMethod]
            public void ReturnsTypeNameForTypeWithAssembly()
            {
                Assert.AreEqual("Catel.Test.TypeHelper", TypeHelper.GetTypeName("Catel.Test.TypeHelper, Catel.Core"));
            }

            [TestMethod]
            public void ReturnsTypeNameForGenericTypes()
            {
                string input = "System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
                string expectedOutput = "System.Collections.Generic.List`1[[System.String]]";

                Assert.AreEqual(expectedOutput, TypeHelper.GetTypeName(input));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeNameWithAssemblyMethod
        [TestClass]
        public class TheGetTypeNameWithAssemblyMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullFullTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeNameWithAssembly(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeNameWithAssembly(string.Empty));
            }

            [TestMethod]
            public void ReturnsFullTypeNameForTypeWithAssemblyWithoutOverhead()
            {
                Assert.AreEqual("Catel.Test.TypeHelper, Catel.Core", TypeHelper.GetTypeNameWithAssembly("Catel.Test.TypeHelper, Catel.Core"));
            }

            [TestMethod]
            public void ReturnsFullTypeNameForTypeWithAssembly()
            {
                Assert.AreEqual("Catel.Test.TypeHelper, Catel.Core", TypeHelper.GetTypeNameWithAssembly("Catel.Test.TypeHelper, Catel.Core, Version=1.0.0.0, PublicKeyToken=1234578, Culture=neutral"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeNameWithoutNamespaceMethod
        [TestClass]
        public class TheGetTypeNameWithoutNamespaceMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullFullTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeNameWithoutNamespace(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeNameWithoutNamespace(string.Empty));
            }

            [TestMethod]
            public void ReturnsTypeNameForTypeWithoutAssembly()
            {
                Assert.AreEqual("TypeHelper", TypeHelper.GetTypeNameWithoutNamespace("Catel.Test.TypeHelper"));
            }

            [TestMethod]
            public void ReturnsTypeNameForTypeWithAssembly()
            {
                Assert.AreEqual("TypeHelper", TypeHelper.GetTypeNameWithoutNamespace("Catel.Test.TypeHelper, Catel.Core"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeNamespaceMethod
        [TestClass]
        public class TheGetTypeNamespaceMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForNullFullTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeNamespace(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeHelper.GetTypeNamespace(string.Empty));
            }

            [TestMethod]
            public void ReturnsTypeNamespaceForTypeWithoutAssembly()
            {
                Assert.AreEqual("Catel.Test", TypeHelper.GetTypeNamespace("Catel.Test.TypeHelper"));
            }

            [TestMethod]
            public void ReturnsTypeNamespaceForTypeWithAssembly()
            {
                Assert.AreEqual("Catel.Test", TypeHelper.GetTypeNamespace("Catel.Test.TypeHelper, Catel.Core"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeWithAssemblyMethod
        [TestClass]
        public class TheGetTypeWithAssemblyMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForInvalidAssemblyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithAssembly(null, "Catel.Core"));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithAssembly(string.Empty, "Catel.Core"));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithAssembly(" ", "Catel.Core"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForInvalidTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", " "));
            }

            [TestMethod]
            public void ReturnsNullForUnavailableType()
            {
                Assert.IsNull(TypeCache.GetTypeWithAssembly("Catel.UnknownType", "Catel.Core"));
            }

            [TestMethod]
            public void ReturnsTypeForAvailableType()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithAssembly("Catel.Reflection.TypeHelper", "Catel.Core"));
            }

            [TestMethod]
            public void ReturnsNullWithoutIgnoringCase()
            {
                Assert.AreEqual(null, TypeCache.GetTypeWithAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", "cAtEl.CoRe", false));
            }

            [TestMethod]
            public void ReturnsTypeWithIgnoringCase()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", "cAtEl.CoRe", true));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypeWithoutAssemblyMethod
        [TestClass]
        public class TheGetTypeWithoutAssemblyMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentExceptionForInvalidTypeName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithoutAssembly(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithoutAssembly(string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => TypeCache.GetTypeWithoutAssembly(" "));
            }

            [TestMethod]
            public void ReturnsNullForUnavailableType()
            {
                Assert.IsNull(TypeCache.GetTypeWithoutAssembly("Catel.UnknownType"));
            }

            [TestMethod]
            public void ReturnsTypeForAvailableType()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithoutAssembly("Catel.Reflection.TypeHelper"));
            }

            [TestMethod]
            public void ReturnsNullWithoutIgnoringCase()
            {
                Assert.AreEqual(null, TypeCache.GetTypeWithoutAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", false));
            }

            [TestMethod]
            public void ReturnsTypeWithIgnoringCase()
            {
                Assert.AreEqual(typeof(TypeHelper), TypeCache.GetTypeWithoutAssembly("cAtEl.rEfLeCtIoN.tYpEhElPeR", true));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypedInstanceMethod
        [TestClass]
        public class TheGetTypedInstanceMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsNullForNullInstance()
            {
                Assert.AreEqual(null, TypeHelper.GetTypedInstance<List<int>>(null));
            }

            [TestMethod]
            public void ThrowsNotSupportedExceptionForInvalidCast()
            {
                var list = new List<int>();
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => TypeHelper.GetTypedInstance<List<string>>(list));
            }

            [TestMethod]
            public void ReturnsTypedInstanceWhenInstanceIsOfType()
            {
                var list = new List<int>();
                Assert.AreEqual(list, TypeHelper.GetTypedInstance<List<int>>(list));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetTypesMethod
        [TestClass]
        public class TheGetTypesMethod
        {
            #region Methods
            [TestMethod]
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
        [TestClass]
        public class TheIsSubclassOfRawGenericMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullGeneric()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => TypeHelper.IsSubclassOfRawGeneric(null, typeof (bool)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullToCheck()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => TypeHelper.IsSubclassOfRawGeneric(typeof (bool), null));
            }

            [TestMethod]
            public void ReturnsFalseForNonDerivingClass()
            {
                var genericType = typeof (SavableModelBase<>);
                var toCheck = new List<string>();

                Assert.IsFalse(TypeHelper.IsSubclassOfRawGeneric(genericType, toCheck.GetType()));
            }

            [TestMethod]
            public void ReturnsTrueForDerivingClass()
            {
                var genericType = typeof (List<>);
                var toCheck = new List<string>();

                Assert.IsTrue(TypeHelper.IsSubclassOfRawGeneric(genericType, toCheck.GetType()));
            }
            #endregion
        }
        #endregion
    }
}