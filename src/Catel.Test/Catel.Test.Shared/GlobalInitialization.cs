// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalInitialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Linq;
using NUnit.Framework;

/// <summary>
/// Sets the current culture to <c>en-US</c> for all unit tests to prevent tests to fail
/// due to cultural string differences.
/// </summary>
[SetUpFixture]
public class GlobalInitialization
{
    [SetUp]
    public static void SetUp()
    {
        //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

#if NET
        using (var dbContext = new Catel.Test.EntityFramework5.DbContextTest.TestDbContextContainer())
        {
            dbContext.Database.CreateIfNotExists();

            // Delete all data
            var allOrders = (from x in dbContext.DbContextOrders
                             select x).ToList();
            foreach (var x in allOrders)
            {
                dbContext.DbContextOrders.Remove(x);
            }

            var allCustomers = (from x in dbContext.DbContextCustomers
                                select x).ToList();
            foreach (var x in allCustomers)
            {
                dbContext.DbContextCustomers.Remove(x);
            }

            var allProducts = (from x in dbContext.DbContextProducts
                               select x).ToList();
            foreach (var x in allProducts)
            {
                dbContext.DbContextProducts.Remove(x);
            }

            dbContext.SaveChanges();
        }
#endif
    }
}