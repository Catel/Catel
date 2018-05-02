// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelWithCollectionViewSource.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Data
{
    using System.Collections.Generic;
    using System.Windows.Data;
    using Catel.Data;
    using Catel.MVVM;

    public class ModelWithCollectionViewSource : ViewModelBase
    {
        /// <summary>
        /// Register the Collection property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CollectionProperty = RegisterProperty("Collection", typeof(CollectionView), () => new CollectionView(new List<int>() {1, 2, 3}));

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public CollectionView Collection
        {
            get { return GetValue<CollectionView>(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }
    }
}