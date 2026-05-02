---
title: "TypeFactory" 
description: ""
weight: 20
---
The *TypeFactory* is responsible for actually creating types inside Catel. It uses the following mechanism:

1.  List all the constructors, order them from most parameters to least parameters
2.  While (constructors available)
    Â  Â  try to construct type using injection
3.  If all constructors fail, the `TypeFactory` will fallback to `Activator.CreateInstance()`

## Dependency injection

The ServiceLocator in Catel supports dependency injection.

## Introduction to dependency injection

Some people make dependency injection hard to understand, or maybe they don't understand it themselves. Dependency injection simply means that instead of hard referencing or instantiating other classes (dependendies), the dependencies are injected into the class via the constructor.

**Example 1: bad, instantiates the dependencies itself**

```
public class MyClass
{
    private IFirstDependency _firstDependency;
    private ISecondDependency _secondDependency;

    public MyClass()
    {
        _firstDependency = new FirstDependency();
        _secondDependency = new SecondDependency();
    }
}
```

**Example 2: good, retrieves the dependencies via the service locator**)

```
public class MyClass
{
    private IFirstDependency _firstDependency;
    private ISecondDependency _secondDependency;

    public MyClass()
    {
        _firstDependency = ServiceLocator.Instance.ResolveType<IFirstDependency>();
        _secondDependency = ServiceLocator.Instance.ResolveType<ISecondDependency>();
    }
}
```
Â 

**Example 3: good, gets the dependencies injected**

```
public class MyClass
{
    private IFirstDependency _firstDependency;
    private ISecondDependency _secondDependency;

    public MyClass(IFirstDependency firstDependency, ISecondDependency secondDepdenceny)
    {
        Argument.IsNotNull("firstDependency", firstDependency);
        Argument.IsNotNull("secondDependency", secondDependency);

        _firstDependency = firstDependency;
        _secondDependency = secondDependency;
    }
}
```

{{% notice info %}}
There are other ways of using dependency injection, for example via attributes. This documentation will focus on dependency injection via the constructor only
{{% /notice %}}

## Using dependency injection in Catel

Dependency injection via the ServiceLocator in Catel is enabled by default. This means that when a type is resolved from the container, it will automatically use dependency injection to construct the type if it is not registered as instance.

It will first search for all available constructors on the type that will be instantiated. Then, for each constructor, starting with the one with the most parameters, it will try to retrieve all values. If one fails, it will go to the next. If all fail, it will try to use the default constructor without parameters. If that fails as well, then the type cannot be constructed and an exception will be thrown.

To get a better understanding of what happens, see the class below:

```
public class MyClass
{
    private IFirstDependency _firstDependency;
    private ISecondDependency _secondDependency;

    public MyClass()
        : this(null) { }

    public MyClass(IFirstDependency firstDependency)
        : this(firstDependency, null) { }

    public MyClass(IFirstDependency firstDependency, ISecondDependency secondDepdenceny)
    {
        _firstDependency = firstDependency;
        _secondDependency = secondDependency;
    }
}
```

When the MyClass will be retrieved from the ServiceLocator, this will happen:

1.  Find constructor with most parameters (the one with both firstDependency and secondDependency). If both IFirstDependency and ISecondDependency can be resolved from the ServiceLocator, the type will be constructed with the constructor. Otherwise it will proceed with step 2.
2.  Find next constructor with most parameters (the one with only firstDependency). If IFirstDependency can be resolved from the ServiceLocator, the type will be constructed with the constructor. Otherwise it will proceed with step 3.
3.  At this point, no constructor could be used. In this case, the ServiceLocator will try to use the default constructor (the one without parameters) as last resort to instantiate the type.

## Disabling dependency injection

Â Maybe you don't want dependency injection because it does not give you what you need or you want a very, very small improvement in performance. In that case, the dependency injection can be disabled using the code below:

```
ServiceLocator.Default.SupportedDependencyInjection = false
```

## Custom initialization

All types created with theÂ `TypeFactory` can be initialized with custom code. This can be done by implementing theÂ `INeedCustomInitialization`Â interface.Â As soon as a type is created, theÂ TypeFactoryÂ will check whether it implements theÂ INeedCustomInitializationÂ interface. If so, it will call theÂ Initialize method of the interface.

{{% notice warning %}}
To prevent misuse of theÂ `Initialize` method, it is best to implement the interface explicitly
{{% /notice %}}

