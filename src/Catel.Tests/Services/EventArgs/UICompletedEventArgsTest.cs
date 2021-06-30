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
            var completedEventArgs = new UICompletedEventArgs(new UIVisualizerResult(true, new UIVisualizerContext
            {
                Data = 15
            }, null));

            Assert.AreEqual(15, completedEventArgs.Context.Data);
            Assert.AreEqual(true, completedEventArgs.Result.DialogResult);
        }
        #endregion
    }
}
