// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UICompletedEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Services.EventArgs
{
    using Catel.MVVM.Services;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class UICompletedEventArgsTest
    {
        #region Methods
        [TestMethod]
        public void UICompletedEventArgs_Constructor()
        {
            UICompletedEventArgs completedEventArgs = new UICompletedEventArgs(15, true);

            Assert.AreEqual(15, completedEventArgs.DataContext);
            Assert.AreEqual(true, completedEventArgs.Result);
        }
        #endregion
    }
}