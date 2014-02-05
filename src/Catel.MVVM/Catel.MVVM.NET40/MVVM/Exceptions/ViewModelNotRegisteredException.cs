// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelNotRegisteredException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
            : base(string.Format(ResourceHelper.GetString("ViewModelNotRegistered"), viewModelType.Name))
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