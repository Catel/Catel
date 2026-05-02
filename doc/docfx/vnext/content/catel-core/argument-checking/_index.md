---
title: "Argument checking" 
description: ""
---
It is best practice to always check if the input to a method is correct. If not, an exception should be thrown. Most people do not check for exceptions correctly and lots of null reference exceptions inside a deep stacktrace are hard to solve.
Catel does check the input on every method. Normally, a check would look like this:

```
public void CheckForException(object obj)
{
    if (obj == null)
    {
        throw new ArgumentNullException("obj");
    }
}
```

However, Catel extensively logs all behavior, thus all the checks started to look like this:

```
public void CheckForException(object obj)
{
    if (obj == null)
    {
        Log.Debug("Argument 'obj' is null in CheckForException");
        throw new ArgumentNullException("obj");
    }
}
```

Handling input correctly in such a case takes a lot of space and repetitive code. Therefore the Argument class is developed. This way, it is very simple to check for arguments:

```
public void CheckForException(object obj)
{
    Argument.IsNotNull("obj", obj);
}
```

Or, if a range should be checked:

```
public void CheckForException(int myInt)
{
    Argument.IsNotOutOfRange("myInt", myInt, 0, 10);
}
```

A final example is to check whether a type implements a specific interface:

```
public void CheckForException(object obj)
{
    Argument.ImplementsInterface("obj", obj, typeof(INotifyPropertyChanged));
}
```

