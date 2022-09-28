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

    /// <summary>
    /// Very basic class implementing the <see cref="INotifyPropertyChanged"/> interfaces.
    /// </summary>
    [Serializable]
    public class ObservableObject : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Occurs when a property of this object has changed.
        /// </summary>
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

            var propertyName = ExpressionHelper.GetPropertyName(propertyExpression);

            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected internal void RaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChangedDirect(sender, e); 
            
            if (ReferenceEquals(this, sender))
            {
                OnPropertyChanged(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event without allowing classes to override this behavior.
        /// <para />
        /// This is the one and only method that actually raises the <see cref="PropertyChanged"/> event. All other
        /// methods are (and should be) just overloads that eventually call this method.
        /// <para />
        /// Note that this method does not call <see cref="OnPropertyChanged(PropertyChangedEventArgs)"/>. Use 
        /// <see cref="RaisePropertyChanged(object, PropertyChangedEventArgs)"/> if that is required.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void RaisePropertyChangedDirect(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Called when the <see cref="PropertyChanged"/> event occurs.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
        }
        #endregion
    }
}
