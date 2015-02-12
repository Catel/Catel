// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceLocatorInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    /// <summary>
    /// If a class implements this interface, it will automatically be called when a new <see cref="IServiceLocator"/>
    /// is created.
    /// </summary>
    public interface IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        void Initialize(IServiceLocator serviceLocator);
    }
}