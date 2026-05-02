---
title: "ViewModelLocator" 
description: ""
---
Starting with Catel 3.0, there are several ways to hook up a view model to the view. When a view is constructed, an MVVM behavior is added to the view. Thanks to these MVVM behaviors, it is possible to use exactly the same logic on 3rd party controls.

{{% notice warning %}}
Note that the while using the conventions, magic words such as "View", "Control", "UserControl", "Window" and "Page" will be stripped from the view name while locating the view model type
{{% /notice %}}

## Resolving by naming convention

If the `GetViewModelType` method returns `null` (which is the default behavior), the view will resolve the `IViewModelLocator` from the `ServiceLocator`. Then it will use the `ResolveViewModel` method to resolve the view model based on the type of the view.

For example, the following view:

```
Catel.Examples.Views.MyView
```

will be resolved as:

```
Catel.Examples.ViewModels.MyViewModel
```

## Manually resolving a view model using naming convention

To manually resolve a view model using naming convention, use the following code:

```
var viewModelLocator = ServiceLocator.Default.ResolveType<IViewModelLocator>();
var viewModelType = viewModelLocator.ResolveViewModel(typeof(MyView));
```

## Customizing naming conventions

By default, the *IViewModelLocator* uses the following naming conventions to resolve view models:

-   [UP].ViewModels.[VW]ViewModel
-   [UP].ViewModels.[VW]ControlViewModel
-   [UP].ViewModels.[VW]WindowViewModel
-   [UP].ViewModels.[VW]PageViewModel
-   [UP].ViewModels.[VW]ActivityViewModel
-   [UP].ViewModels.[VW]FragmentViewModel
-   [AS].ViewModels.[VW]ViewModel
-   [AS].ViewModels.[VW]ControlViewModel
-   [AS].ViewModels.[VW]WindowViewModel
-   [AS].ViewModels.[VW]PageViewModel
-   [AS].ViewModels.[VW]ActivityViewModel
-   [AS].ViewModels.[VW]FragmentViewModel
-   [CURRENT].[VW]ViewModel
-   [CURRENT].[VW]ControlViewModel
-   [CURRENT].[VW]WindowViewModel
-   [CURRENT].[VW]PageViewModel
-   [CURRENT].[VW]ActivityViewModel
-   [CURRENT].[VW]FragmentViewModel

{{% notice info %}}
For more information about naming conventions, see [Naming conventions]({{< relref "catel-mvvm/locators-naming-conventions/naming-conventions.md" >}})
{{% /notice %}}

However, it is possible to add or remove new naming conventions to support your own naming convention. For example, to add a new naming convention for a different assembly, use this code:

```
var viewModelLocator = ServiceLocator.Default.ResolveType<IViewModelLocator>();
viewModelLocator.NamingConventions.Add("MyCustomAssembly.ViewModels.[VW]ViewModel");
```

## Registering custom view models

Sometimes, a class doesn't follow a naming convention (for whatever reason possible). In such a case, it is possible to register a mapping manually using the following code:

```
var viewModelLocator = ServiceLocator.Default.ResolveType<IViewModelLocator>();
viewModelLocator.Register(typeof(MyViewNotFollowingNamingConvention), typeof(MyViewModel));
```

## Using a custom ViewModelLocator

If you want to have total freedom to determine which view model is provided per view (maybe there are other services that have an impact on this), it is possible to create a custom `IViewModelLocator` implementation. Then the only thing to do is to register it using the following code:

```
ServiceLocator.Default.Register<IViewModelLocator, MyViewModelLocator>();
```

## Using a generic implementation of the view

Last but not least, it is still possible to use the "old-fashioned" way by using the generic view bases. These classes directly derive from the non-generic views and return the generic type definition of the view model using the `GetViewModelType` method.

