// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel
{
    using System;
    using IoC;
    using MVVM;

    /// <summary>
    /// The <see cref="IViewModel"/> interface extension methods.
    /// </summary>
    public static class IViewModelExtensions
    {
        #region Methods
        /// <summary>
        /// Gets the service of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <param name="this">The instance.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>Service object or <c>null</c> if the service is not found.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="this"/> is <c>null</c>.</exception>
        [ObsoleteEx(Message = "GetService is no longer recommended. It is better to inject all dependencies (which the TypeFactory fully supports)", TreatAsErrorFromVersion = "3.7", RemoveInVersion = "4.0")]
        public static T GetService<T>(this IViewModel @this, object tag = null)
        {
            //Argument.IsNotNull("@this", @this);

            //return @this is ViewModelBase ? (@this as ViewModelBase).GetService<T>(tag) : (T)ServiceLocator.Default.ResolveType(typeof(T), tag);

            throw new NotSupportedException();
        }
        #endregion
    }
}