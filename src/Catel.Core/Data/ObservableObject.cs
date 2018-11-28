// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableObject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Xml.Schema;

#if !NET && !NETCORE && !NETSTANDARD
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Very basic class implementing the <see cref="INotifyPropertyChanged"/> interfaces.
    /// </summary>
#if NET || NETCORE || NETSTANDARD
    [Serializable]
#else
    [DataContract]
#endif
    public class ObservableObject : IAdvancedNotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Occurs when a property of this object has changed.
        /// </summary>
#if NET || NETCORE || NETSTANDARD
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="TProperty">The type of the object holding the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <example>
        /// <![CDATA[
        ///     RaisePropertyChanged(() => IsDirty);
        /// ]]>
        /// </example>
        protected internal void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            var sender = ExpressionHelper.GetOwner(propertyExpression) ?? this;
            var propertyName = ExpressionHelper.GetPropertyName(propertyExpression);

            RaisePropertyChanged(sender, propertyName);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="TProperty">The type of the object holding the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="newValue">The new value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <example>
        /// <![CDATA[
        /// RaisePropertyChanged(() => IsDirty, true);
        /// ]]>
        /// </example>
        protected internal void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression, object newValue)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            var sender = ExpressionHelper.GetOwner(propertyExpression) ?? this;
            var propertyName = ExpressionHelper.GetPropertyName(propertyExpression);

            RaisePropertyChanged(sender, propertyName, newValue);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="TProperty">The type of the object holding the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <example>
        /// <![CDATA[
        /// RaisePropertyChanged(() => IsDirty, false, true);
        /// ]]>
        /// </example>
        protected internal void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression, object oldValue, object newValue)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            var sender = ExpressionHelper.GetOwner(propertyExpression) ?? this;
            var propertyName = ExpressionHelper.GetPropertyName(propertyExpression);

            RaisePropertyChanged(sender, propertyName, oldValue, newValue);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected internal void RaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(this, propertyName);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        protected internal void RaisePropertyChanged(string propertyName, object newValue)
        {
            RaisePropertyChanged(this, propertyName, newValue);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected internal void RaisePropertyChanged(string propertyName, object oldValue, object newValue)
        {
            RaisePropertyChanged(this, propertyName, oldValue, newValue);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected internal void RaisePropertyChanged(object sender, string propertyName)
        {
            // This is 1 of the 3 places where the AdvancedPropertyChangedEventArgs are created
            var eventArgs = new AdvancedPropertyChangedEventArgs(sender, propertyName);

            RaisePropertyChanged(sender, eventArgs);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        protected internal void RaisePropertyChanged(object sender, string propertyName, object newValue)
        {
            // This is 1 of the 3 places where the AdvancedPropertyChangedEventArgs are created
            var eventArgs = new AdvancedPropertyChangedEventArgs(sender, propertyName, newValue);

            RaisePropertyChanged(sender, eventArgs);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected internal void RaisePropertyChanged(object sender, string propertyName, object oldValue, object newValue)
        {
            // This is 1 of the 3 places where the AdvancedPropertyChangedEventArgs are created
            var eventArgs = new AdvancedPropertyChangedEventArgs(sender, propertyName, oldValue, newValue);

            RaisePropertyChanged(sender, eventArgs);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// <para />
        /// This is the one and only method that actually raises the <see cref="PropertyChanged"/> event. All other
        /// methods are (and should be) just overloads that eventually call this method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void RaisePropertyChanged(object sender, AdvancedPropertyChangedEventArgs e)
        {
            PropertyChanged.SafeInvoke(sender, e);

            if (ReferenceEquals(this, sender))
            {
                OnPropertyChanged(e);
            }
        }

        /// <summary>
        /// Called when the <see cref="PropertyChanged"/> event occurs.
        /// </summary>
        /// <param name="e">The <see cref="AdvancedPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
        }
        #endregion
    }
}
