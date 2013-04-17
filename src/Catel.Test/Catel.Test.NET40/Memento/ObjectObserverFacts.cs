// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectObserverTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using System;
    using Catel.Memento;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ObjectObserverFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullPropertyChanged()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ObjectObserver(null, null, new MementoService()));
            }

            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var obj = new Mocks.MockModel();
                var tag = "MyTag";
                var service = new MementoService();

                var observer = new ObjectObserver(obj, tag, service);

                Assert.AreEqual(tag, observer.Tag);
            }
        }

        [TestClass]
        public class TheCancelSubscriptionMethod
        {
            
        }
    }
}