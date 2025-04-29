namespace Catel.Tests.MVVM.Commands
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Moq;
    using Catel.MVVM;
    using System.Windows.Input;

    public class ICommandExtensionsFacts
    {
        [TestFixture]
        public class The_GetTask_Method
        {
            [Test]
            public void Returns_Task_From_ICatelTaskCommand()
            {
                // Arrange
                var expectedTask = Task.FromResult(42);
                var mockTaskCommand = new Mock<ICatelTaskCommand>();
                mockTaskCommand.Setup(c => c.Task).Returns(expectedTask);

                // Act
                var result = ICommandExtensions.GetTask(mockTaskCommand.Object);

                // Assert
                Assert.That(result, Is.EqualTo(expectedTask));
            }

            [Test]
            public void Returns_CompletedTask_For_Non_ICatelTaskCommand()
            {
                // Arrange
                var mockCommand = new Mock<ICommand>();

                // Act
                var result = ICommandExtensions.GetTask(mockCommand.Object);

                // Assert
                Assert.That(result, Is.EqualTo(Task.CompletedTask));
            }
        }
    }
}
