---
title: "Quick introduction for developers" 
---
This is a quick introduction for developers who don't have a lot of time to read all the docs. This document contains the absolute basics of what a developer needs to know.

## Core

This pare contains the core functionality of Catel and what you should know when using Catel.

### Logging / debugging

If you ever think Catel is behaving strange or does not work as expected, make sure to enable the logging. Below is an example on how to enable the logging:

```
#if DEBUG
    LogManager.RegisterDebugListener();
#endif
```

Catel will then log everything to the output window and provide all the information about its internals.

For more information, read about [logging](../../catel-core/logging/index.md).

### Catel properties

All properties in classes deriving from *ModelBase* (thus also *ViewModelBase*) require a special property definition.

Normally one would write something like this:

```
private string _firstName;
public string FirstName
{
    get { return _firstName; }
    set
    {
        RaisePropertyChanging("FirstName");
        _firstName = value;
        RaisePropertyChanged("FirstName");
    }
}
```

In Catel one should write this:

```
public string FirstName
{
    get { return GetValue< string>(FirstNameProperty); }
    set { SetValue(FirstNameProperty, value); }
}

public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);
```

Catel will automatically take care of change notifications.

Note that you can use the *modelprop* or *vmprop* to easily create these properties using code snippets. You can also use [Catel.Fody]({{< relref "catel-fody/_index.md" >}}) instead

## MVVM

This part is especially meant for the MVVM part.

### Handling of viewmodels

In other MVVM frameworks, you are obliged to set the data context of a view manually. It will look something like this:

```
var view = new PersonView();
view.DataContext = new PersonViewModel();
```

Catel automatically resolves the right view model based on the view. If a view is created, Catel automatically creates the view model:

```
var view = new PersonView();
// view model is automatically created
```

It goes even further. Catel can create view models based on the data context. For more information, read [nested user controls](../../introduction/mvvm/introduction-to-nested-user-controls-problem.md).

### Handling hierarchy and parent/child view models

Note that Catel is already fully aware of parent/child relations of view models so you don’t have to do anything for this yourself. For more information, read nested user controls.

### Resolving views and view models

Catel resolves views and view models by naming convention. This means that based on the name of a view, the view model can be determined. This also works the other way around where the view model can be determined based on the view. For more information, read about [naming conventions](https://catelproject.atlassian.net/wiki/display/CTL/Locators+and+naming+conventions).

