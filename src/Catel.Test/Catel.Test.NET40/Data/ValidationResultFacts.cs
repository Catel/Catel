// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationResultTest.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;

    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class FieldValidationResultFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsNullReferenceExceptionForNullProperty()
            {
                ExceptionTester.CallMethodAndExpectException<NullReferenceException>(() => new FieldValidationResult((PropertyData)null, ValidationResultType.Error, "message"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new FieldValidationResult((string)null, ValidationResultType.Error, "message"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new FieldValidationResult(string.Empty, ValidationResultType.Error, "message"));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullMessage()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new FieldValidationResult("myProperty", ValidationResultType.Error, null));
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingEmptyMessage()
            {
                var validationResult = new FieldValidationResult("myProperty", ValidationResultType.Error, string.Empty);

                Assert.AreEqual("myProperty", validationResult.PropertyName);
                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual(string.Empty, validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = new FieldValidationResult("myProperty", ValidationResultType.Error, "my message");

                Assert.AreEqual("myProperty", validationResult.PropertyName);
                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message", validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = new FieldValidationResult("myProperty", ValidationResultType.Error, "my message with {0}", "format");

                Assert.AreEqual("myProperty", validationResult.PropertyName);
                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message with format", validationResult.Message);
            }
        }

        [TestClass]
        public class TheCreateWarningMethod
        {
            [TestMethod]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = FieldValidationResult.CreateWarning("myProperty", "my message");

                Assert.AreEqual("myProperty", validationResult.PropertyName);
                Assert.AreEqual(ValidationResultType.Warning, validationResult.ValidationResultType);
                Assert.AreEqual("my message", validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = FieldValidationResult.CreateWarning("myProperty", "my message with {0}", "format");

                Assert.AreEqual("myProperty", validationResult.PropertyName);
                Assert.AreEqual(ValidationResultType.Warning, validationResult.ValidationResultType);
                Assert.AreEqual("my message with format", validationResult.Message);
            }
        }

        [TestClass]
        public class TheCreateErrorMethod
        {
            [TestMethod]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = FieldValidationResult.CreateError("myProperty", "my message");

                Assert.AreEqual("myProperty", validationResult.PropertyName);
                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message", validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = FieldValidationResult.CreateError("myProperty", "my message with {0}", "format");

                Assert.AreEqual("myProperty", validationResult.PropertyName);
                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message with format", validationResult.Message);
            }
        }
    }

    public class BusinessRuleValidationResultFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullMessage()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new BusinessRuleValidationResult(ValidationResultType.Error, null));
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingEmptyMessage()
            {
                var validationResult = new BusinessRuleValidationResult(ValidationResultType.Error, string.Empty);

                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual(string.Empty, validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = new BusinessRuleValidationResult(ValidationResultType.Error, "my message");

                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message", validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = new BusinessRuleValidationResult(ValidationResultType.Error, "my message with {0}", "format");

                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message with format", validationResult.Message);
            }
        }

        [TestClass]
        public class TheCreateWarningMethod
        {
            [TestMethod]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateWarning("my message");

                Assert.AreEqual(ValidationResultType.Warning, validationResult.ValidationResultType);
                Assert.AreEqual("my message", validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateWarning("my message with {0}", "format");

                Assert.AreEqual(ValidationResultType.Warning, validationResult.ValidationResultType);
                Assert.AreEqual("my message with format", validationResult.Message);
            }
        }

        [TestClass]
        public class TheCreateErrorMethod
        {
            [TestMethod]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateError("my message");

                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message", validationResult.Message);
            }

            [TestMethod]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateError("my message with {0}", "format");

                Assert.AreEqual(ValidationResultType.Error, validationResult.ValidationResultType);
                Assert.AreEqual("my message with format", validationResult.Message);
            }
        }
    }
}