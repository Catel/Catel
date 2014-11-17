// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelServiceBaseTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Services
{
    using Catel.Services;

    using NUnit.Framework;

    [TestFixture]
    public class ViewModelServiceBaseTest
    {
        #region Classes
        private class ViewModelService : ViewModelServiceBase
        {
        }
        #endregion

        #region Methods
        [TestCase]
        public void Name()
        {
            var testService = new ViewModelService();

            Assert.AreEqual("ViewModelService", testService.Name);
        }
        #endregion
    }
}