---
title: "Creating the views (windows)" 
---
In this step we will create the two windows of the application: `MainWindow` and `PersonWindow`.

## MainWindow

`MainWindow` is the application's main view. It derives from `catel:Window` (a Catel-aware WPF window) and displays the list of persons with Add, Edit, and Remove buttons.

### XAML

```xml
<catel:Window x:Class="Catel.Examples.PersonApplication.Views.MainWindow"
              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:catel="http://schemas.catelproject.com"
              xmlns:xamlbehaviors="http://schemas.microsoft.com/xaml/behaviors"
              MinHeight="350"
              MinWidth="525"
              ShowInTaskbar="True"
              SizeToContent="Manual"
              WindowStartupLocation="Manual"
              ResizeMode="CanResizeWithGrip"
              WindowState="Maximized">

    <Window.Resources>
        <Style x:Key="ImageButtonStyle" TargetType="Button">
            <Setter Property="Width" Value="48" />
            <Setter Property="Height" Value="48" />
            <Setter Property="Margin" Value="6" />
            <Setter Property="Padding" Value="6" />
        </Style>
    </Window.Resources>

    <DockPanel LastChildFill="True">
        <!-- Action buttons -->
        <StackPanel DockPanel.Dock="Right" Orientation="Vertical">
            <WrapPanel Orientation="Vertical">
                <Button Command="{Binding Add}" ToolTip="Add" Style="{StaticResource ImageButtonStyle}">
                    <Image Source="/Resources/Images/add.png" />
                </Button>
                <Button Command="{Binding Edit}" ToolTip="Edit" Style="{StaticResource ImageButtonStyle}">
                    <Image Source="/Resources/Images/edit.png" />
                </Button>
                <Button Command="{Binding Remove}" ToolTip="Remove" Style="{StaticResource ImageButtonStyle}">
                    <Image Source="/Resources/Images/delete.png" />
                </Button>
            </WrapPanel>
        </StackPanel>

        <!-- List of persons -->
        <ListBox DockPanel.Dock="Left"
                 ItemsSource="{Binding PersonCollection}"
                 SelectedItem="{Binding SelectedPerson}">
            <xamlbehaviors:Interaction.Triggers>
                <xamlbehaviors:EventTrigger EventName="MouseDoubleClick">
                    <catel:EventToCommand Command="{Binding Edit}"
                                          DisableAssociatedObjectOnCannotExecute="False" />
                </xamlbehaviors:EventTrigger>
            </xamlbehaviors:Interaction.Triggers>

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
    </DockPanel>
</catel:Window>
```

### Code-behind

The code-behind is minimal — Catel resolves the view model automatically by naming convention (`MainWindow` → `MainWindowViewModel`):

```csharp
namespace Catel.Examples.PersonApplication.Views;

public partial class MainWindow
{
}
```

## PersonWindow

`PersonWindow` is a `catel:DataWindow`. The `DataWindow` automatically provides **OK** and **Cancel** buttons and integrates with `IEditableObject` so that cancelling the dialog reverts all model changes.

### Value converter

The `PersonWindow` uses a `GenderToIsSelectedConverter` to bind `RadioButton.IsChecked` to the `Gender` enum. Add the converter to the `Converters` folder:

```csharp
namespace Catel.Examples.PersonApplication.Converters;

using System;
using System.Windows.Data;
using Catel.Examples.PersonApplication.Models;
using Catel.MVVM.Converters;

[ValueConversion(typeof(Gender), typeof(bool), ParameterType = typeof(Gender))]
public class GenderToIsSelectedConverter : ValueConverterBase<Gender>
{
    protected override object Convert(Gender value, Type targetType, object parameter)
    {
        Gender genderRepresented = ParseGenderParameter(parameter);
        return (value == genderRepresented);
    }

    protected override object ConvertBack(object value, Type targetType, object parameter)
    {
        Gender genderRepresented = ParseGenderParameter(parameter);

        bool isChecked = value is bool b && b;
        return isChecked ? genderRepresented : Binding.DoNothing;
    }

    private static Gender ParseGenderParameter(object parameter) => parameter switch
    {
        Gender g => g,
        string s => (Gender)Enum.Parse(typeof(Gender), s),
        _ => Gender.Unknown
    };
}
```

