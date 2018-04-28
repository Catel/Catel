Catel
=====

Name|Badge
---|---
Chat|[![Join the chat at https://gitter.im/catel/catel](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/catel/catel?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
Downloads|![NuGet downloads](https://img.shields.io/nuget/dt/catel.core.svg)
NuGet stable version|![Version](https://img.shields.io/nuget/v/catel.core.svg)
NuGet unstable version|![Pre-release version](https://img.shields.io/nuget/vpre/catel.core.svg)
MyGet unstable version|![Pre-release version](https://img.shields.io/myget/catel/vpre/catel.core.svg)

Catel is an application development platform with the focus on MVVM (WPF, UWP, Xamarin.Android, Xamarin.iOS and Xamarin.Forms). 
The goal of Catel is to provide a complete set of modular functionality for Line of Business applications written in any .NET 
technology, from client to server.

Catel distinguishes itself by unique features to aid in the development of MVVM applications and server-side application 
development. Since Catel focuses on Line of Business applications, it provides professional support and excellent documentation 
which ensures a safe bet by professional companies and developers.

For documentation, please visit the [documentation portal](http://docs.catelproject.com)

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
- Serialization (BinarySerializer, XmlSerializer and more)
- Weak references (WeakEventListener)

And more.... 

### Catel.MVVM

Catel.MVVM is the library you want to include when you are writing a UI project (e.g. WPF, UWP, Xamarin) and you want to use the MVVM pattern. Catel is the *only* MVVM library that has context-aware view and view-model creation, which can be used to solve the [nested user controls problem](http://docs.catelproject.com/vnext/introduction/mvvm/introduction-to-nested-user-controls-problem/). 

The most important
features are listed below:

- Auditing
- Collections (FastObservableCollection)
- Commands (Command, TaskCommand, etc)
- Converters (tons of converters out of the box)
- Services
	- CameraService	
	- LocationService
	- MessageService
	- NavigationService
	- OpenFileService
	- PleaseWaitService
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

## Apps using Catel

There are a lot of (both free & commercial) apps using Catel. This list provides a few examples that are built 
with Catel:

// todo: add wpf

// todo: add uwp

## WPF components based on Catel

If you are planning on using WPF, there is a huge set (60+) of free open-source components 
available based on Catel. All these open source are developed by a company called WildGums 
(see http://www.wildgums.com) and provided to the community for free. The components are well 
maintained and being used in several commercial WPF applications.

For more information, see https://github.com/wildgums