---
title: "Exception handling" 
---
> **Note:** `IExceptionService` has been removed in Catel 7. Use standard .NET exception handling patterns instead.

For application-wide unhandled exception handling in WPF, subscribe to the `AppDomain.CurrentDomain.UnhandledException` and `Dispatcher.UnhandledException` events directly.

