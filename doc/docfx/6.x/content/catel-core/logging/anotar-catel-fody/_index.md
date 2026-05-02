---
title: "Anotar.Catel.Fody" 
description: ""
weight: 50
---
Logging is a very important part of an application. It provides detailed information about the application workflow, even when an application is already deployed to several clients. Thatâ€™s the reason that logging is a first class citizen in the Catel framework.

In general, logging works by defining an *ILog* instance on a class:

```
private static readonly ILog Log = LogManager.GetCurrentClassLogger();
```

Then in any method, logging can be added like this:

```
Log.Info(â€œThis is a logging with a format â€˜{0}â€™â€, â€œtestâ€);
```

Writing the *Log* definition can be boring and repetitive. Luckily [Simon Cropp](http://twitter.com/simoncropp) came up with a solution for that, namely [Anotar.Catel.Fody](http://www.nuget.org/packages/Anotar.Catel.Fody/). With the Anotar implementation, a reference will be added to the solution. Then after compilation the assembly will be removed and all calls to the *LogTo* class will be replaced by actual calls to the CatelÂ logging classes.

## How to use Anotar

Using Anotar is really easy, just call the static methods on the *LogTo* class as you can see below:

```
LogTo.Info("This is a logging with a format â€˜{0}â€™", â€œtestâ€);
```

Note that it is no longer required to define the *Log* field, it will be added automatically by Anotar.

Besides that it is really easy to use, another benefit is a very small performance improvement. The *GetCurrentClassLogger* uses reflection to determine the current class. This is a very slight hit on performance the first time a class is used (only once, the field is static). Anotar directly replaces the call by an implementation like this:

```
private static readonly ILog Log = LogManager.GetLogger(typeof(MyClass));
```

## Additional options

### Disabling method names and line numbers

By default Anotar also logs the method and line number:

```
03:58:11:858 => [DEBUG] [AnotarDemo.Program] Method: 'Void Main(String[])'. Line: ~19. this is a test
```

If you don't want such output, add this attribute on assembly level:

```
[assembly: LogMinimalMessage]
```

Then the output will look like this:

```
03:59:36:344 => [DEBUG] [AnotarDemo1.Program] this is a test
```

### Logging exceptions automatically

It is possible to automatically log exceptions inside a method. To accomplish this, decorate the method with theÂ *LogTo[LogLevel]OnException* attribute:

```
[LogToDebugOnException]
public static void ExceptionalMethod()
{
    throw new Exception("This will be logged automatically");
}
```

Then the output will be as follows:

```
04:01:48:331 => [DEBUG] [AnotarDemo.Program] Exception occurred in 'Void ExceptionalMethod()'.  | [Exception] System.Exception: This will be logged automatically
   at AnotarDemo.Program.ExceptionalMethod() in c:\Source\AnotarDemo\AnotarDemo\Program.cs:line 27
```

