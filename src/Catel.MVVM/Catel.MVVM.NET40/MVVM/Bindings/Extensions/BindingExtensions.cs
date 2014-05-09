// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN

namespace Catel.MVVM
{
    using System;
    using System.Linq.Expressions;
    using Converters;
    using IoC;

    /// <summary>
    /// Extension methods to create bindings on platforms that initially don't support bindings.
    /// </summary>
    public static class BindingExtensions
    {
        #region Methods
        /// <summary>
        /// Adds a new binding to the source object.
        /// </summary>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetPropertyName">Name of the target property.</param>
        /// <param name="mode">The binding mode.</param>
        /// <param name="converter">The converter, can be set afterwards as well.</param>
        /// <returns>The <see cref="Binding"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public static Binding AddBinding(this BindingContext bindingContext, object source, string sourcePropertyName, object target, string targetPropertyName, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null)
        {
            var binding = new Binding(source, sourcePropertyName, target, targetPropertyName, mode, converter);

            bindingContext.AddBinding(binding);

            return binding;
        }

        /// <summary>
        /// Adds a new binding to the source object.
        /// </summary>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="sourcePropertyExpression">The source property expression.</param>
        /// <param name="targetPropertyExpression">The target property expression.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="converter">The converter, can be set afterwards as well.</param>
        /// <returns>The <see cref="Binding"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="sourcePropertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetPropertyExpression"/> is <c>null</c>.</exception>
        public static Binding AddBinding(this BindingContext bindingContext, Expression<Func<object>> sourcePropertyExpression, Expression<Func<object>> targetPropertyExpression, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null)
        {
            Argument.IsNotNull("sourcePropertyExpression", sourcePropertyExpression);
            Argument.IsNotNull("targetPropertyExpression", targetPropertyExpression);

            var source = ExpressionHelper.GetOwner(sourcePropertyExpression);
            var sourceProperty = ExpressionHelper.GetPropertyName(sourcePropertyExpression);

            var target = ExpressionHelper.GetOwner(targetPropertyExpression);
            var targetProperty = ExpressionHelper.GetPropertyName(targetPropertyExpression);

            return AddBinding(bindingContext, source, sourceProperty, target, targetProperty, mode, converter);
        }

        /// <summary>
        /// Adds a new binding to the source object and automatically instantiates the converter.
        /// </summary>
        /// <typeparam name="TConverter">The type of the t converter.</typeparam>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetPropertyName">Name of the target property.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The <see cref="Binding"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public static Binding AddBindingWithConverter<TConverter>(this BindingContext bindingContext, object source, string sourcePropertyName, object target, string targetPropertyName, BindingMode mode = BindingMode.TwoWay)
            where TConverter : IValueConverter
        {
            var typeFactory = IoCConfiguration.DefaultTypeFactory;
            var converter = typeFactory.CreateInstance<TConverter>();

            return AddBinding(bindingContext, source, sourcePropertyName, target, targetPropertyName, mode, converter);
        }

        /// <summary>
        /// Adds a new binding to the source object and automatically instantiates the converter.
        /// </summary>
        /// <typeparam name="TConverter">The type of the converter.</typeparam>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="sourcePropertyExpression">The source property expression.</param>
        /// <param name="targetPropertyExpression">The target property expression.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The <see cref="Binding"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="sourcePropertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetPropertyExpression"/> is <c>null</c>.</exception>
        public static Binding AddBindingWithConverter<TConverter>(this BindingContext bindingContext, Expression<Func<object>> sourcePropertyExpression, Expression<Func<object>> targetPropertyExpression, BindingMode mode = BindingMode.TwoWay)
            where TConverter : IValueConverter
        {
            var typeFactory = IoCConfiguration.DefaultTypeFactory;
            var converter = typeFactory.CreateInstance<TConverter>();

            return AddBinding(bindingContext, sourcePropertyExpression, targetPropertyExpression, mode, converter);
        }

        /// <summary>
        /// Adds an additional event subscription to support change notification.
        /// <para />
        /// This extension method will use the <see cref="Binding.Target" /> to add an event.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Catel.MVVM.Binding.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binding"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        public static Binding AddTargetEvent(this Binding binding, string eventName)
        {
            Argument.IsNotNull("binding", binding);
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            binding.Target.AddEvent(eventName);

            return binding;
        }

        /// <summary>
        /// Adds an additional event subscription to support change notification.
        /// <para />
        /// This extension method will use the <see cref="Binding.Source" /> to add an event.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Catel.MVVM.Binding.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binding"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        public static Binding AddSourceEvent(this Binding binding, string eventName)
        {
            Argument.IsNotNull("binding", binding);
            Argument.IsNotNullOrWhitespace("eventName", eventName);

            binding.Source.AddEvent(eventName);

            return binding;
        }

        /// <summary>
        /// Adds a new command binding to the element.
        /// </summary>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="element">The element.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandParameterBinding">The command parameter binding.</param>
        /// <returns>Catel.MVVM.CommandBinding.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public static CommandBinding AddCommandBinding(this BindingContext bindingContext, object element, string eventName, ICatelCommand command, Binding commandParameterBinding = null)
        {
            var commandBinding = new CommandBinding(element, eventName, command, commandParameterBinding);

            bindingContext.AddCommandBinding(commandBinding);

            return commandBinding;
        }

        /// <summary>
        /// Gets the binding value. 
        /// <para />
        /// If the <paramref name="binding"/> is <c>null</c>, this method will return <c>null</c>.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns>The binding value.</returns>
        public static object GetBindingValue(this Binding binding)
        {
            if (binding == null)
            {
                return null;
            }

            return binding.Value;
        }
        #endregion
    }
}

#endif