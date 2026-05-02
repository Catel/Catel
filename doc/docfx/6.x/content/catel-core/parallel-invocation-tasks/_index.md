---
title: "Parallel invocation and tasks" 
description: ""
---
This page contains helper classes in Catel to invoke specific actions on very large collections in parallel.

## Running batches on large sets in parallel

When handling a lot of items and invoking a method per item, it might be a viable option to execute the actions in batches. This normally requires quite some code to split up the large collection into batches and execute the method for each item. To make this process much easier, Catel introduces theÂ *ParallelHelper* class.

To invoke anÂ *Initialize* method on all types currently loaded by Catel, in batches of 2500 types per batch, use the following code:

```
var allTypes = new List<Type>(TypeCache.GetTypes());

ParallelHelper.ExecuteInParallel(allTypes, type => 
{ 
    SomeInitializeTypeMethod(type);
}, 2500, "Initialize types");
```

It is really easy to tweak the number of items per batch to find the optimal performance of items per batch.

