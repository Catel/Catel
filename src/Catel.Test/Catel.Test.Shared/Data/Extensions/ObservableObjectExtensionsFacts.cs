// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableObjectExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
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
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ObservableObjectExtensions.RaiseAllPropertiesChanged(null));
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