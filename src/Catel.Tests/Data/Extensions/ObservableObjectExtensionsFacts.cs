namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    using NUnit.Framework;

    public class ObservableObjectExtensionsFacts
    {
        public class TestModel : ObservableObject
        {
            
        }

        [TestFixture]
        public class TheRaiseAllPropertiesChangedMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullObject()
            {
                Assert.Throws<ArgumentNullException>(() => ObservableObjectExtensions.RaiseAllPropertiesChanged(null));
            }

            [TestCase]
            public void RaisesPropertyChangedEventCorrectly()
            {
                var model = new TestModel();

                var propertyChanged = false;
                model.PropertyChanged += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.PropertyName))
                    {
                        propertyChanged = true;
                    }
                };

                model.RaiseAllPropertiesChanged();

                Assert.IsTrue(propertyChanged);
            }
        }
    }
}
