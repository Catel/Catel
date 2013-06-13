// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedPropertyChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.ComponentModel;

    using Reflection;

    /// <summary>
    /// Property changed event args that are used when a property has changed. The event arguments contains both
    /// the original sender as the current sender of the event.
    /// <para />
    /// Best used in combination with <see cref="IAdvancedNotifyPropertyChanged"/>.
    /// </summary>
    public class AdvancedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.Data.AdvancedPropertyChangedEventArgs"/>"/> instance containing the event data.</param>
        public AdvancedPropertyChangedEventArgs(object sender, AdvancedPropertyChangedEventArgs e)
            : this(e.OriginalSender, sender, e.PropertyName, e.OldValue, e.NewValue, e.IsOldValueMeaningful, e.IsNewValueMeaningful) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        public AdvancedPropertyChangedEventArgs(object sender, string propertyName)
            : this(sender, sender, propertyName, null, null, false, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        public AdvancedPropertyChangedEventArgs(object sender, string propertyName, object newValue)
            : this(sender, sender, propertyName, null, newValue, false, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public AdvancedPropertyChangedEventArgs(object sender, string propertyName, object oldValue, object newValue)
            : this(sender, sender, propertyName, oldValue, newValue, true, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="originalSender">The original sender.</param>
        /// <param name="latestSender">The latest sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        public AdvancedPropertyChangedEventArgs(object originalSender, object latestSender, string propertyName)
            : this(originalSender, latestSender, propertyName, null, null, false, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="originalSender">The original sender.</param>
        /// <param name="latestSender">The latest sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        public AdvancedPropertyChangedEventArgs(object originalSender, object latestSender, string propertyName, object newValue)
            : this(originalSender, latestSender, propertyName, null, newValue, false, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="originalSender">The original sender.</param>
        /// <param name="latestSender">The latest sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public AdvancedPropertyChangedEventArgs(object originalSender, object latestSender, string propertyName, object oldValue, object newValue)
            : this(originalSender, latestSender, propertyName, oldValue, newValue, true, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedPropertyChangedEventArgs"/>"/> class.
        /// </summary>
        /// <param name="originalSender">The original sender.</param>
        /// <param name="latestSender">The latest sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="isOldValueMeaningful">if set to <c>true</c>, the <paramref name="oldValue"/> has a meaningful value.</param>
        /// <param name="isNewValueMeaningful">if set to <c>true</c>, the <paramref name="newValue"/> has a meaningful value.</param>
        private AdvancedPropertyChangedEventArgs(object originalSender, object latestSender, string propertyName, object oldValue, object newValue,
            bool isOldValueMeaningful, bool isNewValueMeaningful)
            : base(propertyName)
        {
            OriginalSender = originalSender;
            LatestSender = latestSender;

            // Last resort to get the new value
            if (!isNewValueMeaningful && !string.IsNullOrEmpty(propertyName))
            {
                if (PropertyHelper.IsPropertyAvailable(originalSender, propertyName))
                {
                    if (PropertyHelper.TryGetPropertyValue(originalSender, propertyName, out newValue))
                    {
                        isNewValueMeaningful = true;
                    }
                }
            }

            OldValue = oldValue;
            NewValue = newValue;

            IsOldValueMeaningful = isOldValueMeaningful;
            IsNewValueMeaningful = isNewValueMeaningful;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the original sender.
        /// </summary>
        /// <value>The original sender.</value>
        public object OriginalSender { get; private set; }

        /// <summary>
        /// Gets the latest sender.
        /// </summary>
        /// <value>The latest sender.</value>
        public object LatestSender { get; private set; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OldValue"/> has any meaning. Sometimes it is not possible
        /// to determine the old value in case a 3rd party class triggered the <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event. In such a case, the <see cref="OldValue"/> will be <c>null</c>, but this does not mean that the previous 
        /// value was <c>null</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="OldValue"/> has a meaningful value; otherwise, <c>false</c>.
        /// </value>
        public bool IsOldValueMeaningful { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="NewValue"/> has any meaning. Sometimes it is not possible
        /// to determine the new value in case a 3rd party class triggered the <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event. In such a case, the <see cref="NewValue"/> will be <c>null</c>, but this does not mean that the new 
        /// value is <c>null</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="NewValue"/> has a meaningful value; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewValueMeaningful { get; private set; }
        #endregion
    }
}