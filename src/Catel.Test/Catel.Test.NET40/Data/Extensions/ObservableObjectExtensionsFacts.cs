// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableObjectExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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

    public class ObservableObjectExtensionsFacts
    {
        public class TestModel : ObservableObject
        {
            
        }

        [TestClass]
        public class TheRaiseAllPropertiesChangedMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullObject()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ObservableObjectExtensions.RaiseAllPropertiesChanged(null));
            }

            [TestMethod]
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