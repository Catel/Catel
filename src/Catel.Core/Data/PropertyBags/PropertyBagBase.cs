namespace Catel.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Base class for property bags that take care of common logic such as change notifications.
    /// </summary>
    public abstract class PropertyBagBase : IPropertyBag
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        public abstract string[] GetAllNames();

        public abstract TValue GetValue<TValue>(string name, TValue defaultValue = default!);

        public abstract bool IsAvailable(string name);

        public abstract void SetValue<TValue>(string name, TValue value);

        public abstract Dictionary<string, object> GetAllProperties();

        /// <summary>
        /// Raises the property changed notification event for the specified property.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is not null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
