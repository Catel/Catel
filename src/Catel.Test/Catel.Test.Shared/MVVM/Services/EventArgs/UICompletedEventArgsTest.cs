// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UICompletedEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Services.EventArgs
{
    using Catel.Services;

    using NUnit.Framework;

    [TestFixture]
    public class UICompletedEventArgsTest
    {
        #region Methods
        [TestCase]
        public void UICompletedEventArgs_Constructor()
        {
            UICompletedEventArgs completedEventArgs = new UICompletedEventArgs(15, true);

            Assert.AreEqual(15, completedEventArgs.DataContext);
            Assert.AreEqual(true, completedEventArgs.Result);
        }
        #endregion
    }
}