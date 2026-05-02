---
title: "Exception handling" 
---
With exception handling in Catel, it is possible to create an exception handling policy and execute code in a safe way without have to check all the exception types manually.Â Catel exposes this technique via the IExceptionService.

/\*\*/ Setting up the IExceptionService Executing code using the IExceptionService Executing an action Executing a function Use the retry capabilities Retry Immediately Retry defined Process with retry Subscribe to the retry events Handling exceptions manually Unregistering exceptions Buffering Define the way to buffer Subscribe to the buffering events Determine if an exception type are registered for handling Get a specific exception handler

Also see

WPF implementation for exception handling

## Setting up the IExceptionService

It is important to register an exception in the service and let Catel know how it should be handled. The service handles exceptions in the way they are added to the IExceptionService.

The example below registers several exceptions and how they should be handled. When a *FileNotFoundException* occurs, it will show a message to the user. For any other exception, it will log the exception and show a message that the user should contact the developers.

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver .Resolve<IExceptionService>();

exceptionService.Register<FileNotFoundException>(exception => dependencyResolver.Resolve<IMessageService>().Show(exception.Message));
exceptionService.Register<Exception>(exception =>Â 
{
    Log.Error(exception);
    dependencyResolver.Resolve<IMessageService>().Show("An unknown exception occurred, please contact the developers"); 
});
```

The IExceptionService checks for type hierarchy. For example, when an exception as type Exception is registered, this handler will handle all exceptions

## Executing code using the IExceptionService

The Process method is responsible to keep track of all exceptions which might occur and will handle them if they are registered. If one of your registered exceptions is thrown by the code, the Process method will handle it and perform the action defined while the registration operation (for example, by showing a message box).

The *Process* method comes in two flavors: as action and as function.

### Executing an action

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
exceptionService.Process(() => { throw new ArgumentOutOfRangeException(); });
```

### Executing a function

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
var result = exceptionService.Process<int>(() => 1 + 1);
```

You can process yours actions asynchronously by using the ***ProcessAsync*** method.

## Use the retry capabilities

In some cases, you can want to have possibility to retry an action a certain number of times before finally handle your exception. Let see how the *IExceptionService* allows us to handle this kind of cases.

Firstly, you need to define how the *IExceptionService* will retry your action in case of error, two possibilities are provided for that.

### Retry Immediately

When you setting up your exceptions on IExceptionService, you have to additionnallyÂ use the OnErrorRetryImmediatelyÂ method like shown below :

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
exceptionService.Register<ArgumentNullException>(exception => { /* Do something */ })
                .OnErrorRetryImmediately();
```

This method will say to the IExceptionService to retry the action each times this one throw an exception until it succeed and without to wait before the next retry.

You can also specify the number of times you want the *IExceptionService* to retry immediately like this for example : *OnErrorRetryImmediately(5)*

### Retry defined

You have also the possibility to define more deeply the way you want your actions to be retried by using the *OnErrorRetry* method like shown below.

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
exceptionService.Register<ArgumentNullException>(exception => { /* Do something */ })
                .OnErrorRetry(5, TimeSpan.FromMinutes(2));
```

Where 5 represents the nombre of times the action will be retried andÂ *TimeSpan.FromMinutes(2)* the interval between theÂ retries.

### Process with retry

If you have provided a retry policy, you can use theÂ *ProcessWithRetry* method to expect have your policy applied on error. Below an example :

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
exceptionService.ProcessWithRetry(() =>
            {
                /* Do something */
            });
```

### Subscribe to the retry events

Can you subscribe to the events which are thown each time an action is retried like this :

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
exceptionService.RetryingAction +=
                (sender, eventArgs) =>
                    Console.WriteLine("The '{0}' have caused retrying action for the '{1}' times.",
                        eventArgs.LastException, eventArgs.CurrentRetryCount);
```

## Handling exceptions manually

It is possible to manually handle exceptions using the service. This is useful when you don't want to wrap code in the Process method, but still want to be able to create a generic exception handling policy.

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();

try
{
    var value = 150/0;
}
catch (DivideByZeroException exception)
{
    exceptionService.HandleException(exception);
}
```

If the exception can be handled, the registered action will be executed, but your code can safely continue. If the exception (in this case DivideByZeroException) is not registered, the HandleException method will rethrow the exception.

## Unregistering exceptions

Although it will probably hardly used, it is possible to unregister exceptions again using the code below:

```
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
exceptionService.Unregister<ArgumentException>();
```

## Buffering

### Define the way to buffer

You can want to throttle downÂ the number of exceptions you receive when a production process goes awry for example. You can do it through the *UsingTolerance* extension method as shown below :

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();

exceptionService.Register<DivideByZeroException>(exception => { })
                .UsingTolerance(9, TimeSpan.FromSeconds(10.0));
```

Here, the idea is to only receive the 10<sup>th</sup> exception message.

### Subscribe to the buffering events

Can you subscribe to the events which are thown each time an exception is buffered like this :

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
exceptionService.ExceptionBuffered +=
                (sender, eventArgs) =>
                    Console.WriteLine("The '{0}' is buffered for at '{1}'.",
                        eventArgs.BufferedException, eventArgs.DateTime);
```

## Determine if an exception type are registered for handling

If you want to know if an exception type have its policy registered on the *IExceptionService*, you can do this by using the *IsExceptionRegistered* method like shown belowÂ :

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
if (exceptionService.IsExceptionRegistered<ArgumentNullException>())
   {
       //Do something
   }
```

## Get a specific exception handler

If you want to retrieve the registered exception handler for an exception type, you have to use the *GetHandler* method like shown below :

```
var dependencyResolver = this.GetDependencyResolver();
var exceptionService = dependencyResolver.Resolve<IExceptionService>();
var exceptionHandler = exceptionService.GetHandler<ArgumentException>();
```

