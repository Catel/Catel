---
title: "DispatcherObservableObject" 
description: ""
weight: 20
---
{{% notice warning %}}
Note that the *DispatcherObservableObject* is located in *Catel.MVVM* because it uses the *IDispatcherService*
{{% /notice %}}

The *DispatcherObservableObject* is a class that derives from the *ObservableObject* class. The only difference is that the *DispatcherObservableObject* will dispatch all property change notifications to the UI thread. Below is a class that uses the *DispatcherObservableObject* and is thread-safe for the change notifications.

```
public class Person : DispatcherObservableObject
{
    private string _firstName;
    private string _middleName;
    private string _lastName;

    public Person(string firstName, string middleName, string lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }

    public string FirstName
    {
        get { return _firstName; }
        set
        {
            RaisePropertyChanging(() => FirstName);

            var oldValue = _firstName;
            _firstName = value;
            RaisePropertyChanged(() => FirstName, oldValue, value);
        }
    }

    public string MiddleName
    {
        get { return _middleName; }
        set
        {
            RaisePropertyChanging(() => MiddleName);
            var oldValue = _middleName;
            _middleName = value;
            RaisePropertyChanged(() => MiddleName, oldValue, value);
        }
    }

    public string LastName
    {
        get { return _lastName; }
        set
        {
            RaisePropertyChanging(() => LastName);
            var oldValue = _lastName;
            _lastName = value;
            RaisePropertyChanged(() => LastName, oldValue, value);
        }
    }
}
```

