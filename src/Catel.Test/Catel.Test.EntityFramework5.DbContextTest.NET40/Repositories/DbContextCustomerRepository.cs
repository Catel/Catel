// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextCustomerRepository.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.EntityFramework5.DbContextTest.Repositories
{
    using System.Data.Entity;

    using Catel.Data.Repositories;

    public class DbContextCustomerRepository : EntityRepositoryBase<DbContextCustomer, int>
    {
        public DbContextCustomerRepository(DbContext dbContext)
            : base(dbContext)
        {   
        }
    }
}