﻿namespace Catel.Data
{
    using System;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// Object adapter allowing to customize reflection and property mappings.
    /// </summary>
    public class ReflectionObjectAdapter : IObjectAdapter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the property value of the instance.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to retrieve.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="value">The member value to update.</param>
        /// <returns><c>true</c> if the member was retrieved; otherwise <c>false</c>.</returns>
        public virtual bool TryGetMemberValue<TValue>(object instance, string memberName, out TValue? value)
        {
            try
            {
                var modelEditor = instance as IModelEditor;
                if (modelEditor is not null && modelEditor.IsPropertyRegistered(memberName))
                {
                    value = modelEditor.GetValueFastButUnsecure<TValue>(memberName);
                    return true;
                }

                if (TryGetPropertyValue(instance, memberName, out value))
                {
                    return true;
                }

                if (TryGetFieldValue(instance, memberName, out value))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to get value of member '{0}.{1}', skipping item during serialization", instance.GetType().GetSafeFullName(false), memberName);
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Gets the property value using reflection.
        /// </summary>
        /// <typeparam name="TValue">The type of the member.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="value">The value which will be updated if the member was read.</param>
        /// <returns><c>true</c> if the member value was retrieved; otherwise <c>false</c>.</returns>
        protected virtual bool TryGetPropertyValue<TValue>(object instance, string memberName, out TValue? value)
        {
            var propertyInfo = instance.GetType().GetPropertyEx(memberName);
            if (propertyInfo is not null)
            {
                value = (TValue?)propertyInfo.GetValue(instance, null);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the field value using reflection.
        /// </summary>
        /// <typeparam name="TValue">The type of the member.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="value">The value which will be updated if the member was read.</param>
        /// <returns><c>true</c> if the member value was retrieved; otherwise <c>false</c>.</returns>
        protected virtual bool TryGetFieldValue<TValue>(object instance, string memberName, out TValue? value)
        {
            var fieldInfo = instance.GetType().GetFieldEx(memberName);
            if (fieldInfo is not null)
            {
                value = (TValue?)fieldInfo.GetValue(instance);
                return true;
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
        public virtual bool TrySetMemberValue<TValue>(object instance, string memberName, TValue? value)
        {
            try
            {
                var modelEditor = instance as IModelEditor;
                if (modelEditor is not null && modelEditor.IsPropertyRegistered(memberName))
                {
                    modelEditor.SetValue(memberName, value);
                    return true;
                }

                if (TrySetPropertyValue(instance, memberName, value))
                {
                    return true;
                }

                if (TrySetFieldValue(instance, memberName, value))
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
        /// Sets the member value using reflection.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="value">The value to set the member to.</param>
        /// <returns><c>true</c> if the value as successfully set; otherwise <c>false</c>.</returns>
        protected virtual bool TrySetPropertyValue<TValue>(object instance, string memberName, TValue value)
        {
            var propertyInfo = instance.GetType().GetPropertyEx(memberName);
            if (propertyInfo is not null)
            {
                propertyInfo.SetValue(instance, value, null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the member value using reflection.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="value">The value to set the member to.</param>
        /// <returns><c>true</c> if the value as successfully set; otherwise <c>false</c>.</returns>
        protected virtual bool TrySetFieldValue<TValue>(object instance, string memberName, TValue value)
        {
            var fieldInfo = instance.GetType().GetFieldEx(memberName);
            if (fieldInfo is not null)
            {
                fieldInfo.SetValue(instance, value);
                return true;
            }

            return false;
        }
    }
}
