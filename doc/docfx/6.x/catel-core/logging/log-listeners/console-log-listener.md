---
title: "ConsoleLogListener" 
---
TheÂ `ConsoleLogListener` writes messages to the console with automatic colors:

![](../../../images/catel-core/logging/log-listeners/console-log-listener/console.png)

To add it, use the code below:

```
var logListener = new ConsoleLogListener();
logListener.IgnoreCatelLogging = true;
// TODO: Customize options

LogManager.AddListener(logListener);
```

