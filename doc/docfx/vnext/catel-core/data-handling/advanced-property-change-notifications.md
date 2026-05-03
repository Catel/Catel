---
title: "Advanced property change notifications" 
---
Sometimes the old value is needed in case of property change event. However, the `INotifyPropertyChanged` interface does not provide any of this. To support this behavior, a new version of the `PropertyChangedEventArgs` is created called `AdvancedPropertyChangedEventArgs`. This class derives from `PropertyChangedEventArgs` so the interfaces are not broken, but it does add additional functionality to the software system.

## Getting old value automatically

When using the `ModelBase` or `ViewModelBase` classes, the old and new value of a property are automatically provided on a property change. There are two ways to get more information about a property change event.

### Getting more information from the inside

The easiest way to get more information on the inside is to override the `OnPropertyChanged` method. It automatically provides an instance of the `AdvancedPropertyChangedEventArgs`:

```
protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
{
}
```

### Getting mode information from the outside

Getting the information from outside the objects is somewhat more work. This is because the `PropertyChanged` event still provides a value of the `PropertyChangedEventArgs` class. Therefore, it is required to cast the value:

```
private void OnObjectPropertyChanged(PropertyChangedEventArgs e)
{
    var advancedArgs = e as AdvancedPropertyChangedEventArgs;
    if (advancedArgs != null)
    {
        // a value of AdvancedPropertyChangedEventArgs is now available
    }
}
```

## Providing old value manually

When using the dependency property a-like property registration, the old and new value are automatically provided by the classes. However, when using the `ObservableObject`, the old and new value are not automatically provided. Therefore, it is possible to provide these values manually:

```
private string _firstName;
public string FirstName
{
    get { return _firstName; }
    set 
    { 
        var oldValue = _firstName;
        _firstName = value;

        RaisePropertyChanged(() => FirstName, oldValue, value);
    }
}
```

When the values are not provided, the old and new value are set to null.

## Some sidenotes

As you might have noticed, the `AdvancedPropertyChangedEventArgs` also provide the `IsOldValueMeaningful` and the `IsNewValueMeaningful`. These are introduced because it is not always possible to determine the old or new value (for example, when the property name is string.Empty, there is no old value or new value). Therefore, the `OldValue` and `NewValue` properties are null, but does not mean that those are the actual old and new values.

It is always required to check whether the values are meaningful before actually handing them:

```
protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
{
    if (e.IsOldValueMeaningful)
    {
        // Handle old value
    }

    if (e.IsNewValueMeaningful)
    {
        // Handle new value
    }
}
```

