// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Extensions.DynamicObjects
{
    using System;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class DynamicModelBaseFacts
    {
        public class DynamicModel : DynamicModelBase
        {
            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);
        }

        [TestClass]
        public class TheGetValueProperties
        {
            [TestMethod]
            public void CorrectlyReturnsTheRightValue()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                model.NonExistingGetProperty = "test";

                Assert.IsTrue(dynamicModel.IsPropertyRegistered("NonExistingGetProperty"));

                Assert.AreEqual("test", model.NonExistingGetProperty);
            }
        }

        [TestClass]
        public class TheSetValueProperties
        {
            [TestMethod]
            public void AutomaticallyRegistersNonExistingProperty()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                Assert.IsFalse(dynamicModel.IsPropertyRegistered("NonExistingSetProperty"));

                model.NonExistingSetProperty = "test";

                Assert.IsTrue(dynamicModel.IsPropertyRegistered("NonExistingSetProperty"));
            }
        }
    }
}