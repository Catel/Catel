namespace Catel.Tests.MVVM.Commands;

using System;
using System.Linq;
using System.Reflection;
using Catel.IoC;
using Catel.MVVM;
using Catel.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

public class IServiceCollectionExtensionsFacts
{
    // Test container with command fields
    public static class TestCommands
    {
        public const string TestCommand = "TestCommand";

        public const string TestCommandWithGesture = "TestCommandWithGesture";
        public static readonly InputGesture TestCommandWithGestureInputGesture = new InputGesture(System.Windows.Input.Key.T, System.Windows.Input.ModifierKeys.Control);

        public const string AnotherCommand = "AnotherCommand";
        public static readonly InputGesture AnotherCommandInputGesture = new InputGesture(System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift);
    }

    public class TestCommandCommandContainer
    {
    }

    public class TestCommandWithGestureCommandContainer
    {
    }

    public class AnotherCommandCommandContainer
    {
    }

    // Test container with gesture fields
    public static class TestCommandsWithGesture
    {
        public const string Command = "GestureCommand";
        public static readonly InputGesture CommandInputGesture = new InputGesture(System.Windows.Input.Key.G, System.Windows.Input.ModifierKeys.Control);
    }

    public class GestureCommandCommandContainer
    {
    }

    [TestFixture]
    public class The_AddCommandWithInputGesture_Method
    {
        [Test]
        public void Throws_ArgumentNullException_When_ServiceCollection_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                IServiceCollectionExtensions.AddCommandWithInputGesture(
                    null!, 
                    typeof(TestCommands), 
                    nameof(TestCommands.TestCommand)));
        }

        [Test]
        public void Throws_ArgumentNullException_When_ContainerType_Is_Null()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            Assert.Throws<ArgumentNullException>(() =>
                serviceCollection.AddCommandWithInputGesture(
                    null!, 
                    nameof(TestCommands.TestCommand)));
        }

        [Test]
        public void Throws_ArgumentException_When_CommandNameFieldName_Is_Empty()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            Assert.Throws<ArgumentException>(() =>
                serviceCollection.AddCommandWithInputGesture(
                    typeof(TestCommands), 
                    string.Empty));
        }

        [Test]
        public void Throws_ArgumentException_When_CommandNameFieldName_Is_Whitespace()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            Assert.Throws<ArgumentException>(() =>
                serviceCollection.AddCommandWithInputGesture(
                    typeof(TestCommands), 
                    "   "));
        }

        [Test]
        public void Throws_InvalidOperationException_When_Field_Not_Found()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            Assert.Throws<InvalidOperationException>(() =>
                serviceCollection.AddCommandWithInputGesture(
                    typeof(TestCommands), 
                    "NonExistentField"));
        }

        [Test]
        public void Registers_CommandContainerRegistration_When_Only_CommandName_Field_Exists()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            serviceCollection.AddCommandWithInputGesture(
                typeof(TestCommands), 
                nameof(TestCommands.TestCommand));

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var registrations = serviceProvider.GetServices<CommandContainerRegistration>();

            Assert.That(registrations, Is.Not.Empty);
        }

        [Test]
        public void Registers_CommandContainerRegistration_With_InputGesture_When_Both_Fields_Exist()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            serviceCollection.AddCommandWithInputGesture(
                typeof(TestCommands), 
                nameof(TestCommands.TestCommandWithGesture));

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var registrations = serviceProvider.GetServices<CommandContainerRegistration>();

            Assert.That(registrations, Is.Not.Empty);
        }

        [Test]
        public void Registers_Multiple_Commands_Independently()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            serviceCollection.AddCommandWithInputGesture(
                typeof(TestCommands), 
                nameof(TestCommands.TestCommand));

            serviceCollection.AddCommandWithInputGesture(
                typeof(TestCommands), 
                nameof(TestCommands.AnotherCommand));

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var registrations = serviceProvider.GetServices<CommandContainerRegistration>();

            Assert.That(registrations.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Creates_And_Registers_Command_In_CommandManager()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            serviceCollection.AddCommandWithInputGesture(
                typeof(TestCommands), 
                nameof(TestCommands.TestCommand));

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandManager = serviceProvider.GetRequiredService<ICommandManager>();

            serviceProvider.CreateTypesThatMustBeConstructedAtStartup();

            Assert.That(commandManager.IsCommandCreated(TestCommands.TestCommand), Is.True);
        }

        [Test]
        public void Creates_Command_With_InputGesture_When_Available()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            serviceCollection.AddCommandWithInputGesture(
                typeof(TestCommands), 
                nameof(TestCommands.TestCommandWithGesture));

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandManager = serviceProvider.GetRequiredService<ICommandManager>();

            serviceProvider.CreateTypesThatMustBeConstructedAtStartup();

            var command = commandManager.GetCommand(TestCommands.TestCommandWithGesture);
            Assert.That(command, Is.Not.Null);

            // Verify gesture was registered
            var commandsByGesture = commandManager.FindCommandsByGesture(TestCommands.TestCommandWithGestureInputGesture);
            Assert.That(commandsByGesture.Any(), Is.True);
        }

        [Test]
        public void Handles_Null_InputGesture_Field()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            // This test ensures that if the InputGesture field is not found, 
            // the command is still registered without a gesture
            serviceCollection.AddCommandWithInputGesture(
                typeof(TestCommands), 
                nameof(TestCommands.TestCommand));

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var registrations = serviceProvider.GetServices<CommandContainerRegistration>();
            Assert.That(registrations, Is.Not.Empty);
        }

        [Test]
        public void Throws_CatelException_When_CommandName_Field_Returns_Null()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            // Create a test class with a null field value
            var invalidContainer = typeof(InvalidCommandContainer);

            Assert.Throws<CatelException>(() =>
                serviceCollection.AddCommandWithInputGesture(
                    invalidContainer, 
                    nameof(InvalidCommandContainer.NullCommandNameField)));
        }

        [Test]
        public void Respects_BindingFlags_Public_Static()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            // Ensure it only looks at public static fields, not private or instance fields
            Assert.Throws<InvalidOperationException>(() =>
                serviceCollection.AddCommandWithInputGesture(
                    typeof(PrivateFieldContainer), 
                    "PrivateCommand"));
        }
    }

    // Helper class with null command name
    private static class InvalidCommandContainer
    {
        public static readonly string? NullCommandNameField = null;
    }

    // Helper class with private field (should not be found)
    private static class PrivateFieldContainer
    {
        private const string PrivateCommand = "Private";
    }
}
