// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowNotRegisteredExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Services.Exceptions
{
    using Catel.Services;

    using NUnit.Framework;

    [TestFixture]
    public class WindowNotRegisteredExceptionTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            try
            {
                throw new WindowNotRegisteredException("windowName");
            }
            catch (WindowNotRegisteredException ex)
            {
                Assert.AreEqual("windowName", ex.Name);
            }
        }
        #endregion
    }
}