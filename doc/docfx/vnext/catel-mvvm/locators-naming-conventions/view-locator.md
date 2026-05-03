---
title: "ViewLocator" 
---
The `IViewLocator` class is responsible for resolving the right views for a view model. The `UIVisualizerService` internally uses the `IViewLocator` to resolve the views. 

## Resolving by naming convention

It is possible to resolve views using the `IViewLocator`. Then you can use the `ResolveView` method to resolve the view based on the type of the view model.

For example, the following view model:

```
Catel.Examples.ViewModels.MyViewModel
```

will be resolved as:

```
Catel.Examples.Views.MyView
```

## Manually resolving a view using naming convention

To manually resolve a view using naming convention, inject `IViewLocator` via the constructor and use:

```csharp
var viewType = _viewLocator.ResolveView(typeof(MyViewModel));
```

## Customizing naming conventions

By default, the `IViewLocator` uses the following naming conventions to resolve views:

- [UP].Views.[VM]
- [UP].Views.[VM]View
- [UP].Views.[VM]Control
- [UP].Views.[VM]Window
- [UP].Views.[VM]Page
- [UP].Views.[VM]Activity
- [UP].Views.[VM]Fragment
- [UP].Controls.[VM]
- [UP].Controls.[VM]Control
- [UP].Pages.[VM]
- [UP].Pages.[VM]Page
- [UP].Windows.[VM]
- [UP].Windows.[VM]Window
- [AS].Views.[VM]
- [AS].Views.[VM]View
- [AS].Views.[VM]Control
- [AS].Views.[VM]Page
- [AS].Views.[VM]Window
- [AS].Views.[VM]Activity
- [AS].Views.[VM]Fragment
- [AS].Controls.[VM]
- [AS].Controls.[VM]Control
- [AS].Pages.[VM]
- [AS].Pages.[VM]Page
- [AS].Windows.[VM]
- [AS].Windows.[VM]Window
- [AS].Activities.[VM]
- [AS].Activities.[VM]Activity
- [AS].Fragments.[VM]
- [AS].Fragments.[VM]Fragment
- [AS].UI.Views.[VM]
- [AS].UI.Views.[VM]View
- [AS].UI.Views.[VM]Control
- [AS].UI.Views.[VM]Page
- [AS].UI.Views.[VM]Window
- [AS].UI.Views.[VM]Activity
- [AS].UI.Views.[VM]Fragment
- [AS].UI.Controls.[VM]
- [AS].UI.Controls.[VM]Control
- [AS].UI.Pages.[VM]
- [AS].UI.Pages.[VM]Page
- [AS].UI.Windows.[VM]
- [AS].UI.Windows.[VM]Window
- [AS].UI.Activities.[VM]
- [AS].UI.Activities.[VM]Activity
- [AS].UI.Activities.[VM]Fragment
- [CURRENT].[VM]View
- [CURRENT].[VM]Control
- [CURRENT].[VM]Page
- [CURRENT].[VM]Window
- [CURRENT].[VM]Activity
- [CURRENT].[VM]Fragment

For more information about naming conventions, see [Naming conventions]({{< relref "catel-mvvm/locators-naming-conventions/naming-conventions.md" >}})

However, it is possible to add or remove new naming conventions to support your own naming convention. For example, to add a new naming convention for a different assembly, use this code:

```csharp
_viewLocator.NamingConventions.Add("MyCustomAssembly.Views.[VM]View");
```

## Registering custom views

Sometimes, a class does not follow a naming convention (for whatever reason possible). In such a case, it is possible to register a mapping manually using the following code:

```csharp
_viewLocator.Register(typeof(MyViewModelNotFollowingNamingConvention), typeof(MyView));
```

## Using a custom ViewLocator

If you want to have total freedom to determine which view is provided per view model (maybe there are other services that have an impact on this), it is possible to create a custom `IViewLocator` implementation. Register it in the service collection:

```csharp
services.AddSingleton<IViewLocator, MyViewLocator>();
```


