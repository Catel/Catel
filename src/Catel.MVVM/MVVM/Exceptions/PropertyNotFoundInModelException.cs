﻿namespace Catel.MVVM
{
    using System;
    using Catel.Properties;

    /// <summary>
    /// Exception in case a mapped property is not found on the model.
    /// </summary>
    public class PropertyNotFoundInModelException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundInModelException"/> class.
		/// </summary>
		/// <param name="viewModelPropertyName">Name of the view model property.</param>
		/// <param name="modelName">Name of the model.</param>
		/// <param name="modelPropertyName">Name of the model property.</param>
		public PropertyNotFoundInModelException(string viewModelPropertyName, string modelName, string modelPropertyName)
			: base(string.Format(Exceptions.PropertyNotFoundInModel ?? string.Empty, viewModelPropertyName, modelPropertyName, modelName))
		{
			ViewModelPropertyName = viewModelPropertyName;
			ModelName = modelName;
			ModelPropertyName = modelPropertyName;
		}

		/// <summary>
		/// Gets the name of the view model property.
		/// </summary>
		/// <value>The name of the view model property.</value>
		public string ViewModelPropertyName { get; private set; }

		/// <summary>
		/// Gets the name of the model.
		/// </summary>
		/// <value>The name of the model.</value>
		public string ModelName { get; private set; }

		/// <summary>
		/// Gets the name of the model property.
		/// </summary>
		/// <value>The name of the model property.</value>
		public string ModelPropertyName { get; private set; }
	}
}
