// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingWithValidation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Windows.Data
{
    using System.Windows.Data;

    /// <summary>
    /// Binding that automatically enables <see cref="Binding.NotifyOnValidationError"/> and <see cref="Binding.ValidatesOnDataErrors"/>.
    /// </summary>
    public class BindingWithValidation : Binding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingWithValidation"/> class.
        /// </summary>
        public BindingWithValidation()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingWithValidation" /> class with an initial path.
        /// </summary>
        /// <param name="path">The initial <see cref="P:System.Windows.Data.Binding.Path" /> for the binding.</param>
        public BindingWithValidation(string path)
            : base(path)
        {
            NotifyOnValidationError = true;
            ValidatesOnDataErrors = true;
        }
    }
}

#endif
