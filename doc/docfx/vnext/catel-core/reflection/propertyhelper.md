---
title: "Property helper" 
---
The `PropertyHelper` class is a reflection-based helper class to get or set properties on classes. This class is used by Catel as a fall-back mechanism to use reflection in case cached expression trees are not supported.

For better performance, use the `FastMemberInvoker` instead

## Setting or getting properties of objects

In many cases, you need to possibility to set or get properties of an object via reflection. This behavior is implemented in the `PropertyHelper` class. Below are a few examples.

### Check if a property is available on an object

```
PropertyHelper.IsPropertyAvailable(person, "FirstName");
```

### Getting a property value

```
PropertyHelper.GetValue(person, "FirstName");
```

or

```
string firstName;
PropertyHelper.TryGetValue(person, "FirstName", out firstName);
```

### Setting a property value

```
PropertyHelper.SetValue(person, "FirstName", "Geert");
```

or

```
PropertyHelper.TrySetValue(person, "FirstName", "Geert");
```

