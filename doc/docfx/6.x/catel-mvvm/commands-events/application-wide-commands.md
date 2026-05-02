---
title: "CommandManager and command containers (Application-wide commands)" 
---
Most commands are registered per view and available per view model. Some commands (such as commands on a `Ribbon` or `Toolbar`) are application-wide. Catel supports both types, and this part of the documentation explains how to use the `ICommandManager` to work with application-wide commands such as `Refresh` with a key bound to `F5`.

## CommandManager

There is no generic way to specify application-wide commands in XAML platforms. To overcome this issue, Catel introduces the *CommandManager*. This manager allows to create commands which are hosted by the *CommandManager*. The commands on the command manager can be created with input gestures. Once a view model wants to hook into a specific command, it only has to register the view model command with the application-wide command.

Note that application-wide commands by default are only available on the main window of an application. To support this on other windows, add the following code in the constructor of a window:

```
public class SomeWindow
{
    private readonly CommandManagerWrapper _commandManagerWrapper;

    public SomeWindow()
    {
        InitializeComponent();

        _commandManagerWrapper = new CommandManagerWrapper(this);
    }
}
```

## Creating application-wide commands

To create application-wide commands, one must resolve the *ICommandManager* from the *DependencyResolver *and create the command:

```
var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
var commandManager = dependencyResolver.Resolve<ICommandManager>();

commandManager.CreateCommand("Refresh", new InputGesture(Key.F5));
```

It is recommended to put all the command creation in a single place so they are easily manageable.

### Registering a custom command

When a view model wants to use application-wide specific commands, the only thing it has to do is register the command in the *CommandManager*.

```
public class CommandSubscribingViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;

    public CommandSubscribingViewModel(ICommandManager commandManager, IMessageService messageService)
    {
        Argument.IsNotNull(() => commandManager);
        Argument.IsNotNull(() => messageService);

        _messageService = messageService;

        ExampleCommand = new Command(OnExampleCommandExecute);
        commandManager.RegisterCommand("Refresh", ExampleCommand, this);
    }

    public Command ExampleCommand { get; private set; }

    private void OnExampleCommandExecute()
    {
        _messageService.Show("Application-wide command executed");
    }
}
```

### Using application-wide commands in xaml

To make it easy to bind to application-wide commands, Catel provides the *CommandManagerBinding *markup extension. To bind to commands in xaml, use the following code:

```
<Ribbon catel:StackGrid.ColumnSpan="4">
    <RibbonTab Header="Home" KeyTip="H" >
        <RibbonGroup Header="Example commands">
            <RibbonButton Command="{catel:CommandManagerBinding Refresh}" LargeImageSource="..\Resources\Images\Refresh.png" 
                          Label="Refresh" KeyTip="F5" />
        </RibbonGroup>
    </RibbonTab>
</Ribbon>
```

As the code shows, the *CommandManagerBinding* extension automatically resolves the *Refresh* command from the *CommandManager*.

## Command containers

When implementing a ribbon or any menu structure inside an application can result in a very complex view model containing all the commands. Catel solves this issue by implementing so-called command containers. These are containers that have only 1 purpose: contain a command so the logic can easily be viewed / edited and the commands will be available during the whole lifetime of the app. Internally command containers use the *ICommandManager* to register commands, so the *ICommandManager* is still responsible for the commands.

### Creating a command container

Creating a command container is very simple. It can be done by creating a class deriving from *CommandContainerBase* as shown in the example below:

```
public class ApplicationAboutCommandContainer : CommandContainerBase
{
    private readonly IAboutService _aboutService;

    public ApplicationAboutCommandContainer(ICommandManager commandManager, IAboutService aboutService)
        : base(Commands.Application.About, commandManager)
    {
        Argument.IsNotNull(() => aboutService);

        _aboutService = aboutService;
    }

    protected override Execute(object parameter)
    {
        _aboutService.ShowAbout();
    }
}
```

As you can see the implementation is very clean and won't pollute any other view models.

### Registering a command container

If you don't use the extension methods below, you must register the command container inside the service locator and register the command inside the *ICommandManager*. To make this process easier, use a definition file and the code below.

#### Command definitions

To make it very easy to register new commands, Catel uses naming conventions and extension methods. The name of the command (for example, *About* must be a constant on the command definitions class). If the command definition also contains a *\<CommandName\>InputGesture*, in this case *AboutInputGesture*, it will use that input gesture as a default to register the command with.

```
public static class Commands
{
    public static class Application
    {
        public const string Exit = "Application.Exit";
        public static readonly InputGesture ExitInputGesture = new InputGesture(Key.F4, ModifierKeys.Alt);

        public const string About = "Application.About";
        public static readonly InputGesture AboutInputGesture = new InputGesture(Key.F1);
    }

    public static class OtherPartOfApplication
    {
        public const string SomeCommand = "OtherPartOfApplication.SomeCommand";
        public static readonly InputGesture SomeCommandInputGesture = null;
    }
}
```

It is recommended to keep a well formed structure for your command definitions to keep them manageable, even in very large applications

#### Registering the command container

Once you have the command container and the command definition (command name and the input gesture), it is time to register the command container:

```
var commandManager = ServiceLocator.Default.ResolveType<ICommandManager>();

commandManager.CreateCommandWithGesture(typeof(Commands.Application), "About");
```

This will keep the command registration very readable and maintainable when using a lot of commands:

```
var commandManager = ServiceLocator.Default.ResolveType<ICommandManager>();

commandManager.CreateCommandWithGesture(typeof(AppCommands.Application), "Exit");
commandManager.CreateCommandWithGesture(typeof(AppCommands.Application), "About");

commandManager.CreateCommandWithGesture(typeof(Commands.Project), "Open");
commandManager.CreateCommandWithGesture(typeof(Commands.Project), "Save");
commandManager.CreateCommandWithGesture(typeof(Commands.Project), "SaveAs");
commandManager.CreateCommandWithGesture(typeof(Commands.Project), "Refresh");

commandManager.CreateCommandWithGesture(typeof(AppCommands.Settings), "ToggleTooltips");
commandManager.CreateCommandWithGesture(typeof(AppCommands.Settings), "ToggleQuickFilters");

commandManager.CreateCommandWithGesture(typeof(ExtensibilityCommands.Application), "Extensions");
commandManager.CreateCommandWithGesture(typeof(ExtensibilityCommands.Application), "ExtensionsSettings");
```

