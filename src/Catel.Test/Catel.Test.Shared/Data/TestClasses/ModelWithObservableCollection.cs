// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelWithObservableCollection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Catel.Data;

    public class ModelWithObservableCollection : ChildAwareModelBase
    {
        /// <summary>
        /// Register the Collection property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CollectionProperty = RegisterProperty("Collection", typeof(ObservableCollection<int>), () => new ObservableCollection<int>());

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<int> Collection
        {
            get { return GetValue<ObservableCollection<int>>(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public bool HasCollectionChanged { get; private set; }

        public bool HasPropertyChanged { get; private set; }

        protected override void OnPropertyObjectCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HasCollectionChanged = true;
        }

        protected override void OnPropertyObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasPropertyChanged = true;
        }
    }
}