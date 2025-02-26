Catel
=====

Name|Badge
---|---
Downloads|![NuGet downloads](https://img.shields.io/nuget/dt/catel.core.svg)
NuGet stable version|![Version](https://img.shields.io/nuget/v/catel.core.svg)
NuGet unstable version|![Pre-release version](https://img.shields.io/nuget/vpre/catel.core.svg)
MyGet unstable version|![Pre-release version](https://img.shields.io/myget/catel/vpre/catel.core.svg)
Open Collective|[![Backers on Open Collective](https://opencollective.com/Catel/backers/badge.svg)](#backers) [![Sponsors on Open Collective](https://opencollective.com/Catel/sponsors/badge.svg)](#sponsors)

Catel is an application development platform with the focus on MVVM (WPF, UWP, Xamarin.Android, Xamarin.iOS and Xamarin.Forms). 
The goal of Catel is to provide a complete set of modular functionality for Line of Business applications written in any .NET 
technology, from client to server.

Catel distinguishes itself by unique features to aid in the development of MVVM applications and server-side application 
development. Since Catel focuses on Line of Business applications, it provides professional support and excellent documentation 
which ensures a safe bet by professional companies and developers.

For documentation, please visit the [documentation portal](https://docs.catelproject.com)

## Features and components

Below are a few features that are available in Catel.

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

Catel.MVVM is the library you want to include when you are writing a UI project (e.g. WPF, UWP, Xamarin) and you want to use the MVVM pattern. Catel is the *only* MVVM library that has context-aware view and view-model creation, which can be used to solve the [nested user controls problem](https://docs.catelproject.com/vnext/introduction/mvvm/introduction-to-nested-user-controls-problem/). 

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

```csharp
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

```csharp
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

## How to contribute

### Support on Open Collective

Please consider supporting [Catel on Open Collective](https://opencollective.com/catel).

### Translating

To add translations to Catel, the Multilingual App Toolkit (MAT) is required. 

1. Download the [MAT Editor](https://developer.microsoft.com/en-us/windows/develop/multilingual-app-toolkit)
2. Open your specific language (or create it) in the `MultilingualResources` folder, e.g. `./src/Catel.MVVM/MultilingualResources/Catel.MVVM.nl.xlf`
3. Edit the xlf file and create a pull request (PR) with only this file

## Building Catel

**Prerequisites** 

Catel requires Visual Studio 2019 to compile successfully. You also need to ensure you have the following features installed:

Note that the `.vsconfig` in the src root should notify about missing components when opening the solution.

- .NET desktop development
- Universal Windows Platform development
- Mobile development with .NET
- .NET Core cross-platform development
- Select the following components from Individual components
  - Windows 10 SDK (10.0.16299.0) 
  - MSVC v141 and v142 (ARM, ARM64 and x64/x86) 
  - C++ ATL v141 and v142 (ARM, ARM64 and x86 & x64)  build tools
  - C++ MFC v141 and v142 (ARM, ARM64 and x86 & x64) build tools
- [Latest Version of .NET Core 3.0 Preview SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)
  - Ensure you enable **Use previews of the .NET Core SDK** under Tools -> Options -> Projects and Solutions -> .NET Core

Note that you should run these commands using powershell in the root of the repository.

### Running a build

`.\build.ps1 -target build`

### Running a build with unit tests

`.\build.ps1 -target buildandtest`

### Running a build with local packages

Note that this assumes a local packages directory at `C:\Source\_packages`, which can be added to the NuGet feeds:

![](doc/nuget_local_packages.png)

`.\build.ps1 -target buildandpackagelocal`

## Apps using Catel

There are a lot of (both free & commercial) apps using Catel. This list provides a few examples that are built 
with Catel:

// todo: add wpf

// todo: add uwp

## WPF components based on Catel

If you are planning on using WPF, there is a huge set (60+) of free open-source components 
available based on Catel. All these open source are developed by a company called WildGums 
(see https://www.wildgums.com) and provided to the community for free. The components are well 
maintained and being used in several commercial WPF applications.

For more information, see https://github.com/wildgums

## Tools for Catel
 * **[Analogy.LogViewer.CatelProject](https://github.com/Analogy-LogViewer/Analogy.LogViewer.CatelProject)** - Catel Log parser for [Analogy Log Viewer](https://github.com/Analogy-LogViewer/Analogy.LogViewer).
 
## Contributors

This project exists thanks to all the people who contribute. [[Contribute](CONTRIBUTING.md)].
<a href="graphs/contributors"><img src="https://opencollective.com/Catel/contributors.svg?width=890&button=false" /></a>

## Backers

Thank you to all our backers! 🙏 [[Become a backer](https://opencollective.com/Catel#backer)]

<a href="https://opencollective.com/Catel#backers" target="_blank"><img src="https://opencollective.com/Catel/backers.svg?width=890"></a>

## Sponsors

Support this project by becoming a sponsor. Your logo will show up here with a link to your website. [[Become a sponsor](https://opencollective.com/Catel#sponsor)]

<a href="https://opencollective.com/Catel/sponsor/0/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/0/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/1/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/1/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/2/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/2/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/3/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/3/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/4/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/4/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/5/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/5/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/6/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/6/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/7/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/7/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/8/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/8/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/9/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/9/avatar.svg"></a>
