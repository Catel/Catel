namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;
    using Catel.Caching;
    using Catel.Logging;
    using Catel.Reflection;
    using EventArgsBase = System.EventArgs;

    /// <summary>
    /// Convenience implementation of the <see cref="WeakEventListener{TTarget,TSource,TEventArgs}"/> that automatically determines the type
    /// of the event args.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static class WeakEventListener<TTarget, TSource>
        where TTarget : class
        where TSource : class
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The event handler event arguments cache.
        /// </summary>
        private static readonly ICacheStorage<Type, Type> EventHandlerEventArgsCache = new CacheStorage<Type, Type>();

        private static readonly Dictionary<string, MethodInfo?> ListenerTypeCache = new Dictionary<string, MethodInfo?>();

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
        /// <param name="throwWhenSubscriptionFails">if set to <c>true</c>, throw an exception when subscription fails (does not apply to argument checks).</param>
        /// <returns>
        /// The created event listener.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source" /> and <paramref name="target" /> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source" /> and <paramref name="target" /> are both <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler" /> is not of type <see cref="PropertyChangedEventHandler" />,
        /// <see cref="NotifyCollectionChangedEventHandler" /> or <see cref="EventHandler{TEventArgs}" />.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler" /> is not of type <see cref="PropertyChangedEventHandler" />,
        /// <see cref="NotifyCollectionChangedEventHandler" /> or <see cref="EventHandler{TEventArgs}" />.</exception>
        public static IWeakEventListener? SubscribeToWeakGenericEvent<TEventArgs>(TTarget target, TSource source, string eventName, EventHandler<TEventArgs> handler, bool throwWhenSubscriptionFails = true)
            where TEventArgs : EventArgsBase
        {
            return SubscribeToWeakEvent(target, source, eventName, handler, throwWhenSubscriptionFails);
        }

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// <para />
        /// This method subscribes to the <see cref="PropertyChangedEventHandler"/> which does not follow the <c>EventHandler{TEventArgs}</c> convention.
        /// </summary>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
        /// <param name="throwWhenSubscriptionFails">if set to <c>true</c>, throw an exception when subscription fails (does not apply to argument checks).</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>
        /// The created event listener.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source"/> and <paramref name="target"/> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName"/> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakPropertyChangedEvent(TTarget target, TSource source, PropertyChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "PropertyChanged")
        {
            return SubscribeToWeakEventWithExplicitSourceType<INotifyPropertyChanged>(target, source, eventName, handler, throwWhenSubscriptionFails);
        }

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// <para />
        /// This method subscribes to the <see cref="NotifyCollectionChangedEventHandler"/> which does not follow the <c>EventHandler{TEventArgs}</c> convention.
        /// </summary>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
        /// <param name="throwWhenSubscriptionFails">if set to <c>true</c>, throw an exception when subscription fails (does not apply to argument checks).</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>
        /// The created event listener.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source"/> and <paramref name="target"/> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName"/> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakCollectionChangedEvent(TTarget target, TSource source, NotifyCollectionChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "CollectionChanged")
        {
            return SubscribeToWeakEventWithExplicitSourceType<INotifyCollectionChanged>(target, source, eventName, handler, throwWhenSubscriptionFails);
        }

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// </summary>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
        /// <param name="throwWhenSubscriptionFails">if set to <c>true</c>, throw an exception when subscription fails (does not apply to argument checks).</param>
        /// <returns>
        /// The created event listener.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source"/> and <paramref name="target"/> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName"/> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is not of type <see cref="PropertyChangedEventHandler"/>, 
        /// <see cref="NotifyCollectionChangedEventHandler"/> or <see cref="EventHandler{TEventArgs}"/>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakEvent(TTarget target, TSource source, string eventName, Delegate handler, bool throwWhenSubscriptionFails = true)
        {
            return SubscribeToWeakEventWithExplicitSourceType<TSource>(target, source, eventName, handler, throwWhenSubscriptionFails);
        }

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// </summary>
        /// <typeparam name="TExplicitSourceType">The final source type, which must be specified for explicitly implemented events.</typeparam>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
        /// <param name="throwWhenSubscriptionFails">if set to <c>true</c>, throw an exception when subscription fails (does not apply to argument checks).</param>
        /// <returns>
        /// The created event listener.
        /// </returns>
        /// <remarks>
        /// This method can only be used for non-static event sources and targets. If static events or listeners are required, use
        /// the <see cref="WeakEventListener{TTarget,TSource,TEventArgs}"/> class.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source"/> and <paramref name="target"/> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName"/> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is not of type <see cref="PropertyChangedEventHandler"/>, <see cref="NotifyCollectionChangedEventHandler"/> or <see cref="EventHandler{TEventArgs}"/>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakEventWithExplicitSourceType<TExplicitSourceType>(TTarget target, TSource source, string eventName, Delegate handler, bool throwWhenSubscriptionFails = true)
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            var handlerType = handler.GetType();
            var eventArgsType = EventHandlerEventArgsCache.GetFromCacheOrFetch(handlerType, () =>
            {
                Type? type = null;

                if (handlerType == typeof(PropertyChangedEventHandler))
                {
                    type = typeof(PropertyChangedEventArgs);
                }
                else if (handlerType == typeof(NotifyCollectionChangedEventHandler))
                {
                    type = typeof(NotifyCollectionChangedEventArgs);
                }
                else if (handlerType.IsGenericTypeEx())
                {
                    type = handlerType.GetGenericArgumentsEx()[0];
                    if (!typeof(EventArgsBase).IsAssignableFromEx(type))
                    {
                        type = null;
                    }
                }
                else if (handlerType == typeof(Action))
                {
                    // We must have the right event args, so get the right ones using reflection (cached anyway)
                    var eventHandlerType = typeof(TSource).GetEventEx(eventName)?.EventHandlerType;
                    if (eventHandlerType is not null)
                    {
                        if (eventHandlerType.ContainsGenericParametersEx())
                        {
                            type = eventHandlerType.GetGenericArgumentsEx()[0];
                        }
                        else
                        {
                            var invokeMethod = eventHandlerType.GetMethodEx("Invoke");
                            if (invokeMethod is not null)
                            {
                                type = invokeMethod.GetParameters()[1].ParameterType;
                            }
                        }
                    }
                    else
                    {
                        type = typeof(EventArgsBase);
                    }
                }

                return type ?? typeof(EventArgsBase);
            });

            if (eventArgsType is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Only handlers of type 'Action', 'PropertyChangedEventHandler', 'NotifyCollectionChangedEventHandler' or 'EventHandler<TEventArgs>' are supported. '{0}' does not belong to these supported types", handlerType.Name);
            }

            var targetType = typeof(TTarget);
            var sourceType = typeof(TSource);

            MethodInfo? methodInfo = null;

            var cacheKey = $"{targetType.FullName}_{sourceType.FullName}_{eventArgsType.FullName}";

            lock (ListenerTypeCache)
            {
                if (!ListenerTypeCache.TryGetValue(cacheKey, out methodInfo))
                {
                    var listenerType = typeof(WeakEventListener<TTarget, TSource, EventArgsBase>).GetGenericTypeDefinition().MakeGenericType(new[] { targetType, sourceType, eventArgsType });
                    var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(true, true);

                    methodInfo = listenerType.GetMethodEx("SubscribeToWeakEventWithExplicitSourceType", new[] { targetType, sourceType, typeof(string), typeof(Delegate), typeof(bool) }, bindingFlags);
                    ListenerTypeCache[cacheKey] = methodInfo;
                }
            }

            if (methodInfo is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Expected to find the SubscribeToWeakEventWithExplicitSourceType on WeakEventListener<TTarget, TSource, TEventArgs>, but did not find it");
            }

#pragma warning disable HAA0101 // Array allocation for params parameter
            var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(TExplicitSourceType));
#pragma warning restore HAA0101 // Array allocation for params parameter
            var weakEventListener = (IWeakEventListener?)genericMethodInfo.Invoke(null, new object[] { target, source, eventName, handler, throwWhenSubscriptionFails });
            if (weakEventListener is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Failed to create weak event listener subscription");
            }

            return weakEventListener;
        }
    }
}
