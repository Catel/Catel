// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelNotRegisteredExceptionTest.cs" company="Catel development team">
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

    [TestClass]
    public class ModelNotRegisteredExceptionTest
    {
        #region Methods
        [TestMethod]
        public void Constructor()
        {
            try
            {
                throw new ModelNotRegisteredException("model", "property");
            }
            catch (ModelNotRegisteredException ex)
            {
                Assert.AreEqual("model", ex.ModelName);
                Assert.AreEqual("property", ex.PropertyDeclaringViewModelToModelAttribute);
            }
        }
        #endregion
    }
}