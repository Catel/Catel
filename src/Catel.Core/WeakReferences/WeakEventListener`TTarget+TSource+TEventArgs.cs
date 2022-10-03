namespace Catel
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// Implements a weak event listener that allows the owner to be garbage
    /// collected if its only remaining link is an event handler.
    /// </summary>
    /// <typeparam name="TSource">Type of source for the event.</typeparam>
    /// <typeparam name="TTarget">Type of target listening for the event.</typeparam>
    /// <typeparam name="TEventArgs">Type of event arguments for the event.</typeparam>
    /// <example>
    /// Initially, the code must be used in this way: 
    /// <para />
    /// <code>
    ///  <![CDATA[
    ///     var source = new EventSource();
    ///     var listener = new EventListener();
    ///
    ///     WeakEventListener<EventListener, EventSource, EventArgs>.SubscribeToWeakEvent(listener, source, "Event", listener.OnEvent);
    /// ]]>
    /// </code>
    /// </example>
    public class WeakEventListener<TTarget, TSource, TEventArgs> : IWeakEventListener
        where TTarget : class
        where TSource : class
        where TEventArgs : class
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// WeakReference to the target listening for the event.
        /// </summary>
        private readonly WeakReference _weakTarget;

        /// <summary>
        /// To hold a reference to source object. With this instance the WeakEventListener 
        /// can guarantee that the handler get unregistered when listener is released.
        /// </summary>
        private readonly WeakReference _weakSource;

        /// <summary>
        /// The event name this listener is automatically subscribed to. If this value is <c>null</c>, the
        /// listener is not automatically registered to any event.
        /// </summary>
        private string _automaticallySubscribedEventName;

        /// <summary>
        /// Delegate that needs to be unsubscribed when registered automatically.
        /// </summary>
        private Delegate _internalEventDelegate;

        /// <summary>
        /// The type for event subscriptions. This can differ from TSource for explicitly implemented events.
        /// </summary>
        private readonly Type _typeForEventSubscriptions;

        /// <summary>
        /// Initializes a new instances of the WeakEventListener class.
        /// </summary>
        /// <param name="target">Instance subscribing to the event, should be <c>null</c> for static event handlers.</param>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="typeForEventSubscriptions">The type for event subscriptions.</param>
        private WeakEventListener(object target, TSource source, Type typeForEventSubscriptions)
        {
            _typeForEventSubscriptions = typeForEventSubscriptions;

            IsStaticEventHandler = (target is null);
            if (target is not null)
            {
                _weakTarget = new WeakReference(target);
            }

            IsStaticEvent = (source is null);
            if (source is not null)
            {
                _weakSource = new WeakReference(source);
            }
        }

        /// <summary>
        /// Gets or sets the method to call when the event fires.
        /// </summary>
        internal Delegate OnEventHandler { get; set; }

        /// <summary>
        /// Gets or sets the method to call when the event fires.
        /// </summary>
        internal Delegate OnEventAction { get; set; }

        /// <summary>
        /// Gets or sets the method to call when the static event fires.
        /// </summary>
        /// <value>The on static event action.</value>
        internal EventHandler<TEventArgs> OnStaticEventHandler { get; set; }

        /// <summary>
        /// Gets or sets the method to call when the static event fires.
        /// </summary>
        /// <value>The on static event action.</value>
        internal Action OnStaticEventAction { get; set; }

        /// <summary>
        /// Gets the target or <c>null</c> if there is no target.
        /// </summary>
        /// <value>The target.</value>
        public object Target { get { return (_weakTarget is not null) ? _weakTarget.Target : null; } }

        /// <summary>
        /// Gets the target weak reference.
        /// </summary>
        /// <value>The target weak reference.</value>
        public WeakReference TargetWeakReference { get { return _weakTarget; } }

        /// <summary>
        /// Gets the source or <c>null</c> if there is no source.
        /// </summary>
        /// <value>The target.</value>
        public object Source { get { return (_weakSource is not null) ? _weakSource.Target : null; } }

        /// <summary>
        /// Gets the source weak reference.
        /// </summary>
        /// <value>The source weak reference.</value>
        public WeakReference SourceWeakReference { get { return _weakSource; } }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get { return typeof(TTarget); } }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>The type of the source.</value>
        public Type SourceType { get { return typeof(TSource); } }

        /// <summary>
        /// Gets the type of the event args.
        /// </summary>
        /// <value>The type of the event args.</value>
        public Type EventArgsType { get { return typeof(TEventArgs); } }

        /// <summary>
        /// Gets a value indicating whether the event source has not yet been garbage collected.
        /// </summary>
        /// <value>
        /// <c>true</c> if the event source has not yet been garbage collected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// In case of static events, this property always returns <c>false</c>.
        /// </remarks>
        public bool IsSourceAlive { get { return (_weakSource is not null) && _weakSource.IsAlive; } }

        /// <summary>
        /// Gets a value indicating whether the event target has not yet been garbage collected.
        /// </summary>
        /// <value>
        /// <c>true</c> if the event target has not yet been garbage collected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// In case of static event handlers, this property always returns <c>false</c>.
        /// </remarks>
        public bool IsTargetAlive { get { return (_weakTarget is not null) && _weakTarget.IsAlive; } }

        /// <summary>
        /// Gets a value indicating whether this instance represents a static event.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance represents a static event; otherwise, <c>false</c>.
        /// </value>
        public bool IsStaticEvent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance represents a static event handler.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance represents a static event handler; otherwise, <c>false</c>.
        /// </value>
        public bool IsStaticEventHandler { get; private set; }

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
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener SubscribeToWeakGenericEvent(TTarget target, TSource source, string eventName, EventHandler<TEventArgs> handler, bool throwWhenSubscriptionFails = true)
        {
            return SubscribeToWeakEventWithExplicitSourceType<TSource>(target, source, eventName, handler, throwWhenSubscriptionFails);
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
        public static IWeakEventListener SubscribeToWeakPropertyChangedEvent(TTarget target, TSource source, PropertyChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "PropertyChanged")
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
        public static IWeakEventListener SubscribeToWeakCollectionChangedEvent(TTarget target, TSource source, NotifyCollectionChangedEventHandler handler, bool throwWhenSubscriptionFails = true, string eventName = "CollectionChanged")
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
        /// <exception cref="NotSupportedException">The <paramref name="handler"/> is an anonymous delegate.</exception>
        public static IWeakEventListener SubscribeToWeakEvent(TTarget target, TSource source, string eventName, Delegate handler, bool throwWhenSubscriptionFails = true)
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
        /// <returns>The created event listener.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="source" /> and <paramref name="target" /> are both <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="eventName" /> does not exist or not accessible.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="handler" /> is an anonymous delegate.</exception>
        public static IWeakEventListener SubscribeToWeakEventWithExplicitSourceType<TExplicitSourceType>(TTarget target, TSource source, string eventName, Delegate handler, bool throwWhenSubscriptionFails = true)
            where TExplicitSourceType : class
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            if ((source is null) && (target is null))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Both the source and target are null, which means that a static event handler subscribes to a static event. In such cases, there are no memory leaks, so there is no reason to use this class");
            }

            object finalTarget = target;

            var methodInfo = handler.GetMethodInfoEx();
            if ((methodInfo.Name.Contains("_AnonymousDelegate>")) || (methodInfo.DeclaringType.FullName.Contains("__DisplayClass")))
            {
                if (finalTarget.GetType() != methodInfo.DeclaringType)
                {
                    finalTarget = handler.Target ?? finalTarget;
                }
            }

            var weakListener = new WeakEventListener<TTarget, TSource, TEventArgs>(finalTarget, source, typeof(TExplicitSourceType));

            try
            {
                if (!weakListener.SubscribeToEvent(source, eventName, throwWhenSubscriptionFails))
                {
                    return null;
                }
            }
            catch (ArgumentException ex)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>(ex, "Failed to create the delegate. Probably the wrong type of EventArgs is used and does not match the EventHandler<TEventArgs>");
            }

            var isAction = methodInfo.GetParameters().Length == 0;

            if (weakListener.IsStaticEventHandler || (isAction && methodInfo.IsStatic))
            {
                if (isAction)
                {
                    var del = DelegateHelper.CreateDelegate(typeof(Action), methodInfo);
                    weakListener.OnStaticEventAction = (Action)del;
                }
                else
                {
                    var del = DelegateHelper.CreateDelegate(typeof(EventHandler<TEventArgs>), methodInfo);
                    weakListener.OnStaticEventHandler = (EventHandler<TEventArgs>)del;
                }
            }
            else
            {
                if (isAction)
                {
#pragma warning disable HAA0101 // Array allocation for params parameter
                    var delegateType = typeof(OpenInstanceActionHandler<>).MakeGenericType(finalTarget.GetType());
#pragma warning restore HAA0101 // Array allocation for params parameter
                    var del = DelegateHelper.CreateDelegate(delegateType, methodInfo);
                    weakListener.OnEventAction = del;
                }
                else
                {
#pragma warning disable HAA0101 // Array allocation for params parameter
                    var delegateType = typeof(OpenInstanceEventHandler<,>).MakeGenericType(finalTarget.GetType(), typeof(TEventArgs));
#pragma warning restore HAA0101 // Array allocation for params parameter
                    var del = DelegateHelper.CreateDelegate(delegateType, methodInfo);
                    weakListener.OnEventHandler = del;
                }
            }

            return weakListener;
        }

        /// <summary>
        /// Subscribes to the specific event. If the event occurs, the <see cref="OnEvent" /> method will be invoked.
        /// </summary>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="throwOnError">if set to <c>true</c>, throw an exception when an error occurs.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName" /> is <c>null</c> or whitespace.</exception>
        private bool SubscribeToEvent(object source, string eventName, bool throwOnError = true)
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            // Try normal event first
            if (SubscribeToEventUsingNormalImplementation(source, eventName))
            {
                _automaticallySubscribedEventName = eventName;
                return true;
            }

            var error = $"No add-method available for event '{eventName}', cannot subscribe using weak events. Make sure the event is public";
            Log.Error(error);

            if (throwOnError)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>(error);
            }

            return false;
        }

        /// <summary>
        /// Subscribes to the specific event using normal implementations. If the event occurs, the
        /// <see cref="OnEvent" /> method will be invoked.
        /// </summary>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns><c>true</c> if subscribed successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName" /> is <c>null</c> or whitespace.</exception>
        private bool SubscribeToEventUsingNormalImplementation(object source, string eventName)
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            var eventInfo = _typeForEventSubscriptions.GetEventEx(eventName, true, true);
            if (eventInfo is null)
            {
                if (source is not null)
                {
                    var sourceObjectType = source.GetType();
                    eventInfo = sourceObjectType.GetEventEx(eventName, true, true);
                }

                if (eventInfo is null)
                {
                    return false;
                }
            }

            var addMethod = eventInfo.GetAddMethod();
            if (addMethod is null)
            {
                return false;
            }

            return SubscribeToEventUsingMethodInfo(source, addMethod);
        }

        /// <summary>
        /// Subscribes to the event using method.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <returns><c>true</c> if subscribed successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo" /> is <c>null</c>.</exception>
        private bool SubscribeToEventUsingMethodInfo(object source, MethodInfo methodInfo)
        {
            //var handlerType = eventInfo.EventHandlerType;
            var handlerType = methodInfo.GetParameters()[0].ParameterType;

            _internalEventDelegate = DelegateHelper.CreateDelegate(handlerType, this, "OnEvent");

            methodInfo.Invoke(source, new object[] { _internalEventDelegate });

            return true;
        }

        /// <summary>
        /// Unsubscribes from the specific event. If the event occurs, the <see cref="OnEvent"/> method will no longer be invoked.
        /// </summary>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        private void UnsubscribeFromEvent(object source, string eventName)
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            if (_internalEventDelegate is null)
            {
                return;
            }

            try
            {
                // First, try regular events
                if (UnsubscribeFromEventUsingNormalImplementation(source, eventName))
                {
                    return;
                }

                // Second, try explicit interface implementations
                if (UnsubscribeFromEventUsingExplicitInterfaceImplementation(source, eventName))
                {
                    return;
                }

                Log.Warning("Failed to unsubscribe from event '{0}'", eventName);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to unsubscribe from event '{0}'", eventName);
            }
        }

        /// <summary>
        /// Unsubscribes from the specific event using normal implementations.
        /// </summary>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns><c>true</c> if subscribed successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName" /> is <c>null</c> or whitespace.</exception>
        private bool UnsubscribeFromEventUsingNormalImplementation(object source, string eventName)
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            var eventInfo = _typeForEventSubscriptions.GetEventEx(eventName, true, true);
            if (eventInfo is null)
            {
                if (source is not null)
                {
                    var sourceObjectType = source.GetType();
                    eventInfo = sourceObjectType.GetEventEx(eventName, true, true);
                }

                if (eventInfo is null)
                {
                    return false;
                }
            }

            var removeMethod = eventInfo.GetRemoveMethod();
            if (removeMethod is null)
            {
                return false;
            }

            return UnsubscribeFromEventUsingMethodInfo(source, removeMethod);
        }

        /// <summary>
        /// Unsubscribes from the specific event using explicit interface implementations.
        /// </summary>
        /// <param name="source">The source of the event, should be <c>null</c> for static events.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns><c>true</c> if subscribed successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">The <paramref name="eventName" /> is <c>null</c> or whitespace.</exception>
        private bool UnsubscribeFromEventUsingExplicitInterfaceImplementation(object source, string eventName)
        {
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            var expectedMethodName = string.Format(".remove_{0}", eventName);
            var methods = (from method in _typeForEventSubscriptions.GetMethodsEx(BindingFlags.NonPublic | BindingFlags.Instance)
                           where method.Name.Contains(expectedMethodName)
                           select method);

            foreach (var method in methods)
            {
                if (UnsubscribeFromEventUsingMethodInfo(source, method))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Unsubscribes from the event using method.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <returns><c>true</c> if subscribed successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo" /> is <c>null</c>.</exception>
        private bool UnsubscribeFromEventUsingMethodInfo(object source, MethodInfo methodInfo)
        {
            methodInfo.Invoke(source, new object[] { _internalEventDelegate });

            return true;
        }

        /// <summary>
        /// Handler for the subscribed event calls OnEventAction to handle it.
        /// </summary>
        /// <param name="source">Event source.</param>
        /// <param name="eventArgs">Event arguments.</param>
        // ReSharper disable UnusedMember.Global
        public void OnEvent(object source, TEventArgs eventArgs)
        // ReSharper restore UnusedMember.Global
        {
            if (!IsStaticEventHandler && (Target is null))
            {
                Detach();
                return;
            }

            var target = Target;

            var onEventHandler = OnEventHandler;
            if (onEventHandler is not null)
            {
#pragma warning disable HAA0101 // Array allocation for params parameter
                onEventHandler.DynamicInvoke(target, source, eventArgs);
#pragma warning restore HAA0101 // Array allocation for params parameter
            }

            var onEventAction = OnEventAction;
            if (onEventAction is not null)
            {
#pragma warning disable HAA0101 // Array allocation for params parameter
                onEventAction.DynamicInvoke(target);
#pragma warning restore HAA0101 // Array allocation for params parameter
            }

            var onStaticEventHandler = OnStaticEventHandler;
            if (onStaticEventHandler is not null)
            {
                onStaticEventHandler(source, eventArgs);
            }

            var onStaticEventAction = OnStaticEventAction;
            if (onStaticEventAction is not null)
            {
                onStaticEventAction();
            }
        }

        /// <summary>
        /// Detaches from the subscribed event.
        /// </summary>
        public void Detach()
        {
            if (!IsStaticEvent && (Source is null))
            {
                Log.Warning("Event on source '{0}' is not static, yet the source does no longer exists", typeof(TSource).FullName);
                return;
            }

            if (_automaticallySubscribedEventName is not null)
            {
                UnsubscribeFromEvent(Source, _automaticallySubscribedEventName);
            }
        }
    }
}
