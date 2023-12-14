namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Exception in case a view model is not registered, but still being used.
    /// </summary>
    public class ViewModelNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelNotRegisteredException"/> class.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        public ViewModelNotRegisteredException(Type viewModelType)
            : base(string.Format(ResourceHelper.GetString("ViewModelNotRegistered") ?? string.Empty, viewModelType.Name))
        {
            ViewModelType = viewModelType;
        }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>The type of the view model.</value>
        public Type ViewModelType { get; private set; }
    }
}
