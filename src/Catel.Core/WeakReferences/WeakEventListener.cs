namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;
    using Logging;
    using Reflection;
    using EventArgsBase = System.EventArgs;

    /// <summary>
    /// Open instance delegate which allows the creation of an instance method without an actual reference
    /// to the target.
    /// </summary>
    public delegate void OpenInstanceEventHandler<TTarget, TEventArgs>(TTarget @this, object sender, TEventArgs e);

    /// <summary>
    /// Open instance delegate which allows the creation of an instance method without an actual reference
    /// to the target.
    /// </summary>
    public delegate void OpenInstanceActionHandler<TTarget>(TTarget @this);

    /// <summary>
    /// Convenience implementation of the <see cref="WeakEventListener{TTarget,TSource,TEventArgs}"/> that automatically determines the type
    /// of the event source, the event target and the event args.
    /// </summary>
    /// <remarks>
    /// This class can only be used for non-static event sources and targets. If static events or listeners are required, use
    /// the <see cref="WeakEventListener{TTarget,TSource,TEventArgs}"/> class.
    /// </remarks>
    public static class WeakEventListener
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
        /// <returns>The created event listener.</returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source" /> and <paramref name="target" /> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName" /> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler" /> is not of type <see cref="PropertyChangedEventHandler" />,
        /// <see cref="NotifyCollectionChangedEventHandler" /> or <see cref="EventHandler{TEventArgs}" />.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler" /> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakGenericEvent<TEventArgs>(this object target, object source, string eventName, EventHandler<TEventArgs> handler, bool throwWhenSubscriptionFails = true)
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
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakPropertyChangedEvent(this object target, object source, PropertyChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "PropertyChanged")
        {
            return SubscribeToWeakEvent(target, source, eventName, handler, typeof(INotifyPropertyChanged), throwWhenSubscriptionFails);
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
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakCollectionChangedEvent(this object target, object source, NotifyCollectionChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "CollectionChanged")
        {
            return SubscribeToWeakEvent(target, source, eventName, handler, typeof(INotifyCollectionChanged), throwWhenSubscriptionFails);
        }

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// </summary>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
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
        /// <param name="throwWhenSubscriptionFails">if set to <c>true</c>, throw an exception when subscription fails (does not apply to argument checks).</param>
        /// <exception cref="InvalidOperationException">The <paramref name="source"/> and <paramref name="target"/> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName"/> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is not of type <see cref="PropertyChangedEventHandler"/>, <see cref="NotifyCollectionChangedEventHandler"/> or <see cref="EventHandler{TEventArgs}"/>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakEvent(this object target, object source, string eventName, Action handler, bool throwWhenSubscriptionFails = true)
        {
            return SubscribeToWeakEvent(handler.Target ?? target, source, eventName, handler, source.GetType(), throwWhenSubscriptionFails);
        }

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// </summary>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
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
        /// <param name="throwWhenSubscriptionFails">if set to <c>true</c>, throw an exception when subscription fails (does not apply to argument checks).</param>
        /// <exception cref="InvalidOperationException">The <paramref name="source"/> and <paramref name="target"/> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName"/> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is not of type <see cref="PropertyChangedEventHandler"/>, <see cref="NotifyCollectionChangedEventHandler"/> or <see cref="EventHandler{TEventArgs}"/>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener? SubscribeToWeakEvent(this object target, object source, string eventName, Delegate handler, bool throwWhenSubscriptionFails = true)
        {
            return SubscribeToWeakEvent(target, source, eventName, handler, source.GetType(), throwWhenSubscriptionFails);
        }

        /// <summary>
        /// Subscribes to a weak event by using one single method. This method also takes care of automatic
        /// unsubscription of the event.
        /// </summary>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute when the event occurs.</param>
        /// <param name="eventSourceType">The event source to use for type implementations. Differs from the source type for explicitly implemented events.</param>
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
        private static IWeakEventListener? SubscribeToWeakEvent(object target, object source, string eventName, Delegate handler, Type eventSourceType, bool throwWhenSubscriptionFails)
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            var targetType = target.GetType();
            var sourceType = source.GetType();

            MethodInfo? methodInfo = null;

            var cacheKey = $"{targetType.FullName}_{sourceType.FullName}";

            lock (ListenerTypeCache)
            {
                if (!ListenerTypeCache.TryGetValue(cacheKey, out methodInfo))
                {
                    var listenerType = typeof(WeakEventListener<object, object>).GetGenericTypeDefinition().MakeGenericType(new[] { targetType, sourceType });
                    var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(true, true);

                    methodInfo = listenerType.GetMethodEx("SubscribeToWeakEventWithExplicitSourceType", new[] { targetType, sourceType, typeof(string), typeof(Delegate), typeof(bool) }, bindingFlags);
                    ListenerTypeCache[cacheKey] = methodInfo;
                }
            }

            if (methodInfo is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Expected to find the SubscribeToWeakEventWithExplicitSourceType on WeakEventListener<TTarget, TSource>, but did not find it");
            }

#pragma warning disable HAA0101 // Array allocation for params parameter
            var genericMethodInfo = methodInfo.MakeGenericMethod(eventSourceType);
            var result = genericMethodInfo.Invoke(null, new[] { target, source, eventName, handler, throwWhenSubscriptionFails }) as IWeakEventListener;
#pragma warning restore HAA0101 // Array allocation for params parameter
            return result;
        }
    }
}
