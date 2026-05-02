---
title: "Customizing listeners" 
description: ""
weight: 20
---
Each listener can be customized to only receive the logs that the listener is interested in. This way, the listener does not receive events it is not interested in. For example, to only receive errors, create a new listener and use the following customization:

```
var listener = new MyLogListener();
listener.IsDebugEnabled = false;
listener.IsInfoEnabled = false;
listener.IsWarningEnabled = false;
listener.IsErrorEnabled = true;
```

By default, all types of logging are enabled on a log listener.

