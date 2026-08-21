---
title: "Caching" 
---
Caching is about improving applications performance. The most expensive performance costs of the applications are related with data retrieving, typically when this data requires to be moved across the network or loaded from disk. But some data have a slow changing behavior (a.k.a non-volatile) and does not require to be re-read with the same frequency of the volatile data.

So, to improve your application's performance and handling this "nonvolatile" data from a clean approach, Catel comes with a CacheStorage class. Notice that the first generic parameter is the type of the key and the second the type of the value the will be stored, just like a Dictionary but CacheStorage is not it just a Dictionary. This class allows you to retrieve data and store it into the cache with single statement and also helps you to handle expiration policy if you need it.

## Initializing a cache storage

To initialize a cache storage field into your class use the following code:

```
private readonly CacheStorage<string, Person> _personCache = new CacheStorage<string, Person>(storeNullValues: true);
```

## Retrieving data and storing into cache with single statement

To retrieve data and store into a cache with a single statement use the following code:

```
var person = _personCache.GetFromCacheOrFetch(Id, () => service.FindPersonById(Id));
```

When this statement is executed with the same key more than once , the value will be retrieved from the cache storage instead of the service call. The service call will be executed just the first time or if the item is removed from the cache manually or automatically due to the expiration policy.

## Using cache expiration policies

The cache expiration policies add a removal behavior to the cache storage items. A policy signals that an item is expired to make that cache storage remove the item automatically.

A default cache expiration policy initialization code can be specified during cache storage initialization constructor:

```
CacheStorage<string, Person> _personCache = new CacheStorage<string, Person>(() => ExpirationPolicy.Duration(TimeSpan.FromMinutes(5)), true);
```

You can specify a specific expiration policy for an item when it is storing:

```
_personCache.GetFromCacheOrFetch(id, () => service.GetPersonById(id), ExpirationPolicy.Duration(TimeSpan.FromMinutes(10)));
```

The default cache policy specified at cache storage initialization will be used if the item storing the expiration policy is not specified.

## Build-in expiration policies

Catel comes with build-in expiration policies. They are listed in the follow table:

Expiration policy|Type|Description|Initialization code sample
---|---|---|---
AbsoluteExpirationPolicy|Time-base|The cache item will expire on the absolute expiration DateTime|```ExpirationPolicy.Absolute(new DateTime(21, 12, 2012))```
DurationExpirationPolicy|Time-base|The cache item will expire using the duration TimeSpan to calculate the absolute expiration from DateTime.Now|```ExpirationPolicy.Duration(TimeSpan.FromMinutes(5))```
SlidingExpirationPolicy|Time-base|The cache item will expire using the duration TimeSpan to calculate the absolute expiration from DateTime.Now, but everytime the item is requested, it is expanded again with the specified TimeSpan|```ExpirationPolicy.Sliding(TimeSpan.FromMinutes(5))```
CustomExpirationPolicy|Custom|The cache item will expire using the expire function and execute the reset action if specified. The example shows how to create a sliding expiration policy with a custom expiration policy.|```var startDateTime = DateTime.Now;var duration = TimeSpan.FromMinutes(5);ExpirationPolicy.Custom(() => DateTime.Now > startDateTime.Add(duration), () => startDateTime = DateTime.Now);```
CompositeExpirationPolicy|Custom|Combines several expiration policy into a single one. It can be configured to expire when any policy expires or when all policies expire.|```new CompositeExpirationPolicy().Add(ExpirationPolicy.Sliding(TimeSpan.FromMinutes(5))).Add(ExpirationPolicy.Custom(()=>...))```

Implementing your own expiration cache policy

If the *CustomExpirationPolicy* is not enough, you can implement you own expiration policy to make that cache item expire triggered from a custom event. You are also able to add some code to reset the expiration policy if the item is read from the cache before it expires (just like *SlidingExpirationPolicy* does).

To implement an expiration cache policy use the following code template:

```
public class MyExpirationPolicy : ExpirationPolicy
{
 public MyExpirationPolicy():base(true)
 {
 }

 public override bool IsExpired
 {
 get
 {
 // Add your custom expiration code to detect if the item expires
 }
 }

 public override void OnReset()
 {
 // Add your custom code to reset the policy if the item is read.
 }
}
```

The base constructor has a parameter to indicate if the policy can be reset. Therefore, if you call the base constructor with false then the OnReset method will never called.


