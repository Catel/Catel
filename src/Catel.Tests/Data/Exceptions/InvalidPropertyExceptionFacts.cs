// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPropertyExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Data.Exceptions
{
    using Catel.Data;

    using NUnit.Framework;

    public class InvalidPropertyExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsValuesCorrectly()
            {
                var exception = new InvalidPropertyException("PropertyName");
                Assert.AreEqual("PropertyName", exception.PropertyName);
            }
        }
    }
}