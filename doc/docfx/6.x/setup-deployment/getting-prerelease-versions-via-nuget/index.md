---
title: "Getting prerelease (beta) versions via NuGet & MyGet" 
---
## Adding the custom package source

Starting with Catel v5, the alpha prereleases are only available on the [MyGet feed for Catel](https://www.myget.org/feed/Packages/catel). Therefore you must first add the custom url to the NuGet Package Manager. The easiest way to do this is via Visual Studio.

1.  Go to *Tools* =\> *NuGet Package Manager* =\> *Package Manager Settings*
2.  Select *Package Sources*
3.  Click the + button at the right top and use the following values at the bottom:
    Name: MyGet - Catel
    Source: <https://www.myget.org/F/catel/api/v3/index.json>

## Installing via package manager

Please make sure to select the same settings as in the screenshow below:

![](../../images/setup-deployment/getting-prerelease-versions-via-nuget/nuget.png)

## Installing via package manager console

This example installs Catel.MVVM as a package. However, to install other packages simple change the ID (name) of the package.

**Installing the latest beta**

```
Install-Package Catel.MVVM –IncludePrerelease
```

**Installing a specific beta**

```
Install-Package Catel.MVVM –IncludePrerelease -version 5.0.0-unstable0532
```

**Updating to the latest beta**

```
Update-Package Catel.MVVM –IncludePrerelease
```

**Updating to a specific beta**

```
Update-Package Catel.MVVM –IncludePrerelease -version 5.0.0-unstable0532
```

**Updating to the latest stable version**

```
Update-Package Catel.MVVM
```

