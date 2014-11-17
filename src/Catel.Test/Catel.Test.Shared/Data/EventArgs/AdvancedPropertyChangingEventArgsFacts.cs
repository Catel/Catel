// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedPropertyChangingEventArgsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using Catel.Data;

    using NUnit.Framework;

    [TestFixture]
    public class AdvancedPropertyChangingEventArgsFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsPropertyNameCorrectly()
            {
                var e = new AdvancedPropertyChangingEventArgs("test");

                Assert.AreEqual("test", e.PropertyName);
            }

            [TestCase]
            public void DefaultsCancelToFalse()
            {
                var e = new AdvancedPropertyChangingEventArgs("test");

                Assert.IsFalse(e.Cancel);
            }
        }
    }
}