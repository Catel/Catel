---
title: "Serializing data from/to disk" 
---
> **Note:** The Catel serialization engine (`IXmlSerializer`, `SavableModelBase`) was removed in Catel 7. The `ServiceLocator` IoC container was also removed in Catel 7 in favor of standard .NET dependency injection.
>
> For serialization, this guide uses `Orc.Serialization.Json`. Other alternatives include `System.Text.Json` and `Newtonsoft.Json`.
> For dependency injection, register services in `IServiceCollection` using `services.AddCatelCore()` and `services.AddCatelMvvm()`.

In this step we will create services that will serialize the models from/to disk. Services are a great way to abstract functionality that can be used in every part of the application.

## Creating the service definition

The first thing to do is to create the *Services* folder to group the services. Below is a screenshot of how to solution will look after creating the folders:

![](../../images/getting-started/wpf/serializing-data-from-to-disk/solutionexplorer.png)

Then add a new interface to the `Interfaces` folder named `IFamilyService`. This will manage the families that are avaiable. Below is the interface defined:

```csharp
namespace WPF.GettingStarted.Services
{
    using WPF.GettingStarted.Models;

    public interface IFamilyService
    {
        IEnumerable<Family> LoadFamilies();
        void SaveFamilies(IEnumerable<Family> families);
    }
}
```

## Creating the service implementation

First, add the `Orc.Serialization.Json` and `Orc.FileSystem` NuGet packages to the project:

```
dotnet add package Orc.Serialization.Json
dotnet add package Orc.FileSystem
```

Below is an example implementation using `Orc.Serialization.Json` for serialization and `Orc.FileSystem` for file access:

```csharp
namespace WPF.GettingStarted.Services
{
    using System.Collections.Generic;
    using System.IO;
    using Orc.FileSystem;
    using Orc.Serialization.Json;
    using WPF.GettingStarted.Models;

    public class FamilyService : IFamilyService
    {
        private readonly string _path;
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IJsonSerializer _jsonSerializer;

        public FamilyService(IFileService fileService, IDirectoryService directoryService,
            IJsonSerializerFactory jsonSerializerFactory)
        {
            ArgumentNullException.ThrowIfNull(fileService);
            ArgumentNullException.ThrowIfNull(directoryService);
            ArgumentNullException.ThrowIfNull(jsonSerializerFactory);

            _fileService = fileService;
            _directoryService = directoryService;
            _jsonSerializer = jsonSerializerFactory.CreateSerializer();

            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CatenaLogic", "WPF.GettingStarted");

            _directoryService.Create(directory);
            _path = Path.Combine(directory, "family.json");
        }

        public IEnumerable<Family> LoadFamilies()
        {
            if (!_fileService.Exists(_path))
            {
                return Array.Empty<Family>();
            }

            using var stream = _fileService.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return _jsonSerializer.Deserialize<List<Family>>(stream) ?? new List<Family>();
        }

        public void SaveFamilies(IEnumerable<Family> families)
        {
            using var stream = _fileService.Create(_path);
            _jsonSerializer.Serialize(stream, families);
        }
    }
}
```

## Registering the service in the service collection

Now we have created the service, it is time to register it in the service collection. In the `App.xaml.cs`, add the following code:

```csharp
// AddOrcFileSystem registers IFileService and IDirectoryService, which FamilyService depends on
services.AddOrcFileSystem();
// AddOrcSerializationJson registers IJsonSerializerFactory, which FamilyService depends on
services.AddOrcSerializationJson();
services.AddSingleton<IFamilyService, FamilyService>();
```

The call to `AddOrcFileSystem()` registers `IFileService` and `IDirectoryService`. The call to `AddOrcSerializationJson()` registers the `IJsonSerializerFactory` which is injected into `FamilyService`.

## Adding the service usage to the MainWindowViewModel

Now the service is registered, it can be used anywhere in the application. A great place to load and save the families is in the `MainWindowViewModel` which contains all the logic of the main application window. 

### Injecting the service via dependency injection

To get an instance of the service in the view model, change the constructor to the following definition.

```csharp
private readonly IFamilyService _familyService;

/// <summary>
/// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
/// </summary>
public MainWindowViewModel(IServiceProvider serviceProvider, IFamilyService familyService)
    : base(serviceProvider)
{
    _familyService = familyService;
}
```

### Creating the Families property on the MainWindowViewModel

The next thing we need is a `Families` property on the `MainWindowViewModel` to store the families we load from disk. Below is the property definition for that:

```csharp
/// <summary>
/// Gets the families.
/// </summary>
public ObservableCollection<Family> Families
{
    get { return GetValue<ObservableCollection<Family>>(FamiliesProperty); }
    private set { SetValue(FamiliesProperty, value); }
}

/// <summary>
/// Register the Families property so it is known in the class.
/// </summary>
public static readonly PropertyData FamiliesProperty = RegisterProperty<ObservableCollection<Family>>(nameof(Families));
```

### Loading the families at startup

Now we have the `IFamilyService` and the `Families` property, it is time to combine these two. To do this, we need to override the `InitializeAsync` method on the view model which is automatically called as soon as the view is loaded by Catel:

```csharp
protected override async Task InitializeAsync()
{
    var families = _familyService.LoadFamilies();
    Families = new ObservableCollection<Family>(families);
}
```

### Saving the families at shutdown

To save the families at shutdown, override the `CloseAsync` method on the view model which is automatically called as soon as the view is closed by Catel:

```csharp
protected override async Task CloseAsync()
{
    _familyService.SaveFamilies(Families);
}
```

After running the application once, a new file will be stored in the following directory:

*C:\\Users\\[yourusername]\\AppData\\Roaming\\CatenaLogic\\WPF.GettingStarted*

## Up next

[Creating the view models]({{< relref "getting-started/wpf/creating-the-view-models.md" >}})


