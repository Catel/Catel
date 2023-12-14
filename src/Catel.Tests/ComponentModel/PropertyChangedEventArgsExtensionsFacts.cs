namespace Catel.Tests.ComponentModel
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
                Assert.Throws<ArgumentNullException>(() => PropertyChangedEventArgsExtensions.AllPropertiesChanged(null));
            }

            [TestCase]
            public void ReturnsTrueForNullPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs(null);

                Assert.That(propertyChangedEventArgs.AllPropertiesChanged(), Is.True);
            }

            [TestCase]
            public void ReturnsTrueForEmptyPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs(string.Empty);

                Assert.That(propertyChangedEventArgs.AllPropertiesChanged(), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForNonEmptyPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("MyProperty");

                Assert.That(propertyChangedEventArgs.AllPropertiesChanged(), Is.False);
            }
        }

        [TestFixture]
        public class TheHasPropertyChangedMethod
        {
            public string TestProperty { get; set; }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyChangedArguments()
            {
                Assert.Throws<ArgumentNullException>(() => PropertyChangedEventArgsExtensions.HasPropertyChanged(null, () => TestProperty));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyExpressionArguments()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestProperty");

                Assert.Throws<ArgumentNullException>(() => propertyChangedEventArgs.HasPropertyChanged((Expression<Func<string>>)null));
            }

            [TestCase]
            public void ReturnsTrueForSamePropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestProperty");

                Assert.That(propertyChangedEventArgs.HasPropertyChanged(() => TestProperty), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForDifferentPropertyName()
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs("TestPropertyNotExisting");

                Assert.That(propertyChangedEventArgs.HasPropertyChanged(() => TestProperty), Is.False);
            }
        }
    }
}