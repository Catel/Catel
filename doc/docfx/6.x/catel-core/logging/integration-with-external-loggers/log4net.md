---
title: "Log4net" 
---
The example below provides an ILogListener for Log4net, but any external logging library can be used.

## Creating the listener

A listener can be created by creating a new class deriving from LogListenerBase.

```
public class Log4netListener : LogListenerBase
{
    protected override void Debug(ILog log, string message, object extraData)
    {
        var finalLog = log4net.LogManager.GetLogger(log.TargetType);
        finalLog.Debug(message);
    }

    protected override void Info(ILog log, string message, object extraData)
    {
        var finalLog = log4net.LogManager.GetLogger(log.TargetType);
        finalLog.Info(message);
    }

    protected override void Warning(ILog log, string message, object extraData)
    {
        var finalLog = log4net.LogManager.GetLogger(log.TargetType);
        finalLog.Warn(message);
    }

    protected override void Error(ILog log, string message, object extraData)
    {
        var finalLog = log4net.LogManager.GetLogger(log.TargetType);
        finalLog.Error(message);
    }
}
```

## Registering the listener

Last but not least, it is important to register the listener:

```
LogManager.AddListener(new Log4netListener());
```

## Configuring log4net

Note that this is just a sample configuration. Please use the log4net documentation for all options

1.  Add reference to log4net
2.  Add [assembly: log4net.Config.XmlConfigurator(Watch = true)] to AssemblyInfo.cs
3.  Configure log4net in your app.config to configure the actual data

