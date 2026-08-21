---
title: "Creating the WPF project" 
---
In this step we will create the project and add the relevant NuGet packages.

## Creating the project

Create a new WPF Application project targeting `net10.0-windows` (or another supported .NET version). You can do this via Visual Studio (*File => New Project => WPF Application*) or from the command line:

```
dotnet new wpf -n Catel.Examples.WPF.PersonApplication
```

## Adding the NuGet packages

Add the following NuGet packages to the project:

```xml
<PackageReference Include="Catel.MVVM" Version="7.0.0" />
<PackageReference Include="Catel.Fody" Version="7.0.0" PrivateAssets="all" />
<PackageReference Include="Catel.SourceGenerators" Version="7.0.1" PrivateAssets="all" />
<PackageReference Include="Fody" Version="6.9.3" PrivateAssets="all" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
```

- **Catel.MVVM** – the core Catel MVVM library.
- **Catel.Fody** – a Fody weaver that auto-generates Catel property boilerplate at compile time.
- **Catel.SourceGenerators** – Roslyn source generators that complement Catel.
- **Microsoft.Extensions.Hosting** – provides the .NET generic host for DI and application lifecycle management.

## Project structure

Create the following folders to organise the application:

| Folder | Contents |
|--------|----------|
| `Models` | Data models (`Person`, `Gender`) |
| `ViewModels` | View models (`MainWindowViewModel`, `PersonViewModel`) |
| `Views` | Views (`MainWindow`, `PersonWindow`) |
| `Converters` | Value converters (`GenderToIsSelectedConverter`) |

## Up next

[Creating the models]({{< relref "getting-started/wpf/creating-the-models.md" >}})

