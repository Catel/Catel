// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelNotRegisteredException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Exception for when a model is not registered on a class, but a <see cref="ViewModelToModelAttribute"/> is used with the model.
    /// </summary>
    public class ModelNotRegisteredException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelNotRegisteredException"/> class.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="propertyDeclaringViewModelToModelAttribute">The property declaring the view model to model attribute.</param>
        public ModelNotRegisteredException(string modelName, string propertyDeclaringViewModelToModelAttribute)
            : base(string.Format(ResourceHelper.GetString("ModelNotRegistered"), modelName, propertyDeclaringViewModelToModelAttribute))
        {
            ModelName = modelName;
            PropertyDeclaringViewModelToModelAttribute = propertyDeclaringViewModelToModelAttribute;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <value>The name of the model.</value>
        public string ModelName { get; private set; }

        /// <summary>
        /// Gets the property declaring the view model to model attribute.
        /// </summary>
        /// <value>The property declaring the view model to model attribute.</value>
        public string PropertyDeclaringViewModelToModelAttribute { get; private set; }
        #endregion
    }
}