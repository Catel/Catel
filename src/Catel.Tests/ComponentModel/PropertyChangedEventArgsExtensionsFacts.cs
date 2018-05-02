// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedEventArgsExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using NUnit.Framework;

    public class PropertyChangedEventArgsExtensionsFacts
    {
        [TestFixture]
        public class TheAllPropertiesChangedMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyChangedArguments()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyChangedEventArgsExtensions.AllPropertiesChanged(null));
            }

            [TestCase]
            public void ReturnsTrueForNullPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs(null);

                Assert.IsTrue(propertyChangedEventArgs.AllPropertiesChanged());
            }

            [TestCase]
            public void ReturnsTrueForEmptyPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs(string.Empty);

                Assert.IsTrue(propertyChangedEventArgs.AllPropertiesChanged());
            }

            [TestCase]
            public void ReturnsFalseForNonEmptyPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("MyProperty");

                Assert.IsFalse(propertyChangedEventArgs.AllPropertiesChanged());
            }
        }

        [TestFixture]
        public class TheHasPropertyChangedMethod
        {
            public string TestProperty { get; set; }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyChangedArguments()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyChangedEventArgsExtensions.HasPropertyChanged(null, () => TestProperty));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyExpressionArguments()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestProperty");

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyChangedEventArgs.HasPropertyChanged((Expression<Func<string>>)null));
            }

            [TestCase]
            public void ReturnsTrueForSamePropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestProperty");

                Assert.IsTrue(propertyChangedEventArgs.HasPropertyChanged(() => TestProperty));
            }

            [TestCase]
            public void ReturnsFalseForDifferentPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestPropertyNotExisting");

                Assert.IsFalse(propertyChangedEventArgs.HasPropertyChanged(() => TestProperty));
            }
        }
    }
}