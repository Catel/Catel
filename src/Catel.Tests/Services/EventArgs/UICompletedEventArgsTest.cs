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

            Assert.That(completedEventArgs.Context.Data, Is.EqualTo(15));
            Assert.That(completedEventArgs.Result.DialogResult, Is.EqualTo(true));
        }
        #endregion
    }
}
