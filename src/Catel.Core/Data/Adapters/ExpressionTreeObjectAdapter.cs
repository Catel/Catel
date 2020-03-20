namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;

    /// <summary>
    /// Object adapter allowing to customize reflection and property mappings.
    /// </summary>
    public partial class ExpressionTreeObjectAdapter : IObjectAdapter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Type, IFastMemberInvoker> _fastMemberInvokerCache = new Dictionary<Type, IFastMemberInvoker>();

        /// <summary>
        /// Gets the property value of the instance.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to retrieve.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="value">The member value to update.</param>
        /// <returns><c>true</c> if the member was retrieved; otherwise <c>false</c>.</returns>
        public virtual bool GetMemberValue<TValue>(object instance, string memberName, out TValue value)
        {
            try
            {
                var modelEditor = instance as IModelEditor;
                if (modelEditor != null && modelEditor.IsPropertyRegistered(memberName))
                {
                    var modelValue = modelEditor.GetValueFastButUnsecure(memberName);
                    value = (TValue)modelValue;
                    return true;
                }

                if (instance is IPropertySerializable propertySerializable)
                {
                    object objectValue = null;
                    if (propertySerializable.GetPropertyValue(memberName, ref objectValue))
                    {
                        value = (TValue)objectValue;
                        return true;
                    }
                }

                if (instance is IFieldSerializable fieldSerializable)
                {
                    object objectValue = null;
                    if (fieldSerializable.GetFieldValue(memberName, ref objectValue))
                    {
                        value = (TValue)objectValue;
                        return true;
                    }
                }

                var modelType = instance.GetType();
                IFastMemberInvoker fastMemberInvoker = null;

                lock (_fastMemberInvokerCache)
                {
                    if (!_fastMemberInvokerCache.TryGetValue(modelType, out fastMemberInvoker))
                    {
                        fastMemberInvoker = GetFastMemberInvoker(modelType);
                        _fastMemberInvokerCache[modelType] = fastMemberInvoker;
                    }
                }

                if (fastMemberInvoker.TryGetPropertyValue(instance, memberName, out value))
                {
                    return true;
                }

                if (fastMemberInvoker.TryGetFieldValue(instance, memberName, out value))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to get value of member '{0}.{1}', skipping item during serialization", instance.GetType().GetSafeFullName(false), memberName);
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Sets the property value of the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the member was set successfully; otherwise <c>false</c>.</returns>
        public virtual bool SetMemberValue<TValue>(object instance, string memberName, TValue value)
        {
            try
            {
                var modelEditor = instance as IModelEditor;
                if (modelEditor != null && modelEditor.IsPropertyRegistered(memberName))
                {
                    // Don't use SetValueFastbutUnsecure, change notifications must be possible
                    modelEditor.SetValue(memberName, value);
                    return true;
                }

                if (instance is IPropertySerializable propertySerializable)
                {
                    if (propertySerializable.SetPropertyValue(memberName, value))
                    {
                        return true;
                    }
                }

                if (instance is IFieldSerializable fieldSerializable)
                {
                    if (fieldSerializable.SetFieldValue(memberName, value))
                    {
                        return true;
                    }
                }

                var modelType = instance.GetType();
                IFastMemberInvoker fastMemberInvoker = null;

                lock (_fastMemberInvokerCache)
                {
                    if (!_fastMemberInvokerCache.TryGetValue(modelType, out fastMemberInvoker))
                    {
                        fastMemberInvoker = GetFastMemberInvoker(modelType);
                        _fastMemberInvokerCache[modelType] = fastMemberInvoker;
                    }
                }

                if (fastMemberInvoker.SetPropertyValue(instance, memberName, value))
                {
                    return true;
                }

                if (fastMemberInvoker.SetFieldValue(instance, memberName, value))
                {
                    return true;
                }

                Log.Warning($"Failed to set member '{instance.GetType().GetSafeFullName(false)}.{memberName}' because the member cannot be found on the model");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to populate '{instance.GetType().GetSafeFullName(false)}.{memberName}', setting the member value threw an exception");
            }

            return false;
        }

        /// <summary>
        /// Gets the fast member invoker to use.
        /// </summary>
        /// <param name="modelType">The model type to get the fast member invoker for.</param>
        /// <returns>The <see cref="FastMemberInvoker{TEntity}"/> for the specified model type.</returns>
        protected virtual IFastMemberInvoker GetFastMemberInvoker(Type modelType)
        {
            var typeDefinition = typeof(FastMemberInvoker<>).MakeGenericTypeEx(modelType);
            return (IFastMemberInvoker)Activator.CreateInstance(typeDefinition);
        }
    }
}
