---
title: 'Catel 4.0'
description: ""
weight: 400
---
This guide describes how to update your code to be fully compatible with Catel 4.0.

## Renaming classes

Some classes in Catel have been renamed.

-   Catel.Environment =\> CatelEnvironment
-   IDependencyPropertySelector =\> IViewPropertySelector

## Renaming namespaces

Some namespaces in Catel have been changed to match the functionality. For example, all services are now inÂ *Catel.Service* instead ofÂ *Catel.MVVM.Services* because they can be used without MVVM.

Below is a list of changed namespaces:

-   Catel.MVVM.Services =\> Catel.Services
-   Catel.Windows.Data.Converters =\> Catel.MVVM.Converters

-   Catel.Windows.Controls.MVVMProviders.Logic =\>Â Catel.MVVM.Providers

Some interfaces were moved (but not all classes in the namespace):

-   Catel.Windows =\> Catel.MVVM.Views

## Simplied ModelBase external interfacing

TheÂ *ModelBase* class exposed a lot of properties for validation such asÂ *HasErrors, HasFieldErrors, HasBusinessRuleErrors*, etc. These all these properties are now explicitly implemented into interfaces to make models usingÂ *ModelBase* cleaner to use for end-developers. Below is a list of properties that are now implemented as explicit interface implementations:

Member name

Explicit interface implementation

Member type

BusinessRuleErrorCount

IModelValidation.BusinessRuleErrorCount

property

BusinessRuleWarningCount

IModelValidation.BusinessRuleWarningCount

property

Deserialized

IModelSerialization.Deserialized

event

ErrorsChanged

INotifyDataErrorInfo.ErrorsChanged

event

FieldErrorCount

IModelValidation.FieldErrorCount

property

FieldWarningCount

IModelValidation.FieldWarningCount

property

HasErrors

INotifyDataErrorInfo.HasErrors

property

HasWarnings

INotifyDataWarningInfo.HasWarnings

property

ValidationContext

IModelValidation.ValidationContext

property

Validator

IModelValidation.Validator

property

WarningsChanged

INotifyDataWarningInfo.WarningsChanged

event

## Renamed LoadTabItemsBehavior

TheÂ *LoadTabItemsBehavior* has been refactored with new names. The old names will be removed in v5, but will error in v4. Below are the renames:

-   Single =\> LazyLoading
-   SingleUnloadOthers =\> LazyLoadingUnloadOthers
-   AllOnStartup =\>Â EagerLoadingÂ 
-   AllOnFirstUse =\>Â EagerLoadingOnFirstUseÂ 

## Add additional members to custom IView implementations

To support Xamarin, theÂ *IView* interface has been extended with new members. Make sure to implement the new members.

## Using FastViewPropertySelector by default for major performance improvement

When not using theÂ *ViewToViewModel* attributes, it is not required to subscribe to all dependency properties in theÂ *UserControlLogic*. Starting from Catel 4.0, Catel uses theÂ *FastViewPropertySelector* by default which subscribes to no properties by default. This is a breaking change for users using theÂ *ViewToViewModel* attribute.

To get back the behavior, there are 2 ways:

### Manually add interesting properties (recommended)

It is best to let Catel only subscribe to the properties that it should (for the best performance). To do so, use theÂ *IViewPropertySelector.AddPropertyToSubscribe* method to add properties:

```
var serviceLocator = ServiceLocator.Default;
var viewPropertySelector = serviceLocator.ResolveType<IViewPropertySelector>();


viewPropertySelector.AddPropertyToSubscribe("MyProperty", typeof(MyView));
```

In most cases, the only reason to subscribe to property changes is because of theÂ *ViewToViewModelÂ *attribute. If that is the case, it is best to use the extension methodÂ *AutoDetectViewPropertiesToSubscribe* in the static constructor of the view:

```
static MyView()
{
    typeof(MyView).AutoDetectViewPropertiesToSubscribe();
}
```

### Register the ViewPropertySelector

The default implementation of theÂ *ViewPropertySelector* subscribes to all properties by default. By registering it in theÂ *ServiceLocator* will ensure Catel subscribes to all dependency properties:

```
var serviceLocator = ServiceLocator.Default;
serviceLocator.RegisterType<IViewPropertySelector, ViewPropertySelector>();
```

## Full support for asynchronous (async/await)

### IViewModel

TheÂ *IViewModel* interface now returns tasks instead of direct values to support async/await.

#### Updating Initialize method

```
public override void Initialize()
{
    base.Initialize();
}
```

Must be changed in:

```
public override async Task Initialize()
{
    await base.Initialize();
}
```

#### Updating Save method

```
public override bool Save()
{
    return base.Save();
}
```

Must be changed in:

```
public override async Task<bool?> Save()
{
    return await base.Save();
}
```

#### Updating Close method

```
public override void Close()
{
    base.Close();
}
```

Must be changed in:

```
public override async Task Close()
{
    await base.Close();
}
```

### IMessageService

The use of *await* or *Task.ContinueWith* to await the result is now necessary or use the code below:

```
if (await messageService.ShowInfo("message", other parameters...) == MessageBoxResult.Yes)
{
    // Handle yes here
}
```

### IUIVisualizerService

The use ofÂ *await*Â orÂ *Task.ContinueWith*Â to await the result is now necessary or use the code below:

```
await uiVisualizerService.ShowDialog<MyViewModel>();
Â 
// Window is closed here thanks to the await keyword
```

## Optimizing views (especially 3rd party)

Catel 4.0 introduces a much simpler way to use Catel on 3rd party controls. This means that the following changes have been applied and might be breaking:

-   Removed *GetViewModelType*() from view base classes. Instead use the *IViewModelLocator* to ensure Catel can find the view models. Note that it is possible to manually register a custom view with a view model in case the view / view model don't match any naming convention.
-   Removed *GetViewModelInstance*() from view base classes. Customize the *IViewModelFactory* instead.
-   RemovedÂ *ValidateData*,Â *DiscardChanges* andÂ *ApplyChanges* from all views except *DataWindow*
-   Merged *ViewLoaded* and *Loaded* events on *IViewModelContainer* and *IView* interfaces
-   RenamedÂ *ViewLoading*Â and *ViewUnloadingÂ *events onÂ *IViewModelContainerÂ *toÂ *Loading* andÂ *Unloading*
-   Renamed *IViewLoadedManagerÂ *toÂ *IViewLoadManager*

## Removed IServiceLocator.RemoveInstance methods

The *IServiceLocator.Remove[x]* methods are removed. Use the *RemoveType* methods instead.

## Changed CompositeCommand

The composite command will always allow execution, even when commands don't allow it. Therefore the *AllowPartialExecutionÂ *is now set to *falseÂ *by default.

If there is a requirement to allow partial invocation, set this property to *true*.

## Added time to all log calls

TheÂ *time* parameter has been added to all log calls. This is a breaking change for all classes implementing *ILogListener*.

## Behavior changes

To improve multiple platforms support, all parameters of the following methods on *BehaviorBase* have been removed:

```
OnAssociatedObjectLoaded(object sender, EventArgs e) => OnAssociatedObjectLoaded()
Â 
OnAssociatedObjectUnloaded(object sender, EventArgs e) => OnAssociatedObjectUnloaded()
```

Â 

Â 

