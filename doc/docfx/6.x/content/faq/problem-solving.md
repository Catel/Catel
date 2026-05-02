---
title: "Problem solving" 
description: ""
weight: 30
---
As with every framework or toolkit, developers run into problems. This can be bugs or abuse of the API. Catel has several ways for developers to solve the problems themselves before contacting the team with the possible issue.

## Enabling the log

We take logging very serious in Catel. This means that a lot of information about the internals can be be seen in the output window. To enable logging in Catel, use the following code in your application startup code:

```
#if DEBUG
    Catel.Logging.LogManager.AddDebugListener();
#endif
```

Now you can see all the log messages in the output window.

## Enabling stepping through the code

It's possible to step through the Catel code to see what is happening in the internals of Catel. This gives you great insights in Catel and can help you solve the problems you are encountering. See the [stepping through the code documentation](Stepping_through_the_code).

