// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalContainerNotSupportedExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC.Exceptions
{
    using Catel.IoC;
    using NUnit.Framework;

    [TestFixture]
    public class ExternalContainerNotSupportedExceptionTest
    {
        #region Methods
        [TestCase]
        public void FormatMessage_SupportedContainers()
        {
            string expectedString = @"The specified container is not supported. Please use one of the following:
  * Unity
  * Another IoC
";
            var ex = new ExternalContainerNotSupportedException(new[] {"Unity", "Another IoC"});

            Assert.AreEqual(expectedString, ex.Message);
        }
        #endregion
    }
}