---
title: "Event Tracing for Windows (ETW)" 
---
Starting with version 4.0.0, Catel supports [Event Tracking for Windows (ETW)](http://msdn.microsoft.com/en-us/library/ms751538(v=vs.110).aspx) out of the box. To add a log listener, use the code below:

```
var logListener = new EtwLogListener();
logListener.IgnoreCatelLogging = true;
// TODO: Customize options
Â 
LogManager.AddListener(logListener);
```

