---
title: "MVVM" 
description: ""
weight: 20
---
## How to support example data with view models?

To find out how to create design time data, see the [designers](Designers)topic.

## How to use events with MVVM?

When writing MVVM, it's "forbidden" (read: not a best practice) to use click handlers (or other UI events) in your view-model. But then should you react to events?

1.  Start with creating a command like you are used to using MVVM. This command will be executed when the event occurs.
2.  Add a reference to *System.Windows.Interactivity.dll* (ships with Catel). If you have used NuGet to add a reference, it is automatically included for you.
3.  Add the following namespace definitions to your view declaration:

    ```
    xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
    xmlns:catel="http://schemas.catelproject.com"
    ```

4.  Use the following code to convert an event to a command:

    ```
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="[YourEvent]">
            <catel:EventToCommand Command="{Binding [YourCommand]}" DisableAssociatedObjectOnCannotExecute="False" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
    ```

An example for aÂ *ListBoxÂ *double click:

```
<ListBox ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="MouseDoubleClick">
            <catel:EventToCommand Command="{Binding Edit}" DisableAssociatedObjectOnCannotExecute="False" />
        </i:EventTrigger>
    </i:Interaction.Triggers>

    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <Label Content="{Binding FirstName}" />
                <Label Content="{Binding MiddleName}" />
                <Label Content="{Binding LastName}" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

## How can I add the MVVM behaviors via code (programmatically)?

Below is the code-behind of a view that adds the *UserControlBehavior* via code:

```
public partial class DynamicBehaviorView : UserControl, IViewModelContainer
{
   private Catel.Windows.Controls.MVVMProviders.UserControlBehavior _mvvmBehavior;

   /// <summary>
   /// Initializes a new instance of the <see cref="DynamicBehaviorView"/> class.
   /// </summary>
   public DynamicBehaviorView()
   {
      InitializeComponent();
      _mvvmBehavior = new Catel.Windows.Controls.MVVMProviders.UserControlBehavior();
      _mvvmBehavior.ViewModelType = typeof(ViewModels.MyViewModel);
      System.Windows.Interactivity.Interaction.GetBehaviors(this).Add(_mvvmBehavior);
      _mvvmBehavior.ViewModelChanged += (sender, e) => ViewModelChanged.SafeInvoke(this, e);
      _mvvmBehavior.ViewModelPropertyChanged += (sender, e) => ViewModelPropertyChanged.SafeInvoke(this, e);
   }

   /// <summary>
   /// Gets the view model that is contained by the container.
   /// </summary>
   /// <value>The view model.</value>
   public IViewModel ViewModel
   {
      get { return _mvvmBehavior.ViewModel; }
   }

   /// <summary>
   /// Occurs when the <see cref="ViewModel"/> property has changed.
   /// </summary>
   public event EventHandler<EventArgs> ViewModelChanged;

   /// <summary>
   /// Occurs when a property on the <see cref="ViewModel"/> has changed.
   /// </summary>
   public event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;

   /// <summary>
   /// Occurs when a property on the container has changed.
   /// </summary>
   /// <remarks>
   /// This event makes it possible to externally subscribe to property changes of a <see cref="DependencyObject"/>
   /// (mostly the container of a view model) because the .NET Framework does not allows us to.
   /// </remarks>
   public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

   /// <summary>
   /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
   /// </summary>
   /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
   protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs(e.Property.Name));
   }         
}
```

Using this technique, it is even possible to determine the view model of any view dynamically at runtime.

## How can I inject or manipulate the view model of a UserControl?

The *UserControl* is a very powerful control. It allows lazy loaded dynamic view model construction. However, sometimes you just don't want the user control to dynamically create the view model. Luckily, the user control instantiates a new view model with this logic:

1.  The *DataContext* of the control can be injected into a constructor of the view model
2.  The view model has an empty constructor

Â You can set the DataContext of the control to a view model, and this way "inject" a view model into a control instead of letting it be created first. In fact, the user control first checks whether the DataContext is already a valid view model for the user control. If so, it keeps it that way.

## How can I prevent validation of required fields?

Catel does not validate the properties with data annotations at startup. It will only validate the data annotations when properties change or when the view model is about to be saved. This is implemented this way to allow a developer to show required fields with an asterisk (\*) instead of errors. If a developer still wants to initially display errors, only a single call has to be made in the constructor:

```
Validate(true, false);
```

If the validation is implemented in the models and not in the view model, set the *ValidateModelsOnInitialization* to false.

Â 

