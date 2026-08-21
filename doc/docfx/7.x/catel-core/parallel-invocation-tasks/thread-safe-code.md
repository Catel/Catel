---
title: "Thread safe code" 
---
Writing a multiple threading application is always a challenge. Eventually you need use objects or statements with the ability to synchronize access to the critical sections of the code by taking and releasing a lock.

## Background information

The common solution to access the thread-sensitive resources is use the lock statement just as follow:

```
private readonly object _syncObj = new object();

public void DoTheWork()
{
    lock (_syncObj)
    {
        // Access to the thread-sensitive resources here.
    }
}
```



But some times the scenario is not simple, then you need to use the Monitor class in order to synchronize cross method operations. Here is an example: 

```
private readonly object _syncObj = new object();

public void DoTheWork()
{
    StartTheWork();
    object result = EndTheWork(); 
}

private void StartTheWork()
{
    Monitor.Enter(_syncObj);

    try
    {
        // Access to the thread-sensitive resources here.
    }
    catch(Exception)
    {
        Monitor.Exit(_syncObj);
        throw;
    }
}

private object EndTheWork()
{
    try
    {
        // Access to the thread-sensitive resources here.
        return new object();
    }
    finally
    {
        Monitor.Exit(_syncObj);
    } 
}
```

To combine the power of the simplicity of the lock statement syntax and the flexibility of the Monitor class, Catel introduces the SynchronizationContext class, allowing you to write the code like this.

```
private readonly List<IValidator> _validators = new List<IValidator>(); 
private readonly SynchronizationContext _synchronizationContext = new SynchronizationContext();

public bool Contains(IValidator validator)
{
    Argument.IsNotNull("validator", validator);

    return _synchronizationContext.Execute(() => _validators.Contains(validator));
}

public void Remove(IValidator validator)
{
    Argument.IsNotNull("validator", validator);

    _synchronizationContext.Execute(() => _validators.Remove(validator));
}

public void BeforeValidation(object instance, List<IFieldValidationResult> previousFieldValidationResults, List<IBusinessRuleValidationResult> previousBusinessRuleValidationResults)
{
    _synchronizationContext.Acquire();

    try
    {
        foreach (IValidator validator in _validators)
        {
            validator.BeforeValidation(instance, previousFieldValidationResults, previousBusinessRuleValidationResults);
        }
    }
    catch (Exception)
    {
        _synchronizationContext.Release(); 
        throw;
    }
}

public void AfterValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> validationResults)
{
    try
    {
        foreach (IValidator validator in _validators)
        {
            validator.AfterValidateBusinessRules(instance, validationResults);
        }
    }
    catch (Exception)
    {
        _synchronizationContext.Release();
        throw;
    }
}
```

SynchronizationContext also allow you create asynchronous locking request.

## Acquiring a lock

To acquire a lock, only a call to Acquire is required:

```
_synchronizationContext.Acquire();
```

Releasing a lock

To release a lock, only a call to Release is required:

```
_synchronizationContext.Release();
```

## Automatic locking of a method

It is also possible to automatically lock and release a method call. This can be accomplished using the Execute method.

```
_synchronizationContext.Execute(() => ThreadSafeCodeExecution());
```

