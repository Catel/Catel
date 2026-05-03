---
title: 'EventArgsConverter'
---
The implementation and usage of the `EventToCommand` behavior is always strongly encouraged when developing an MVVM application as it provides a logical separation of concerns. 

However many implementations of the behavior come with the caveat, they cause a ViewModel to have knowledge of View specific types as well as causing a ViewModel to be directly dependent on the View.

To resolve that concern, Catel provides support for an `EventArgsConverter` that can be attached to an `EventToCommand` binding. This allows a developer to provide a more agnostic transport object between the View and the ViewModel for event handling. Be aware that usage of an `EventArgsConverter` does come with its own caveats.

As the ViewModel will no longer have any direct knowledge of the `EventArgs` being passed (because the `EventArgsConverter` translates it to an agnostic object), you will not be able to execute any behavior against the `EventArgs` (such as an `e.Handled` call) and any short-circuiting you must do must be done within the `EventArgsConverter`. 

If you need more fine grain behavior, you will need to stick with what's called a **Mixed-Mode Implementation** where the Event is implemented in the View Code-Behind and it manipulates the ViewModel through an Interface.

With that, proceed with a basic implementation of an `EventArgsConverter` that will handle the `PreviewMouseDoubleClick` event against a `ListBoxItem`.

---

The following code creates a `PersonModel` that will hold some basic information about a person.

```
public class PersonModel : ModelBase
{
    private static int _nextId;

    public PersonModel()
    {
        Id = Interlocked.Increment(ref _nextId);
    }

    public string FirstName { get; set; }

    public int Id { get; set; }

    public string LastName { get; set; }
}
```

The following code creates our ViewModel to aid in the display and manipulation of people:

```
public class MainViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;

    public MainViewModel(IMessageService messageService)
    {
        Argument.IsNotNull(() => messageService);

        _messageService = messageService;

        ShowItemAsyncCommand = new TaskCommand<PersonModel>(OnShowItemAsync);
    }

    public FastObservableCollection<PersonModel> Items { get; set; } = new FastObservableCollection<PersonModel>();

    public TaskCommand<PersonModel> ShowItemAsyncCommand { get; }

    protected override Task InitializeAsync()
    {
        using ( Items.SuspendChangeNotifications() )
        {
            Items.Add(new PersonModel {FirstName = "John", LastName = "Doe"});
            Items.Add(new PersonModel {FirstName = "Jane", LastName = "Doe"});
        }

        return base.InitializeAsync();
    }

    private async Task OnShowItemAsync(PersonModel model)
    {
        await _messageService.ShowInformationAsync($"Id: {model.Id} - Name: {model.FirstName} {model.LastName}");
    }
}
```

With both the Model and ViewModel created we need to create our `EventArgsConverter` before we create the View that will handle the communication between both the View and ViewModel.

```
public class PersonListEventArgsConverter : EventArgsConverterBase<MouseEventArgs>
{
    protected override object Convert(object sender, MouseEventArgs args)
    {
        if ( !( args.OriginalSource is DependencyObject dependencyObject ) )
        {
            return null;
        }

        ListBoxItem item = ParentOfType<ListBoxItem>(dependencyObject);

        if ( !( item.DataContext is PersonModel model ) )
        {
            return null;
        }

        return model;
    }

    private static T ParentOfType<T>(DependencyObject dependencyObject)
        where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);
        if ( parent == null )
        {
            return default;
        }

        T result = parent as T;
        return result ?? ParentOfType<T>(parent);
    }
}
```

As shown, the `Convert` method is an event handler that will handle `MouseEventArgs` and does the following:

- Checks if the `OriginalSource` implements a `DependencyObject`
- Grabs the `ListBoxItem` from the `DependencyObject` using `ParentyOfType<T>`
- Checks if the `DataContext` of the `ListBoxItem` implements `PersonModel`
- Returns the `PersonModel` which will be ultimately tunneled to `MainViewModel.OnShowItemAsync`

This demonstrates that the `EventArgsConverter` acts solely as a unidirectional transport because `MouseEventArgs` is lost once everything is tunneled to `MainViewModel`. With that implementation now complete, we can implement the view.

```
<catel:Window x:Class="WpfApp1.Views.MainWindow"
              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
              xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
              xmlns:catel="http://schemas.catelproject.com"
              xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
              xmlns:converters="clr-namespace:WpfApp1.Converters"
              mc:Ignorable="d"
              Title="MainWindow" Height="450" Width="800">

    <catel:Window.Resources>
        <converters:PersonListEventArgsConverter x:Key="PersonListEventArgsConverter" />
    </catel:Window.Resources>

    <ListBox ItemsSource="{Binding Items}">
        <i:Interaction.Triggers>
            <i:EventTrigger EventName="PreviewMouseDoubleClick">
                <catel:EventToCommand Command="{Binding ShowItemAsyncCommand}" PassEventArgsToCommand="True" EventArgsConverter="{StaticResource PersonListEventArgsConverter}" />
            </i:EventTrigger>
        </i:Interaction.Triggers>
        <ListBox.ItemTemplate>
            <DataTemplate>
                <VirtualizingStackPanel Orientation="Horizontal">
                    <TextBlock Text="{Binding FirstName}" />
                    <TextBlock Text="{Binding LastName}" Margin="5 0 0 0" />
                </VirtualizingStackPanel>
            </DataTemplate>
        </ListBox.ItemTemplate>
    </ListBox>
</catel:Window>
```

As shown above, we have defined an `EventTrigger` that references the `PreviewMouseDoubleClick` event and relies on `EventToCommand`. We are then telling `EventToCommand` that we want to pass event arguments by setting `PassEventArgsToCommand` to `true` and providing it with the `PersonListEventArgsConverter` which it will use instead of directly passing the `MouseEventArgs` to `MainViewModel`

Now when a person double clicks on any of the items within the `ListBox` they will be presented with a modal window that will show the first and last name of the person in question.

We hope that this information helps you with the development of your application.

