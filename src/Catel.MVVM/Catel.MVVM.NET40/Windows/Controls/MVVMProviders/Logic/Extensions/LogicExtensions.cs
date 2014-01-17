// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using Reflection;
    using Windows.Controls.MVVMProviders.Logic;

    /// <summary>
    /// Extension methods to safely interact with logic from inside views.
    /// </summary>
    public static class LogicExtensions
    {
        /// <summary>
        /// Sets the value of the logic property.
        /// </summary>
        /// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        /// <param name="action">The action that will set the actual value, will only be executed if <paramref name="logic"/> is not <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static void SetValue<TLogic>(this LogicBase logic, Action<TLogic> action)
            where TLogic : LogicBase
        {
            Argument.IsNotNull("action", action);

            if (logic == null)
            {
                return;
            }

            action((TLogic)logic);
        }

        /// <summary>
        /// Sets the value of the logic property.
        /// </summary>
        /// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        /// <param name="function">The function that will get the actual value, will only be executed if <paramref name="logic"/> is not <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="function"/> is <c>null</c>.</exception>
        public static TValue GetValue<TLogic, TValue>(this LogicBase logic, Func<TLogic, TValue> function)
            where TLogic : LogicBase
        {
            return GetValue(logic, function, default(TValue));
        }

        /// <summary>
        /// Sets the value of the logic property.
        /// </summary>
        /// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        /// <param name="function">The function that will get the actual value, will only be executed if <paramref name="logic"/> is not <c>null</c>.</param>
        /// <param name="defaultValue">The default value to return if the logic is not available.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="function"/> is <c>null</c>.</exception>
        public static TValue GetValue<TLogic, TValue>(this LogicBase logic, Func<TLogic, TValue> function, TValue defaultValue)
            where TLogic : LogicBase
        {
            Argument.IsNotNull("function", function);

            if (logic == null)
            {
                return defaultValue;
            }

            return function((TLogic) logic);
        }

        ///// <summary>
        ///// Sets the value of the logic property.
        ///// </summary>
        ///// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <param name="value">The value.</param>
        ///// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        //public static void SetValue(this LogicBase logic, string propertyName, object value)
        //{
        //    Argument.IsNotNullOrWhitespace("propertyName", propertyName);

        //    if (logic == null)
        //    {
        //        return;
        //    }

        //    PropertyHelper.SetPropertyValue(logic, propertyName, value);
        //}

        ///// <summary>
        ///// Gets the value of the logic property.
        ///// </summary>
        ///// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <returns>The property value.</returns>
        ///// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        //public static TValue GetValue<TValue>(this LogicBase logic, string propertyName)
        //{
        //    return GetValue(logic, propertyName, default(TValue));
        //}

        ///// <summary>
        ///// Gets the value of the logic property.
        ///// </summary>
        ///// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <param name="defaultValue">The default value to return if the logic is not available.</param>
        ///// <returns>The property value.</returns>
        ///// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        //public static TValue GetValue<TValue>(this LogicBase logic, string propertyName, TValue defaultValue)
        //{
        //    Argument.IsNotNullOrWhitespace("propertyName", propertyName);

        //    if (logic == null)
        //    {
        //        return defaultValue;
        //    }

        //    return PropertyHelper.GetPropertyValue<TValue>(logic, propertyName);
        //}
    }
}