// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelNotRegisteredExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Exceptions
{
    using Catel.MVVM;

    using NUnit.Framework;

    [TestFixture]
    public class ViewModelNotRegisteredExceptionTest
    {
        #region Methods
        [TestCase]
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