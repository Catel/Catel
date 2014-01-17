// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedEventArgsExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class PropertyChangedEventArgsExtensionsFacts
    {
        [TestClass]
        public class TheAllPropertiesChangedMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullPropertyChangedArguments()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyChangedEventArgsExtensions.AllPropertiesChanged(null));
            }

            [TestMethod]
            public void ReturnsTrueForNullPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs(null);

                Assert.IsTrue(propertyChangedEventArgs.AllPropertiesChanged());
            }

            [TestMethod]
            public void ReturnsTrueForEmptyPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs(string.Empty);

                Assert.IsTrue(propertyChangedEventArgs.AllPropertiesChanged());
            }

            [TestMethod]
            public void ReturnsFalseForNonEmptyPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("MyProperty");

                Assert.IsFalse(propertyChangedEventArgs.AllPropertiesChanged());
            }
        }

        [TestClass]
        public class TheHasPropertyChangedMethod
        {
            public string TestProperty { get; set; }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullPropertyChangedArguments()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyChangedEventArgsExtensions.HasPropertyChanged(null, () => TestProperty));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullPropertyExpressionArguments()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestProperty");

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyChangedEventArgs.HasPropertyChanged((Expression<Func<string>>)null));
            }

            [TestMethod]
            public void ReturnsTrueForSamePropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestProperty");

                Assert.IsTrue(propertyChangedEventArgs.HasPropertyChanged(() => TestProperty));
            }

            [TestMethod]
            public void ReturnsFalseForDifferentPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestPropertyNotExisting");

                Assert.IsFalse(propertyChangedEventArgs.HasPropertyChanged(() => TestProperty));
            }
        }
    }
}