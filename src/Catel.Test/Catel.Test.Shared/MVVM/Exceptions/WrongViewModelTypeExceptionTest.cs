// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrongViewModelTypeExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Exceptions
{
    using Catel.MVVM;

    using NUnit.Framework;

    public class WrongViewModelTypeExceptionFacts
    {
        #region Nested type: TheConstructor
        [TestFixture]
        public class TheConstructor
        {
            #region Methods
            [TestCase]
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