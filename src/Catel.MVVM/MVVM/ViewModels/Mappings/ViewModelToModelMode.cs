namespace Catel.MVVM
{ 
    /// <summary>
    /// Specifies the different mapping modes available for the <see cref="ViewModelToModelAttribute" />.
    /// </summary>
    public enum ViewModelToModelMode
    {
        /// <summary>
        /// Automatically maps the property from view model to model and back as soon
        /// as either one changes the property value.
        /// </summary>
        TwoWay,

        /// <summary>
        /// Automatically maps the property from the model to the view model if the model
        /// changes the property value.
        /// <para />
        /// This mode does not map any values from the view model to the model, thus can also
        /// be seen as read-only mode.
        /// </summary>
        OneWay,

        /// <summary>
        /// Automatically maps the property from the view model to the model if the view model
        /// changes the property value.
        /// <para />
        /// This mode does not map any values from the model to the view model, but still keeps track
        /// of all validation that occurs in the model.
        /// </summary>
        OneWayToSource,

        /// <summary>
        /// Automatically maps properties from the model to the view model as soon as the model is initialized. As 
        /// soon as a property value changes in the model, the view model value is updated instantly. However,
        /// the mapping from the view model to model is explicit.
        /// </summary>
        Explicit,
    }
}
