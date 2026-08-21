---
title: "ObservableObject" 
---
The *ObservableObject* is a very lightweight class that only implements the *INotifyPropertyChanging* and *INotifyPropertyChanged* interfaces. This class is ideal for simple objects that only need property notification. Below is an example:

```
public class Person : ObservableObject
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

