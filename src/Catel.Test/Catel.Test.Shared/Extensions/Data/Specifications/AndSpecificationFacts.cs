// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndSpecificationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.Data.Specifications
{
    using System.Linq;
    using Catel.Data.Specifications;

    using NUnit.Framework;

    [TestFixture]
    public class AndSpecificationFacts
    {
        public class DeletedAndOutofStockSpecification : AndSpecification<Product>
        {
            public DeletedAndOutofStockSpecification()
                : base(new DeletedProductSpecification(), new OutOfStockProductSpecification())
            {
            }
        }

        [TestCase]
        public void WorksCorrectly()
        {
            var allProducts = SpecificationTestData.CreateDefaultCollection();
            var specification = new DeletedAndOutofStockSpecification();

            var filteredProducts = allProducts.Where(specification).ToList();

            Assert.AreEqual(1, filteredProducts.Count);

            // Deleted and out of stock
            Assert.AreEqual(true, filteredProducts[0].IsDeleted);
            Assert.AreEqual(true, filteredProducts[0].IsOutOfStock);
        }
    }
}