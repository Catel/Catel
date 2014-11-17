// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectObserverFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using System;
    using Catel.Memento;

    using NUnit.Framework;

    public class ObjectObserverFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyChanged()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ObjectObserver(null, null, new MementoService()));
            }

            [TestCase]
            public void SetsValuesCorrectly()
            {
                var obj = new Mocks.MockModel();
                var tag = "MyTag";
                var service = new MementoService();

                var observer = new ObjectObserver(obj, tag, service);

                Assert.AreEqual(tag, observer.Tag);
            }
        }
    }
}