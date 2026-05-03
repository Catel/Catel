---
title: "NavigationService" 
---
The `INavigationService` allows a developer to navigate to other pages inside an application using view models only.

All pages will have to be registered manually or following the right naming convention.

The `NavigationService` uses the `IViewLocator` and `IViewModelLocator` do handle the navigation. See the [Naming conventions](../locators-naming-conventions/index.md) section for more details.

## Closing an application

It is possible to close an application using the following code:

```csharp
private readonly INavigationService _navigationService;

public MyViewModel(IServiceProvider serviceProvider, INavigationService navigationService)
    : base(serviceProvider)
{
    _navigationService = navigationService;
}
```

```csharp
await _navigationService.CloseApplicationAsync();
```

## Preventing an application to be closed

To prevent an application to be closed, one can subscribe to the `ApplicationClosing` event:

```csharp
_navigationService.ApplicationClosing += (sender, e) 
=>
{
   e.Cancel = true;
};
```

## Navigating to a new view

To navigate to a new page, use the following code:

```csharp
await _navigationService.NavigateAsync<EmployeeViewModel>();
```

## Navigating with parameters

It is easy to navigate to a new page with parameters. Use the following code:

```csharp
var parameters = new Dictionary<string, object>();
parameters.Add("id", employee.EmployeeID);

await _navigationService.NavigateAsync<EmployeeViewModel>(parameters);
```

To read the navigation parameters in the receiving view model, use the `OnNavigationCompleted` method.

## Navigating back and forward

The service also supports navigating back and forward:

```csharp
await _navigationService.GoBackAsync(); // navigates to the previous page, obviously
await _navigationService.GoForwardAsync(); // navigates to the next page, obviously
```


