// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNotFoundInModelExceptionTest.cs" company="Catel development team">
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
    public class PropertyNotFoundInModelExceptionTest
    {
        #region Methods
        [TestMethod]
        public void Constructor()
        {
            try
            {
                throw new PropertyNotFoundInModelException("viewModelProperty", "modelName", "modelPropertyName");
            }
            catch (PropertyNotFoundInModelException ex)
            {
                Assert.AreEqual("viewModelProperty", ex.ViewModelPropertyName);
                Assert.AreEqual("modelName", ex.ModelName);
                Assert.AreEqual("modelPropertyName", ex.ModelPropertyName);
            }
        }
        #endregion
    }
}