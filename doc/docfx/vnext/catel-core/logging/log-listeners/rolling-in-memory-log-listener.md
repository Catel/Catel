---
title: "RollingInMemoryLogListener" 
---
In memory logging can be very useful to be able to query the latest log messages. Catel provides this via the *RollingInMemoryLogListener* and *RollingInMemoryLogService*. TheÂ *RollingInMemoryLogServiceÂ *is a wrapper around theÂ *RollingInMemoryLogListener*Â to provide a simple way to use the listener.

## Enabling the feature

By default, the feature is disabled to not eat any CPU ticks when not being used. To use the feature, the only thing required is to resolve the type from the *ServiceLocator*:

```
var rollingInMemoryLogService = ServiceLocator.Default.ResolveType<IRollingInMemoryLogService>();
```

## Querying log messages

To query the latest log messages, use one of the following methods:

-   GetLogEntries
-   GetWarningLogEntries
-   GetErrorLogEntries

## Customizing number of items to keep in memory

To customize the number of items being kept in memory, use one of the following properties:

-   MaximumNumberOfLogEntries
-   MaximumNumberOfWarningLogEntries
-   MaximumNumberOfErrorLogEntries

