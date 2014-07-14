// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingTypeEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC.EventArgs
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class MissingTypeEventArgsFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInterfaceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new MissingTypeEventArgs(null));
            }

            [TestCase]
            public void SetsValuesCorrectly()
            {
                var eventArgs = new MissingTypeEventArgs(typeof (ITestInterface));

                Assert.AreEqual(typeof (ITestInterface), eventArgs.InterfaceType);
            }
        }
    }
}