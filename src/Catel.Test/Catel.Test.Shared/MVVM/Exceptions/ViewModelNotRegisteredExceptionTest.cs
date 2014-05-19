// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelNotRegisteredExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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

    [TestClass]
    public class ViewModelNotRegisteredExceptionTest
    {
        #region Methods
        [TestMethod]
        public void Constructor()
        {
            try
            {
                throw new ViewModelNotRegisteredException(typeof(ViewModelBase));
            }
            catch (ViewModelNotRegisteredException ex)
            {
                Assert.AreEqual(typeof(ViewModelBase), ex.ViewModelType);
            }
        }
        #endregion
    }
}