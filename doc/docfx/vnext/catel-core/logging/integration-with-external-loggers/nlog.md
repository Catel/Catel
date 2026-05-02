---
title: "NLog" 
---
# NLog

The example below provides an ILogListener for NLog, but any external logging library can be used.

## Creating the listener

A listener can be created by creating a new class deriving from LogListenerBase.

```
public class NLogListener : LogListenerBase
{
    protected override void Debug(ILog log, string message, object extraData)
    {
        var finalLog = NLog.LogManager.GetLogger(log.TargetType.ToString());
        finalLog.Debug(message);
    }

    protected override void Info(ILog log, string message, object extraData)
    {
        var finalLog = NLog.LogManager.GetLogger(log.TargetType.ToString());
        finalLog.Info(message);
    }

    protected override void Warning(ILog log, string message, object extraData)
    {
        var finalLog = NLog.LogManager.GetLogger(log.TargetType.ToString());
        finalLog.Warn(message);
    }

    protected override void Error(ILog log, string message, object extraData)
    {
        var finalLog = NLog.LogManager.GetLogger(log.TargetType.ToString());
        finalLog.Error(message);
    }
}
```

## Registering the listener

Last but not least, it is important to register the listener:

```
LogManager.AddListener(new NLogListener());
```

