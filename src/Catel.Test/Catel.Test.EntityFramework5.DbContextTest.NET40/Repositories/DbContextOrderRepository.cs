// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextOrderRepository.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.EntityFramework5.DbContextTest.Repositories
{
    using System.Data.Entity;

    using Catel.Data.Repositories;

    public class DbContextOrderRepository : EntityRepositoryBase<DbContextOrder, int>
    {
        #region Constructors
        public DbContextOrderRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
        #endregion
    }
}