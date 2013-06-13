// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrongViewModelTypeExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Exceptions
{
    using Catel.MVVM;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class WrongViewModelTypeExceptionFacts
    {
        #region Nested type: TheConstructor
        [TestClass]
        public class TheConstructor
        {
            #region Methods
            [TestMethod]
            public void SetsPropertiesCorrectly()
            {
                try
                {
                    throw new WrongViewModelTypeException(typeof (int), typeof (string));
                }
                catch (WrongViewModelTypeException ex)
                {
                    Assert.AreEqual(typeof (int), ex.ActualType);
                    Assert.AreEqual(typeof (string), ex.ExpectedType);
                }
            }
            #endregion
        }
        #endregion
    }
}