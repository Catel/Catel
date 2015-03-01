// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestedInAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.Attributes
{
    using System;
    using Catel.MVVM;

    using NUnit.Framework;

    public class InterestedInAttributeFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InterestedInAttribute(null));
            }

            [TestCase]
            public void CorrectlySetsViewModelType()
            {
                var interestedInAttribute = new InterestedInAttribute(typeof(ViewModelBase));

                Assert.AreEqual(typeof(ViewModelBase), interestedInAttribute.ViewModelType);
            }
        }
    }
}