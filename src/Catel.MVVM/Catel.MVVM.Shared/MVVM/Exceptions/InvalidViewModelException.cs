// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidViewModelException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Exception in case there is an invalid view model.
    /// </summary>
    public class InvalidViewModelException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidViewModelException"/> class.
        /// </summary>
        public InvalidViewModelException()
            : base(ResourceHelper.GetString("InvalidViewModel"))
        {
        }
    }
}