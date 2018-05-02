// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPropertyValueExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Data.Exceptions
{
    using Catel.Data;

    using NUnit.Framework;

    public class InvalidPropertyValueExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsValuesCorrectly()
            {
                var exception = new InvalidPropertyValueException("PropertyName", typeof(int), typeof(string));
                Assert.AreEqual("PropertyName", exception.PropertyName);
                Assert.AreEqual(typeof(int), exception.ExpectedType);
                Assert.AreEqual(typeof(string), exception.ActualType);
            }
        }
    }
}