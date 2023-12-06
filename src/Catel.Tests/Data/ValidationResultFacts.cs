namespace Catel.Tests.Data
{
    using System;

    using Catel.Data;

    using NUnit.Framework;

    public class FieldValidationResultFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsNullReferenceExceptionForNullProperty()
            {
                Assert.Throws<NullReferenceException>(() => new FieldValidationResult((IPropertyData)null, ValidationResultType.Error, "message"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullPropertyName()
            {
                Assert.Throws<ArgumentException>(() => new FieldValidationResult((string)null, ValidationResultType.Error, "message"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyPropertyName()
            {
                Assert.Throws<ArgumentException>(() => new FieldValidationResult(string.Empty, ValidationResultType.Error, "message"));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullMessage()
            {
                Assert.Throws<ArgumentNullException>(() => new FieldValidationResult("myProperty", ValidationResultType.Error, null));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingEmptyMessage()
            {
                var validationResult = new FieldValidationResult("myProperty", ValidationResultType.Error, string.Empty);

                Assert.That(validationResult.PropertyName, Is.EqualTo("myProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo(string.Empty));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = new FieldValidationResult("myProperty", ValidationResultType.Error, "my message");

                Assert.That(validationResult.PropertyName, Is.EqualTo("myProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message"));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = new FieldValidationResult("myProperty", ValidationResultType.Error, "my message with {0}", "format");

                Assert.That(validationResult.PropertyName, Is.EqualTo("myProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }
        }

        [TestFixture]
        public class TheCreateWarningMethod
        {
            private string MyProperty { get; set; }

            [TestCase]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = FieldValidationResult.CreateWarning("myProperty", "my message");

                Assert.That(validationResult.PropertyName, Is.EqualTo("myProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(validationResult.Message, Is.EqualTo("my message"));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = FieldValidationResult.CreateWarning("myProperty", "my message with {0}", "format");

                Assert.That(validationResult.PropertyName, Is.EqualTo("myProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }

            [TestCase]
            public void SetsValueCorrectlyUsingExpression()
            {
                var validationResult = FieldValidationResult.CreateWarning(() => MyProperty, "my message with {0}", "format");

                Assert.That(validationResult.PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }
        }

        [TestFixture]
        public class TheCreateErrorMethod
        {
            private string MyProperty { get; set; }

            [TestCase]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = FieldValidationResult.CreateError("myProperty", "my message");

                Assert.That(validationResult.PropertyName, Is.EqualTo("myProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message"));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = FieldValidationResult.CreateError("myProperty", "my message with {0}", "format");

                Assert.That(validationResult.PropertyName, Is.EqualTo("myProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }

            [TestCase]
            public void SetsValueCorrectlyUsingExpression()
            {
                var validationResult = FieldValidationResult.CreateError(() => MyProperty, "my message with {0}", "format");

                Assert.That(validationResult.PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }
        }
    }

    public class BusinessRuleValidationResultFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullMessage()
            {
                Assert.Throws<ArgumentNullException>(() => new BusinessRuleValidationResult(ValidationResultType.Error, null));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingEmptyMessage()
            {
                var validationResult = new BusinessRuleValidationResult(ValidationResultType.Error, string.Empty);

                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo(string.Empty));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = new BusinessRuleValidationResult(ValidationResultType.Error, "my message");

                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message"));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = new BusinessRuleValidationResult(ValidationResultType.Error, "my message with {0}", "format");

                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }
        }

        [TestFixture]
        public class TheCreateWarningMethod
        {
            [TestCase]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateWarning("my message");

                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(validationResult.Message, Is.EqualTo("my message"));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateWarning("my message with {0}", "format");

                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }
        }

        [TestFixture]
        public class TheCreateErrorMethod
        {
            [TestCase]
            public void SetsValuesCorrectlyUsingNormalMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateError("my message");

                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message"));
            }

            [TestCase]
            public void SetsValuesCorrectlyUsingFormattedMessage()
            {
                var validationResult = BusinessRuleValidationResult.CreateError("my message with {0}", "format");

                Assert.That(validationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(validationResult.Message, Is.EqualTo("my message with format"));
            }
        }
    }
}
