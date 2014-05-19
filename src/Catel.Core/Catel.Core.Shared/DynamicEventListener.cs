// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicEventListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Logging;
    using Reflection;

    /// <summary>
    /// Dynamic event listener which enables the subscription to events where the type of the handler is unknown. This
    /// class uses the ILGenerator to create the dynamic methods.
    /// </summary>
    /// <remarks>
    /// Some parts in this class (with the instances and increments), but this is required to dynamically subscribe to
    /// an even that we do not know the handler of on forehand. Normally, you would do this via an anynomous delegate, 
    /// but that doesn't work so the event delegate is created via ILGenerator at runtime.
    /// <para />
    /// http://stackoverflow.com/questions/8122085/calling-an-instance-method-when-event-occurs/8122242#8122242.
    /// </remarks>
    public class DynamicEventListener
    {
        #region Classes
        /// <summary>
        /// A dictionary so the ILGenerator code can access the <c>Get</c> method.
        /// </summary>
        /// <remarks>
        /// Do NOT remove this type. It is required for dynamic reflection.
        /// </remarks>
        public class HandlerDictionary : Dictionary<int, DynamicEventListener>
        {
            /// <summary>
            /// Gets the <see cref="DynamicEventListener"/> with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns></returns>
            public DynamicEventListener Get(int key)
            {
                return this[key];
            }
        }
        #endregion

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the list of instances of the window. Required to make a call to a specific instance because
        /// Silverlight doesn't allow us to add a method to this class via ILGenerator.
        /// </summary>
        public static readonly HandlerDictionary Instances = new HandlerDictionary();

        private readonly object _eventInstance;
        private readonly Type _eventInstanceType;
        private readonly object _handlerInstance;

        private Delegate _handler;
        private EventInfo _eventInfo;

        private bool _isSubscribed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEventListener"/> class.
        /// </summary>
        /// <param name="eventInstance">The instance that contains the event.</param>
        /// <param name="eventName">Name of the event, must be a publicly accessible event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventInstance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        public DynamicEventListener(object eventInstance, string eventName)
        {
            Argument.IsNotNull("eventInstance", eventInstance);
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<DynamicEventListener>();

            _eventInstance = eventInstance;
            _eventInstanceType = eventInstance.GetType();
            EventName = eventName;

            SubscribeToEvent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEventListener"/> class.
        /// </summary>
        /// <param name="eventInstance">The instance that contains the event.</param>
        /// <param name="eventName">Name of the event, must be a publicly accessible event.</param>
        /// <param name="handlerInstance">The instance that contains the handler.</param>
        /// <param name="handlerName">Name of the handler, must be a publicly accessible method without any parameters.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventInstance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handlerInstance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="handlerName"/> is <c>null</c> or whitespace.</exception>
        public DynamicEventListener(object eventInstance, string eventName, object handlerInstance, string handlerName)
            : this(eventInstance, eventName)
        {
            Argument.IsNotNull("handlerInstance", handlerInstance);
            Argument.IsNotNullOrWhitespace("handlerName", handlerName);
   
            _handlerInstance = handlerInstance;
            HandlerName = handlerName;
        }

        #region Properties
        /// <summary>
        /// Gets the name of the event. This must be a publicly accessible event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; private set; }

        /// <summary>
        /// Gets the name of the handler. This must be a publicly accessible method without any parameters.
        /// </summary>
        /// <value>The name of the handler.</value>
        public string HandlerName { get; private set; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public int UniqueIdentifier { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the dynamic event has occurred.
        /// </summary>
        public event EventHandler EventOccurred;
        #endregion

        #region Methods
        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        private void SubscribeToEvent()
        {
            if (_isSubscribed)
            {
                return;
            }

            lock (Instances)
            {
                Instances.Add(UniqueIdentifier, this);
            }

            _eventInfo = _eventInstanceType.GetEventEx(EventName, BindingFlagsHelper.GetFinalBindingFlags(true, false));
            if (_eventInfo == null)
            {
                string error = string.Format("Cannot find the '{0}' event, implement the '{0}' event on '{1}'", EventName, _eventInstanceType.Name);
                Log.Error(error);

                throw new NotSupportedException(error);
            }

            _handler = CreateDynamicHandlerDelegate(_eventInfo.EventHandlerType);
            _eventInfo.AddEventHandler(_eventInstance, _handler);
        }

        /// <summary>
        /// Unsubscribes to event.
        /// </summary>
        public void UnsubscribeFromEvent()
        {
            if (_handler != null)
            {
                _eventInfo.RemoveEventHandler(_eventInstance, _handler);
            }

            _eventInfo = null;
            _handler = null;

            lock (Instances)
            {
                Instances.Remove(UniqueIdentifier);
            }

            _isSubscribed = false;
        }

        /// <summary>
        /// Creates the dynamic closed delegate.
        /// </summary>
        /// <param name="eventHandlerType">Type of the event handler.</param>
        /// <returns>A dynamically created closed delegate.</returns>
        private Delegate CreateDynamicHandlerDelegate(Type eventHandlerType)
        {
            var handler = new DynamicMethod("DynamicEventHandler", null, GetDelegateParameterTypes(eventHandlerType));

            ILGenerator ilgen = handler.GetILGenerator();

            var handlerMethodInfo = GetType().GetMethodEx("OnEvent", BindingFlagsHelper.GetFinalBindingFlags(false, false));

            // this.OnTargetWindowClosed
            ilgen.Emit(OpCodes.Ldsfld, typeof(DynamicEventListener).GetFieldEx("Instances", BindingFlagsHelper.GetFinalBindingFlags(false, true)));
            ilgen.Emit(OpCodes.Ldc_I4, UniqueIdentifier);
            ilgen.Emit(OpCodes.Call, typeof(HandlerDictionary).GetMethodEx("Get", BindingFlagsHelper.GetFinalBindingFlags(true, false)));
            ilgen.Emit(OpCodes.Call, handlerMethodInfo);
            ilgen.Emit(OpCodes.Ret);

            return handler.CreateDelegate(eventHandlerType);
        }

        /// <summary>
        /// Gets the delegate parameter types.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <returns></returns>
        private static Type[] GetDelegateParameterTypes(Type delegateType)
        {
            if (!delegateType.HasBaseTypeEx(typeof(MulticastDelegate)))
            {
                throw new InvalidOperationException("Not a delegate");
            }

            var invoke = delegateType.GetMethodEx("Invoke");
            if (invoke == null)
            {
                throw new InvalidOperationException("Not a delegate");
            }

            var parameters = invoke.GetParameters();
            var typeParameters = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                typeParameters[i] = parameters[i].ParameterType;
            }

            return typeParameters;
        }

        /// <summary>
        /// Called when the event occurs.
        /// </summary>
// ReSharper disable UnusedMember.Global
        public void OnEvent()
// ReSharper restore UnusedMember.Global
        {
            EventOccurred.SafeInvoke(this, EventArgs.Empty);

            if (_handlerInstance != null)
            {
                var handlerMethodInfo = _handlerInstance.GetType().GetMethodEx(HandlerName, BindingFlagsHelper.GetFinalBindingFlags(true, false));
                if (handlerMethodInfo == null)
                {
                    string error = string.Format("Cannot find the '{0}' method, implement the '{0}' method on '{1}'", EventName, _handlerInstance.GetType().Name);
                    Log.Error(error);

                    throw new NotSupportedException(error);
                }

                handlerMethodInfo.Invoke(_handlerInstance, null);
            }
        }
        #endregion
    }
}

#endif