### XAML

```xml
<catel:DataWindow x:Class="Catel.Examples.PersonApplication.Views.PersonWindow"
                  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:converters="clr-namespace:Catel.Examples.PersonApplication.Converters"
                  xmlns:catel="http://schemas.catelproject.com"
                  xmlns:Models="clr-namespace:Catel.Examples.PersonApplication.Models">

    <catel:DataWindow.Resources>
        <converters:GenderToIsSelectedConverter x:Key="GenderToIsSelectedConverter" />
    </catel:DataWindow.Resources>

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition Width="*" MinWidth="300" />
        </Grid.ColumnDefinitions>

        <!-- Gender -->
        <Label Grid.Row="0" Grid.Column="0" Content="Gender" />
        <StackPanel Grid.Row="0" Grid.Column="1" Orientation="Horizontal" x:Name="genderStackPanel">
            <RadioButton Content="Male"
                         IsChecked="{Binding Gender, Converter={StaticResource GenderToIsSelectedConverter},
                                     ConverterParameter={x:Static Models:Gender.Male},
                                     ValidatesOnDataErrors=True, NotifyOnValidationError=True}"
                         Validation.ValidationAdornerSiteFor="{Binding ElementName=genderStackPanel}" />
            <RadioButton Content="Female"
                         IsChecked="{Binding Gender, Converter={StaticResource GenderToIsSelectedConverter},
                                     ConverterParameter={x:Static Models:Gender.Female},
                                     ValidatesOnDataErrors=True, NotifyOnValidationError=True}"
                         Validation.ValidationAdornerSiteFor="{Binding ElementName=genderStackPanel}" />
        </StackPanel>

        <!-- First name -->
        <Label Grid.Row="1" Grid.Column="0" Content="First name" />
        <TextBox Grid.Row="1" Grid.Column="1"
                 Text="{Binding FirstName, ValidatesOnDataErrors=True, NotifyOnValidationError=True}" />

        <!-- Middle name -->
        <Label Grid.Row="2" Grid.Column="0" Content="Middle name" />
        <TextBox Grid.Row="2" Grid.Column="1"
                 Text="{Binding MiddleName, ValidatesOnDataErrors=True, NotifyOnValidationError=True}" />

        <!-- Last name -->
        <Label Grid.Row="3" Grid.Column="0" Content="Last name" />
        <TextBox Grid.Row="3" Grid.Column="1"
                 Text="{Binding LastName, ValidatesOnDataErrors=True, NotifyOnValidationError=True}" />
    </Grid>
</catel:DataWindow>
```

### Code-behind

The code-behind configures the window mode and registers any additional custom buttons:

```csharp
namespace Catel.Examples.PersonApplication.Views;

using Catel.Examples.PersonApplication.ViewModels;
using Catel.Services;
using Catel.Windows;
using System;

public partial class PersonWindow
{
    public PersonWindow(PersonViewModel viewModel, IServiceProvider serviceProvider,
        IWrapControlService wrapControlService, ILanguageService languageService)
        : base(viewModel, serviceProvider, wrapControlService, languageService)
    {
        Mode = DataWindowMode.OkCancel;
        DefaultButton = DataWindowDefaultButton.OK;
        InfoBarMessageControlGenerationMode = InfoBarMessageControlGenerationMode.Inline;

        InitializeComponent();
    }
}
```

`DataWindowMode.OkCancel` tells the `DataWindow` to show standard **OK** and **Cancel** buttons. Clicking **OK** saves the view model; clicking **Cancel** reverts all changes via `IEditableObject.CancelEdit`.

## Up next

[Hooking up everything together]({{< relref "getting-started/wpf/hooking-up-everything-together.md" >}})


