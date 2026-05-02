---
title: "Logging" 
---
Starting with version 2.2, Catel uses a custom internal logging system. This way, the reference to log4net could be removed. The idea behind this move is not to force the user to use log4net. Also, log4net seems deprecated (no new releases for a long time), and other logging systems such as NLog seem to grow in popularity.

The new logging system only allows very basic logging. This way, the logging is kept very simple and the real logging can be implemented by the end-developer if he/she feels the need for it.

## Log and ILog

All logging is available via the ILog interface. This interface is registered automatically on all objects in Catel as Log field. This way, every object can log any information by calling methods on the Log field.

In Catel, there is only one implementation of the ILog interface, which is the Log class. This class makes sure that the log messages are formatted correctly and the LogMessage event is invoked when a message is written to the log.

Catel internally creates a separate log per type. This way, there will be lots of logs and it should be easy to filter the information the end-developer is really interested in.

## LogManager

The LogManager is the class where it all comes together. This class is responsible for creating new logs for types, but also keeps track of all logs and log listeners. To retrieve the log for a specific class, use the following code:

```
private static readonly ILog Log = LogManager.GetCurrentClassLogger();
```

## Logging in code

To log in code, the ILog interface implements some basic methods to log information with an option for extra data. There are however lots of extension methods available to log exceptions, string formats and more. Below is an example of logging in code:

```
Log.Warning("Customer '{0}' does not exist", customerId); 
```

Or, if an exception is available, this can written to the log as well.

```
Log.Error(ex, "Failed to delete file '{0}'", fileName); 
```

## Logging in code with additional data

Sometimes additional data is required (for example, the thread id, or anything else like this). The logging is extensible and contains on the the bare minimum required for logging. To pass in additional information, use theÂ *[Level]WithData* methods (such asÂ *DebugWithData*):

```
Log.Debug("This is a message from a specific thread", new LogData
{
    { "ThreadId", threadId }
});
```

Then the log data will be available in theÂ *LogData* of theÂ *LogEntry*:

```
var logData = logEntry.LogData;
Â 
var threadId = logData["ThreadId"];
```

## Logging to the output window or console

By default, Catel does not add any listeners. However, it contains a ready-to-use implementation that writes all logs to the output window or console, which is the DebugLogListener. To register this listener, call this at any time:

```
#if DEBUG
LogManager.AddDebugListener();
#endif
```

## Overriding global log level flags

Start with Catel 3.8, it is possible to override the global log level flags for all listeners. To do this, set the corresponding flag on theÂ *LogManager* to a value. For example, to force debug logging on all log listeners, use the code below:

```
LogManager.IsDebugEnabled = true;
```

To reset the override, set the value back toÂ *null*:

```
LogManager.IsDebugEnabled = null;
```

