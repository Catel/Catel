// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestedInAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

	/// <summary>
	/// Attribute to decorate view models with. When a view model is decorated with this attribute, it will automatically
	/// receive property change notifications for the view models.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class InterestedInAttribute : Attribute
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="InterestedInAttribute"/> class.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
		public InterestedInAttribute(Type viewModelType)
		{
            Argument.IsNotNull("viewModelType", viewModelType);

			ViewModelType = viewModelType;
		}

		/// <summary>
		/// Gets the type of the view model.
		/// </summary>
		/// <value>The type of the view model.</value>
		public Type ViewModelType { get; private set; }
	}
}
