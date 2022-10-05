namespace Catel.Messaging
{
    using System;
    using IoC;
    using Logging;
    using Reflection;

    /// <summary>
    /// Helper class for the <see cref="MessageMediator"/> to allow easy subscription 
    /// </summary>
    public static class MessageMediatorHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Subscribes all methods of the specified instance that are decorated with the <see cref="MessageRecipientAttribute"/>.
        /// </summary>
        /// <param name="instance">The instance to subscribe.</param>
        /// <param name="messageMediator">The message mediator. If <c>null</c>, the default will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The object has non-public methods decorated with the <see cref="MessageRecipientAttribute"/>, but the
        /// application is not written in full .NET.</exception>
        /// <exception cref="InvalidCastException">One of the methods cannot be casted to a valid message method.</exception>
        public static void SubscribeRecipient(object instance, IMessageMediator? messageMediator = null)
        {
            ArgumentNullException.ThrowIfNull(instance);

            if (messageMediator is null)
            {
                var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
                messageMediator = dependencyResolver.ResolveRequired<IMessageMediator>();
            }

            var mediator = messageMediator;
            var methodInfos = instance.GetType().GetMethodsEx(BindingFlagsHelper.GetFinalBindingFlags(true, false));

            foreach (var methodInfo in methodInfos)
            {
                var customAttributes = methodInfo.GetCustomAttributesEx(typeof(MessageRecipientAttribute), true);

                foreach (var customAttribute in customAttributes)
                {
                    var attribute = (MessageRecipientAttribute)customAttribute;
                    var parameterInfos = methodInfo.GetParameters();

                    Type actionType;
                    Type actionParameterType;

                    switch (parameterInfos.Length)
                    {
                        case 0:
                            actionType = typeof(Action<object>);
                            actionParameterType = typeof(object);
                            break;

                        case 1:
                            actionType = typeof(Action<>).MakeGenericTypeEx(parameterInfos[0].ParameterType);
                            actionParameterType = parameterInfos[0].ParameterType;
                            break;

                        default:
                            throw Log.ErrorAndCreateException<InvalidCastException>("Cannot cast '{0}' to Action or Action<T> delegate type.", methodInfo.Name);
                    }

                    var tag = attribute.Tag;
                    var action = DelegateHelper.CreateDelegate(actionType, instance, methodInfo);

#pragma warning disable HAA0101 // Array allocation for params parameter
                    var registerMethod = mediator.GetType().GetMethodEx("Register").MakeGenericMethod(actionParameterType);
#pragma warning restore HAA0101 // Array allocation for params parameter
                    registerMethod.Invoke(mediator, new[] { instance, action, tag });
                }
            }
        }

        /// <summary>
        /// Unsubscribes all methods of the specified instance that are decorated with the <see cref="MessageRecipientAttribute"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="messageMediator">The message mediator. If <c>null</c>, the default will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        public static void UnsubscribeRecipient(object instance, IMessageMediator? messageMediator = null)
        {
            ArgumentNullException.ThrowIfNull(instance);

            if (messageMediator is null)
            {
                var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
                messageMediator = dependencyResolver.ResolveRequired<IMessageMediator>();
            }

            messageMediator.UnregisterRecipient(instance);
        }
    }
}
