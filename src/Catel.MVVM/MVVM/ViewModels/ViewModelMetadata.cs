namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class containing meta data for a view model type.
    /// </summary>
    internal class ViewModelMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelMetadata"/> class.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="models">The models.</param>
        /// <param name="mappings">The mappings.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="models"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="mappings"/> is <c>null</c>.</exception>
        internal ViewModelMetadata(Type viewModelType, Dictionary<string, ModelInfo> models, Dictionary<string, ViewModelToModelMapping> mappings)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);
            ArgumentNullException.ThrowIfNull(models);
            ArgumentNullException.ThrowIfNull(mappings);

            ViewModelType = viewModelType;
            Models = models;
            Mappings = mappings;
        }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the models.
        /// </summary>
        public Dictionary<string, ModelInfo> Models { get; private set; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        public Dictionary<string, ViewModelToModelMapping> Mappings { get; private set; }
    }
}
