// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using Catel.IoC;
    using Catel.Test.EntityFramework5.DbContextTest.Repositories;

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        #region Methods

        /// <summary>
        /// Initializes the module
        /// </summary>
        public static void Initialize()
        {
                var serviceLocator = ServiceLocator.Default;

                serviceLocator.RegisterType<IDbContextCustomerRepository, DbContextCustomerRepository>(registrationType: RegistrationType.Transient);
                serviceLocator.RegisterType<IDbContextOrderRepository, DbContextOrderRepository>(registrationType: RegistrationType.Transient);
                serviceLocator.RegisterType<IDbContextProductRepository, DbContextProductRepository>(registrationType: RegistrationType.Transient);
        }

        #endregion
    }
}