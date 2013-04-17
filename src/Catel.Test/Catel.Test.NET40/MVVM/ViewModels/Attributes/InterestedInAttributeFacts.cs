// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestedInAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.Attributes
{
    using System;
    using Catel.MVVM;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class InterestedInAttributeFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InterestedInAttribute(null));
            }

            [TestMethod]
            public void CorrectlySetsViewModelType()
            {
                var interestedInAttribute = new InterestedInAttribute(typeof(ViewModelBase));

                Assert.AreEqual(typeof(ViewModelBase), interestedInAttribute.ViewModelType);
            }
        }
    }
}