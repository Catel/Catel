---
title: "Creating the view models" 
---
In this step we will create the view models. A view model is a class that derives from `ViewModelBase` (or `FeaturedViewModelBase` for additional features) and contains all the presentation logic for a view.

## Creating the PersonViewModel

The `PersonViewModel` is responsible for editing a single `Person`. It receives the `Person` model via constructor injection (provided automatically by Catel's `IUIVisualizerService`).

```csharp
namespace Catel.Examples.PersonApplication.ViewModels;

using System;
using System.Collections.Generic;
using Catel.Data;
using Catel.Examples.PersonApplication.Models;
using Catel.MVVM;

public class PersonViewModel : FeaturedViewModelBase
{
    public PersonViewModel(Person person, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        if (Catel.CatelEnvironment.IsInDesignMode)
        {
            return;
        }

        Person = person;

        Title = "Person";
    }

    [Model]
    [Fody.Expose("FirstName")]
    [Fody.Expose("MiddleName")]
    public Person Person { get; private set; }

    [ViewModelToModel("Person")]
    public Gender Gender { get; set; }

    [ViewModelToModel("Person")]
    public string LastName { get; set; }
}
```

### Key attributes explained

| Attribute | Effect |
|-----------|--------|
| `[Model]` | Marks the property as the backing model. Catel automatically calls `IEditableObject.BeginEdit` / `EndEdit` / `CancelEdit` when the view model is saved or cancelled. |
| `[Fody.Expose("FirstName")]` | Catel.Fody generates a `FirstName` property on the view model that is kept in sync with `Person.FirstName`. |
| `[ViewModelToModel("Person")]` | Catel keeps the view model property and the named model property in sync automatically, without manual mapping code. |

## Creating the MainWindowViewModel

The `MainWindowViewModel` manages the list of persons shown in the main window and exposes commands for adding, editing, and removing persons.

```csharp
namespace Catel.Examples.PersonApplication.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Catel.Examples.PersonApplication.Models;
using Catel.MVVM;
using Catel.Services;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;

    public MainWindowViewModel(IServiceProvider serviceProvider, IUIVisualizerService uiVisualizerService,
        IMessageService messageService)
        : base(serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(uiVisualizerService);
        ArgumentNullException.ThrowIfNull(messageService);

        _uiVisualizerService = uiVisualizerService;
        _messageService = messageService;

        Add = new TaskCommand(serviceProvider, OnAddExecuteAsync);
        Edit = new TaskCommand(serviceProvider, OnEditExecuteAsync, OnEditCanExecute);
        Remove = new TaskCommand(serviceProvider, OnRemoveExecuteAsync, OnRemoveCanExecute);

        PersonCollection = new ObservableCollection<Person>();
        PersonCollection.Add(new Person { Gender = Gender.Male, FirstName = "Geert", MiddleName = "van", LastName = "Horrik" });
        PersonCollection.Add(new Person { Gender = Gender.Male, FirstName = "Fred", MiddleName = string.Empty, LastName = "Retteket" });

        Title = "Person Application";
    }

    public ObservableCollection<Person> PersonCollection { get; private set; }

    public Person SelectedPerson { get; set; }

    public TaskCommand Add { get; private set; }

    private async Task OnAddExecuteAsync()
    {
        var person = new Person();

        var result = await _uiVisualizerService.ShowDialogAsync<PersonViewModel>(person);
        if (result.DialogResult ?? false)
        {
            PersonCollection.Add(person);
        }
    }

    public TaskCommand Edit { get; private set; }

    private bool OnEditCanExecute()
    {
        return (SelectedPerson is not null);
    }

    private async Task OnEditExecuteAsync()
    {
        await _uiVisualizerService.ShowDialogAsync<PersonViewModel>(SelectedPerson);
    }

    public TaskCommand Remove { get; private set; }

    private bool OnRemoveCanExecute()
    {
        return (SelectedPerson is not null);
    }

    private async Task OnRemoveExecuteAsync()
    {
        if (await _messageService.ShowAsync("Are you sure you want to remove this person?", "Are you sure?", MessageButton.YesNo) == MessageResult.Yes)
        {
            PersonCollection.Remove(SelectedPerson);
        }
    }
}
```

### Dependency injection

Both services (`IUIVisualizerService` and `IMessageService`) are registered automatically by `services.AddCatelMvvm()` and are injected via the constructor.

`TaskCommand` requires the `IServiceProvider` so that Catel can manage command execution correctly on the dispatcher thread.

## Up next

[Creating the views (windows)]({{< relref "getting-started/wpf/creating-the-windows.md" >}})

