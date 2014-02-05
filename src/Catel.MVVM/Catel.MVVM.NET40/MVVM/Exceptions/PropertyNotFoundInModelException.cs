// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNotFoundInModelException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
	/// Exception in case a mapped property is not found on the model.
	/// </summary>
	public class PropertyNotFoundInModelException : Exception
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundInModelException"/> class.
		/// </summary>
		/// <param name="viewModelPropertyName">Name of the view model property.</param>
		/// <param name="modelName">Name of the model.</param>
		/// <param name="modelPropertyName">Name of the model property.</param>
		public PropertyNotFoundInModelException(string viewModelPropertyName, string modelName, string modelPropertyName)
			: base(string.Format(ResourceHelper.GetString("PropertyNotFoundInModel"), viewModelPropertyName, modelPropertyName, modelName))
		{
			ViewModelPropertyName = viewModelPropertyName;
			ModelName = modelName;
			ModelPropertyName = modelPropertyName;
		}
		#endregion

		#region Properties
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
		#endregion
	}
}
