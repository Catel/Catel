// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrSpecificationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.Data.Specifications
{
    using System.Linq;
    using Catel.Data.Specifications;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class OrSpecificationFacts
    {
        public class DeletedOrOutofStockSpecification : OrSpecification<Product>
        {
            public DeletedOrOutofStockSpecification() 
                : base(new DeletedProductSpecification(), new OutOfStockProductSpecification())
            {
            }
        }

        [TestMethod]
        public void WorksCorrectly()
        {
            var allProducts = SpecificationTestData.CreateDefaultCollection();
            var specification = new DeletedOrOutofStockSpecification();

            var filteredProducts = allProducts.Where(specification).ToList();

            Assert.AreEqual(3, filteredProducts.Count);

            // Deleted item
            Assert.AreEqual(true, filteredProducts[0].IsDeleted);
            Assert.AreEqual(false, filteredProducts[0].IsOutOfStock);

            // Out of stock item
            Assert.AreEqual(false, filteredProducts[1].IsDeleted);
            Assert.AreEqual(true, filteredProducts[1].IsOutOfStock);

            // Deleted and out of stock
            Assert.AreEqual(true, filteredProducts[2].IsDeleted);
            Assert.AreEqual(true, filteredProducts[2].IsOutOfStock);
        }
    }
}