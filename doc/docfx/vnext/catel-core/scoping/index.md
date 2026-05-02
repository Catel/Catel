---
title: "Scoping" 
---
Sometimes scoping is important to share an object inside a specific scope which cannot be determined upfront. A great example is the serialization inside Catel which requires a serialization scope which can be shared over a lot of objects. Scoping in Catel is really easy. To create a scope of an object with a specific tag, use the code below:

```
using (var scopeManager = ScopeManager<object>.GetScopeManager("object"))
{
    var scopeObject = scopeManager.ScopeObject;

    // scope can be used here
}
```

When the scope does not yet exist, it will be created and the object will be created by the `TypeFactory`.

