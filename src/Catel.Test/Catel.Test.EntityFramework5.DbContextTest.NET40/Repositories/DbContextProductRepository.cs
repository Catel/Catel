// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextProductRepository.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.EntityFramework5.DbContextTest.Repositories
{
    using System.Data.Entity;

    using Catel.Data.Repositories;

    public class DbContextProductRepository : EntityRepositoryBase<DbContextProduct, int>
    {
        #region Constructors
        public DbContextProductRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
        #endregion
    }
}