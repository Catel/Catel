// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UICompletedEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Services.EventArgs
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
            var completedEventArgs = new UICompletedEventArgs(new UIVisualizerResult(true, 15, null, null));

            Assert.AreEqual(15, completedEventArgs.DataContext);
            Assert.AreEqual(true, completedEventArgs.DialogResult);
        }
        #endregion
    }
}
