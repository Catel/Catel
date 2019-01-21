// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingPartyExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN

namespace Catel.MVVM
{
    using System;
    using System.Reflection;
    using Logging;
    using Reflection;

    /// <summary>
    /// Extension methods for binding parties.
    /// </summary>
    public static class BindingPartyExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly MethodInfo AddEventMethodInfo;

        static BindingPartyExtensions()
        {
            AddEventMethodInfo = typeof(BindingParty).GetMethodEx("AddEvent");
            if (AddEventMethodInfo is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Cannot find BindingParty.AddEvent method, BindingPartyExtensions will not be supported");
            }
        }

        /// <summary>
        /// Adds the event by automatically retrieving the event args type.
        /// </summary>
        /// <param name="bindingParty">The binding party.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="bindingParty"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="bindingParty"/> is <c>null</c>.</exception>
        public static void AddEvent(this BindingParty bindingParty, string eventName)
        {
            Argument.IsNotNull("bindingParty", bindingParty);
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            var instance = bindingParty.Instance;
            if (instance is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("The BindingParty '{0}' is no longer alive, cannot add event '{1}'", bindingParty, eventName);
            }

            var instanceType = instance.GetType();
            var eventInfo = instanceType.GetEventEx(eventName);
            if (eventInfo is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Event '{0}.{1}' does not exist", instanceType.Name, eventName);
            }

            var eventHandlerType = eventInfo.EventHandlerType;
            var eventArgsType = eventHandlerType.GetGenericArgumentsEx()[0];

            var genericAddEventMethod = AddEventMethodInfo.MakeGenericMethod(eventArgsType);
            genericAddEventMethod.Invoke(bindingParty, new object[] {eventName});
        }
    }
}

#endif
