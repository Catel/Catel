// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensionsFacts.type.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Catel.IoC;
    using Catel.Reflection;
    using Catel.Windows;
    using NUnit.Framework;

    public partial class ReflectionExtensionsFacts
    {
        [TestFixture]
        public class TheImplementsInterfaceExMethod
        {
            public interface ISomeInterface
            {
                
            }

            public class A : ISomeInterface
            {
                
            }

            public class B : A
            {
                
            }

            [TestCase]
            public void ReturnsTrueForTypeDirectImplementingInterface()
            {
                var type = typeof(A);

                Assert.IsTrue(type.ImplementsInterfaceEx<ISomeInterface>());
            }

            [TestCase]
            public void ReturnsTrueForTypeDerivetiveImplementingInterface()
            {
                var type = typeof (B);

                Assert.IsTrue(type.ImplementsInterfaceEx<ISomeInterface>());
            }
        }

        [TestFixture]
        public class TheGetSafeFullNameMethod
        {
            [TestCase(typeof(string), false, "System.String")]
            [TestCase(typeof(string), true, "System.String, System")]
            [TestCase(typeof(TypeFactory), false, "Catel.IoC.typeFactory")]
            [TestCase(typeof(TypeFactory), true, "Catel.IoC.typeFactory, Catel.Core")]
            public void ReturnsFullName(Type type, bool includeAssembly, string expected)
            {
                
            }
        }

        [TestFixture]
        public class TheIsInstanceOfTypeExMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ReflectionExtensions.IsInstanceOfTypeEx(null, new object()));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullObjectToCheck()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ReflectionExtensions.IsInstanceOfTypeEx(typeof(object), null));
            }

            [TestCase]
            public void ReturnsTrueForEqualReferenceType()
            {
                var type = typeof (InvalidOperationException);
                var instance = new InvalidOperationException();

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));
            }

            [TestCase]
            public void ReturnsTrueForInheritingReferenceType()
            {
                var type = typeof(Exception);
                var instance = new InvalidOperationException();

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));              
            }

            [TestCase]
            public void ReturnsFalseForNonInheritingReferenceType()
            {
                var type = typeof(Exception);
                var instance = new EventArgs();

                Assert.IsFalse(type.IsInstanceOfTypeEx(instance));
            }

            [TestCase]
            public void ReturnsTrueForEqualValueType()
            {
                var type = typeof(int);
                var instance = 32;

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));
            }

            [TestCase]
            public void ReturnsTrueForSpecialValueTypes()
            {
                var type = typeof(Int64);
                var instance = 32;

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));
            }

            [TestCase]
            public void ReturnsFalseForNonInheritingValueType()
            {
                var type = typeof(bool);
                var instance = 32;

                Assert.IsFalse(type.IsInstanceOfTypeEx(instance));
            }            
        }

        [TestFixture]
        public class TheGetPropertyExMethod
        {
            public class TestViewModel : ApplicationViewModel
            {
                public new IPerson Variable1 { get; set; }

            }

            public class ApplicationViewModel
            {
                public INameProvider Variable1 { get; set; }

            }

            public interface IPerson : INameProvider
            {
                
            }

            public interface INameProvider
            {
                string FirstName { get; }
            }

            public class Person : IPerson
            {
                string INameProvider.FirstName { get { return "John"; } }
            }

            [TestCase]
            public void PreventsAmbiguousMatchExceptionForAmbiguousProperties()
            {
                // Note: see https://github.com/Catel/Catel/issues/1325

                var propertyInfo = typeof(TestViewModel).GetPropertyEx(nameof(TestViewModel.Variable1));

                Assert.IsNotNull(propertyInfo);
                Assert.AreEqual(typeof(IPerson), propertyInfo.PropertyType);
            }

            [TestCase]
            public void ReturnsNoExplicitInterfacePropertiesWhenDisabled()
            {
                var propertyInfo = typeof (Person).GetPropertyEx("FirstName", allowExplicitInterfaceProperties: false);

                Assert.IsNull(propertyInfo);
            }

            [TestCase]
            public void ReturnsExplicitInterfacePropertiesWhenEnabled()
            {
                var propertyInfo = typeof(Person).GetPropertyEx("FirstName", allowExplicitInterfaceProperties: true);

                Assert.IsNotNull(propertyInfo);
            }
        }

        [TestFixture]
        public class TheGetFieldExMethod
        {
            [TestCase]
            public void DoesNotReturnNonPublicBaseClassField()
            {
                var fieldInfo = typeof(DataWindow).GetFieldEx("_showingAsDialog");

                // Note: see https://github.com/Catel/Catel/issues/1617, this should return null
                Assert.IsNull(fieldInfo);
            }

            [TestCase]
            public void ReturnsField()
            {
                var fieldInfo = typeof(System.Windows.Window).GetFieldEx("_showingAsDialog");

                Assert.IsNotNull(fieldInfo);
            }
        }

        [TestFixture]
        public class StaticInheritedMembers
        {
            public class BaseClass
            {
#pragma warning disable CS0169, CS0414
                private static readonly bool field1 = true;

                private readonly bool field2 = true;

                private static bool Property1 { get; set; }

                private bool Property2 { get; set; }

                private static void Method1()
                {
                }

                private void Method2()
                {
                }
#pragma warning restore CS0169, CS0414
            }

            public class DerivedClass : BaseClass
            {

            }

            [Test]
            public void ReturnsStaticFields()
            {
                var fields = typeof(DerivedClass).GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, true), true);

                Assert.IsTrue(fields.Any(x => x.Name == "field1"));
            }

            [Test]
            public void ReturnsInstanceFields()
            {
                var fields = typeof(DerivedClass).GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true), true);

                Assert.IsTrue(fields.Any(x => x.Name == "field2"));
            }

            [Test]
            public void ReturnsStaticProperties()
            {
                var properties = typeof(DerivedClass).GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, true), true);

                Assert.IsTrue(properties.Any(x => x.Name == "Property1"));
            }

            [Test]
            public void ReturnsInstanceProperties()
            {
                var properties = typeof(DerivedClass).GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true), true);

                Assert.IsTrue(properties.Any(x => x.Name == "Property2"));
            }

            [Test]
            public void ReturnsStaticMethods()
            {
                var methods = typeof(DerivedClass).GetMethodsEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, true), true);

                Assert.IsTrue(methods.Any(x => x.Name == "Method1"));
            }

            [Test]
            public void ReturnsInstanceMethods()
            {
                var methods = typeof(DerivedClass).GetMethodsEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true), true);

                Assert.IsTrue(methods.Any(x => x.Name == "Method2"));
            }
        }
    }
}
