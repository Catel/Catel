// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN

namespace Catel.MVVM
{
    using Logging;

    /// <summary>
    /// Base class for all bindings.
    /// </summary>
    public abstract class BindingBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private string _toStringValue;

        /// <summary>
        /// Determines the value to use in the <see cref="ToString"/> method.
        /// </summary>
        /// <returns>The string to use.</returns>
        protected abstract string DetermineToString();

        /// <summary>
        /// Uninitializes this binding.
        /// </summary>
        protected abstract void Uninitialize();

        private void UninitializeBinding()
        {
            Uninitialize();

            Log.Debug("Uninitialized binding '{0}'", this);
        }

        /// <summary>
        /// Converts the current instance to a string.
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            if (_toStringValue == null)
            {
                _toStringValue = DetermineToString();
            }

            return _toStringValue;
        }

        /// <summary>
        /// Clears the binding and stops listening to both the source and target instances.
        /// </summary>
        public void ClearBinding()
        {
            UninitializeBinding();
        }
    }
}

#endif