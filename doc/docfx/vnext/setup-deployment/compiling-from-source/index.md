---
title: 'Compiling from source'
---
In order to compile, the following 3rd party software is required:

- Visual Studio 2022

All other libraries required are retrieved via NuGet.

Note that the `.vsconfig` in the `src` root should notify about missing components when opening the solution.

## Building

Note that you should run these commands using PowerShell in the root of the repository.

### Running a build

```
.\build.ps1 -target build
```

### Running a build with unit tests

```
.\build.ps1 -target buildandtest
```

### Running a build with local packages

Note that this assumes a local packages directory at `C:\Source\_packages`, which can be added to the NuGet feeds.

```
.\build.ps1 -target buildandpackagelocal
```

