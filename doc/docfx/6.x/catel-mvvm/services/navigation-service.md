---
title: "NavigationService" 
---
The `INavigationService` allows a developer to navigate to other pages inside an application using view models only.

All pages will have to be registered manually or following the right naming convention.

The `NavigationService` uses the `IViewLocator` and `IViewModelLocator` do handle the navigation. See the [Naming conventions](../locators-naming-conventions/index.md) section for more details.

## Closing an application

It is possible to close an application using the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var navigationService = dependencyResolver.Resolve<INavigationService>();
navigationService.CloseApplication();
```

## Preventing an application to be closed

To prevent an application to be closed, one can subscribe to the `ApplicationClosing` event:

```
var dependencyResolver = this.GetDependencyResolver();
var navigationService = dependencyResolver.Resolve<INavigationService>();
navigationService.ApplicationClosing += (sender, e) 
=>
{
   e.Cancel = true;
};
```

## Navigating to a new view

To navigate to a new page, use the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var navigationService = dependencyResolver.Resolve<INavigationService>();
navigationService.Navigate<EmployeeViewModel>();
```

## Navigating with parameters

It is very easy to navigate to a new page with parameters. Use the following code:

```
var parameters = new Dictionary<string, object>();
parameters.Add("id", employee.EmployeeID);

var dependencyResolver = this.GetDependencyResolver();
var navigationService = dependencyResolver.Resolve<INavigationService>();
navigationService.Navigate<EmployeeViewModel>(parameters);
```

To read the navigation parameters in the receiving view model, use the `OnNavigationCompleted` method.

## Navigating back and forward

The service also supports navigating back and forward:

```
var dependencyResolver = this.GetDependencyResolver();
var navigationService = dependencyResolver.Resolve<INavigationService>();
navigationService.GoBack(); // navigates to the previous page, obviously
navigationService.GoForward(); // navigates to the next page, obviously
```

