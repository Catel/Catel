---
title: "Change notification wrapper" 
---
Subscribing to change notifications of objects mostly results in large statements such as the one below:

```
var itemAsPropertyChanged = obj as INotifyPropertyChanged;
if (itemAsPropertyChanged != null)
{
    itemAsPropertyChanged.PropertyChanged += OnPropertyChanged;
}
```

However, using this code one must be aware that if not unsubscribed, there might be a potential memory leak here. In Catel, there is a solution for such cases that can raise change notifications using weak events called the ChangeNotificationWrapper. It allows the subscription of both the INotifyPropertyChanged and INotifyCollectionChanged interfaces.

## Subscribing to events of an observable object

Using the code below, one can subscribe to the PropertyChanged event of an object:

```
var wrapper = new ChangeNotificationWrapper(obj);
wrapper.PropertyChanged += OnPropertyChanged;
```

Note that it is not required to check whether the object implements INotifyPropertyChanged, the wrapper does it automatically

## Subscribing to events of an observable collection

Using the code below, one can subscribe to the CollectionChanged event of an object:

```
var wrapper = new ChangeNotificationWrapper(observableCollection);
wrapper.CollectionChanged += OnCollectionChanged;
```

Note that it is not required to check whether the object implements INotifyCollectionChanged, the wrapper does it automatically

## Advanced scenario with observable collections

Sometimes it is required to watch both changes inside a collection, but also the items inside a collection. For example, there is a list of customers and you are also interested in changes of customers inside a collection. This is fully supported by the ChangeNotificationWrapper using the code below:

```
var wrapper = new ChangeNotificationWrapper(observableCustomerCollection);
wrapper.CollectionChanged += OnCollectionChanged;
wrapper.CollectionItemPropertyChanged += OnCollectionItemPropertyChanged;
```

All subscriptions are automatically managed by the ChangeNotificationWrapper when items are added or removed from the collection.

## Unsubscribing from events

When you are no longer interested in events from the source object, there are two options:

1. Just leave them coming, as soon as the objects are no longer used, they will be garbage collected
2. Unsubscribe using the following code:

```
wrapper.UnsubscribeFromAllEvents();
```

