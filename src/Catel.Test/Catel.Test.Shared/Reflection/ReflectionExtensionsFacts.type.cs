// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensionsFacts.type.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Catel.Reflection;
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
    }
}