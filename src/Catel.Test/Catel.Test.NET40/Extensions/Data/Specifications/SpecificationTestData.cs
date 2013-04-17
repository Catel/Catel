// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecificationTestData.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.Data.Specifications
{
    using System.Collections.Generic;
    using Catel.Data.Specifications;

    public class DeletedProductSpecification : Specification<Product>
    {
        public DeletedProductSpecification() 
            : base(x => x.IsDeleted)
        {
        }
    }

    public class NotDeletedProductSpecification : Specification<Product>
    {
        public NotDeletedProductSpecification()
            : base(x => !x.IsDeleted)
        {
        }
    }

    public class OutOfStockProductSpecification : Specification<Product>
    {
        public OutOfStockProductSpecification()
            : base(x => x.IsOutOfStock)
        {
        }
    }

    public class NotOutOfStockProductSpecification : Specification<Product>
    {
        public NotOutOfStockProductSpecification()
            : base(x => !x.IsOutOfStock)
        {
        }
    }

    public class Product
    {
        #region Constructors
        public Product(string name, bool isOutOfStock, bool isDeleted)
        {
            Name = name;
            IsOutOfStock = isOutOfStock;
            IsDeleted = isDeleted;
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public bool IsOutOfStock { get; set; }
        public bool IsDeleted { get; set; }
        #endregion
    }

    public static class SpecificationTestData
    {
        public static List<Product> CreateDefaultCollection()
        {
            var list = new List<Product>();

            list.Add(new Product("Available product", false, false));
            list.Add(new Product("Deleted product", false, true));
            list.Add(new Product("Out of stock product", true, false));
            list.Add(new Product("Deleted out of stock product", true, true));

            return list;
        }
    }
}