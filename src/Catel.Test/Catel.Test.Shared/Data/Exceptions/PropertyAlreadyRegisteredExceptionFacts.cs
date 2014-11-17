// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyAlreadyRegisteredExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data.Exceptions
{
    using Catel.Data;

    using NUnit.Framework;

    public class PropertyAlreadyRegisteredExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsValuesCorrectly()
            {
                var exception = new PropertyAlreadyRegisteredException("PropertyName", typeof(string));
                Assert.AreEqual("PropertyName", exception.PropertyName);
                Assert.AreEqual(typeof(string), exception.PropertyType);
            }
        }
    }
}