---
title: "EventLogListener" 
---
The `EventLogListener` allow to write log data to the system event log.Â 

![](../../../images/catel-core/logging/log-listeners/event-log-listener/eventlog.png)Â 

To add it, use the code below:

```
var logListener = new EventLogListener();
logListener.IgnoreCatelLogging = true;
// TODO: Customize options

LogManager.AddListener(logListener);
```

This log listener is currentlyÂ available only for the full .net framework


