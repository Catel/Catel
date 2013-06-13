// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateBindingOnTextChanged.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Data;

    /// <summary>
    /// Behavior base for all behaviors that should update a binding.
    /// </summary>
    public class UpdateBindingBehaviorBase<T> : BehaviorBase<TextBox>
        where T : FrameworkElement
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateBindingOnTextChanged"/> class.
        /// </summary>
        /// <param name="dependencyPropertyName">Name of the dependency property.</param>
        /// <exception cref="ArgumentException">The <paramref name="dependencyPropertyName"/> is <c>null</c> or whitespace.</exception>
        public UpdateBindingBehaviorBase(string dependencyPropertyName)
        {
            Argument.IsNotNullOrWhitespace("dependencyPropertyName", dependencyPropertyName);

            DependencyPropertyName = dependencyPropertyName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the dependency property.
        /// </summary>
        /// <remarks></remarks>
        protected string DependencyPropertyName { get; private set; }

        /// <summary>
        /// Gets the dependency property, which is retrieved at runtime.
        /// <para />
        /// This property can only be used when the associated object is attached.
        /// </summary>
        protected DependencyProperty DependencyProperty { get { return AssociatedObject.GetDependencyPropertyByName(DependencyPropertyName); } }
        #endregion

        #region Methods
        /// <summary>
        ///   Updates the binding value.
        /// </summary>
        protected void UpdateBinding()
        {
            var binding = AssociatedObject.GetBindingExpression(DependencyProperty);
            binding.UpdateSource();
        }
        #endregion
    }
}