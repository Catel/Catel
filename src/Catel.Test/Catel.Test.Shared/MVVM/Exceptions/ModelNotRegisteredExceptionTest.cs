// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelNotRegisteredExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Exceptions
{
    using Catel.MVVM;

    using NUnit.Framework;

    [TestFixture]
    public class ModelNotRegisteredExceptionTest
    {
        #region Methods
        [TestCase]
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