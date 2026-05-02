---
title: "Catel Documentation - 6.x"
---
# Catel

Catel is an application development platform with the focus on MVVM (WPF & Universal Windows Platform). 
The goal of Catel is to provide a complete set of modular functionality for Line of Business applications written in any .NET 
technology, from client to server.

Catel distinguishes itself by unique features to aid in the development of MVVM applications and server-side application 
development. Since Catel focuses on Line of Business applications, it provides professional support and excellent documentation 
which ensures a safe bet by professional companies and developers.

## Major features

Below are a few major features that are available in Catel.

### Catel.Core

Catel.Core is the library you want to include in all your projects, whether you are writing a UI project or not. It contains lots of useful helper methods. The
most important features are listed below:

- Argument validation (e.g. `Argument.IsNotNull(() => myArgument)`)
- Caching
- Data (ModelBase, PropertyBag, Validation)
- IoC (ServiceLocator, TypeFactory)
- Logging (LogManager, Log, several log listeners)
- Messaging
- Reflection (same reflection API for *every supported platform*)
- Serialization (XmlSerializer and JsonSerializer)
- Weak references (WeakEventListener)

And more.... 

### Catel.MVVM

Catel.MVVM is the library you want to include when you are writing a UI project (e.g. WPF, UWP, Xamarin) and you want to use the MVVM pattern. Catel is the *only* MVVM library that has context-aware view and view-model creation, which can be used to solve the [nested user controls problem](http://docs.catelproject.com/vnext/introduction/mvvm/introduction-to-nested-user-controls-problem/). 

The most important
features are listed below:

- Auditing
- Collections (FastObservableCollection, FastObservableDictionary, etc)
- Commands (Command, TaskCommand, etc)
- Converters (tons of converters out of the box)
- Services
    - BusyIndicatorService
    - MessageService
    - NavigationService
    - OpenFileService
    - SaveFileService
    - UIVisualizerService
- View models
    - Automatic validation
    - Automatic mappings from model to view model
- Views
    - DataWindow
    - UserControl
    - Window

## Example code

### Models

This model has automatic change notifications and validation.

```
public class Person : ValidatableModelBase
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        if (string.IsNullOrWhitespace(FirstName))
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(FirstName), "First name is required"));
        }

        if (string.IsNullOrWhitespace(LastName))
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(LastName), "Last name is required"));
        }
    }    
}
```

### View models

This is a view model with:

- Automatic injection of the DataContext
- Automatic mapping of properties & validation from model => view model

```
public class PersonViewModel : ViewModelBase
{
    public PersonViewModel(Person person)
    {
        Argument.IsNotNull(() => person);

        Person = person;
    }

    [Model]
    private Person Person { get; set; }

    [ViewModelToModel]
    public string FirstName { get; set; }

    [ViewModelToModel]
    public string LastName { get; set; }
}
```

### Continue reading

