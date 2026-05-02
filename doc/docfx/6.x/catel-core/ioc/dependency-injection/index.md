---
title: "Dependency injection" 
---
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

**Example 2: good, retrieves the dependencies via the service locator**

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

There are other ways of using dependency injection, for example via attributes. This documentation will focus on dependency injection via the constructor only

## Using dependency injection in Catel

### Constructor injection

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

    public MyClass(IFirstDependency firstDependency, ISecondDependency secondDependency)
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

#### Manually defining the constructor to use for dependency injection

Catel first sorts the constructors based on the number of parameters. Then it will "sub-sort" the same number of parameters and puts parameters with *Object* and *DynamicObject* as last so it will first try all constructors with the best possible matches

If Catel is still unable to pick the right constructor in a class, this behavior can be overridden by decorating the constructor with theÂ DependencyInjectionConstructor attribute:

```
public MyClass(IPerson person)
{
Â 
}
Â 
[InjectionConstructor]
public MyClass(Person person)
{
    // Catel will now first try to use this constructor, no matter the order or number of parameters
}
```

#### Advanced dependency injection

Starting with Catel 3.7, a new type of dependency injection is supported. It allows a developer to instantiate types that combine custom constructor injection with dependency injection. The class belows shows an interesting combination of custom values that need to be injected and dependencies that can be retrieved from the IoC container. Before Catel 3.7, one had to manually retrieve the dependencies from the IoC container when it also required other dependencies to be injected that were not registered in the IoC container:

```
public class PersonViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IProcessService _processService;
Â 
    public PersonViewModel(IPerson person)
    {
        Argument.IsNotNull(() => person);

        Person = person;

        _messageService = ServiceLocator.Default.ResolveType<IMessageService>();
        _processService = ServiceLocator.Default.ResolveType<IProcessService>();
    }

    ...
}
```

With the new technology, such a constructor can be rewritten to truly support dependency injection:

```
public class PersonViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IProcessService _processService;
Â 
    public PersonViewModel(IPerson person, IMessageService messageService, IProcessService processService)
    {
        Argument.IsNotNull(() => person);
        Argument.IsNotNull(() => messageService);
        Argument.IsNotNull(() => processService);

        Person = person;

        _messageService = messageService;
        _processService = processService
    }

    ...
}
```

This feature is initially written to support dependency injection in combination with [nested user controls](Introduction_to_the_nested_user_controls_problem)

The advanced dependency injection can be used by using the TypeFactory class. Below is an example on how to create a new type using advanced dependency injection:

```
var personViewModel = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<PersonViewModel>(new Person());
```

As you can see it is only required to pass in the objects that are not registered in the IoC container. All other dependencies will be automatically resolved from theÂ *ServiceLocator*.

Note that the order of the parameters must be the same as the constructor, otherwise the *TypeFactory* cannot determine the right constructor to use

### Property injection

Starting with Catel 3.8, it is also possible to use property injection. The difference with constructor injection is that the TypeFactory will automatically set up all properties that required dependency injection.

Note that the Catel team recommends using constructor injection over property injection. Property injection looks like a silver bullet, but is very tricky because:

1) It does not allow you to check for *null* values and store dependencies in private fields (when)?

2) Dependency Injection is just a technique. When using a constructor, you can force a user to provide the value and check the input. With property injection, you can only *hope* that the user will set them for you, there is no way to check this (unless that is some *after constructor and dependency injection* initialization routine. This is never the case if a user manually creates a type though.

To use property injection, simply decorate the properties of a class with theÂ Inject attribute. Below are several options:

#### Type is automatically determined based on property type

```
public class MyClass
{
    [Inject]
    public IMyDependency MyDependency { get; set; }
}
```

#### Type is manually defined

```
public class MyClass
{
    [Inject(typeof(IMySubclassedDependency))]
    public IMyDependency MyDependency { get; set; }
}
```

#### Using tags

It is also possible to determine the tag of a registered dependency:

```
public class MyClass
{
    [Inject(Tag = "myTag")]
    public IMyDependency MyDependency { get; set; }
}
```

## Disabling dependency injection

Â Maybe you don't want dependency injection because it does not give you what you need or you want a very, very small improvement in performance. In that case, the dependency injection can be disabled using the code below:

```
ServiceLocator.Default.SupportedDependencyInjection = false
```

