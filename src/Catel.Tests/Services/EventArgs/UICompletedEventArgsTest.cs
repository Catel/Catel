namespace Catel.Tests.Services
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
