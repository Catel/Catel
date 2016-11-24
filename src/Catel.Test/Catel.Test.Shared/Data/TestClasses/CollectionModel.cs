// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System.Collections.ObjectModel;
    using Catel.Data;

    public class CollectionModel : ModelBase
    {
        public CollectionModel(bool initializeValues)
        {
            Items = new ObservableCollection<ObservableCollection<CollectionModel>>();

            if (initializeValues)
            {
                for (int i = 0; i < 5; i++)
                {
                    var innerCollection = new ObservableCollection<CollectionModel>();
                    Items.Add(innerCollection);

                    for (int j = 0; j < 3; j++)
                    {
                        innerCollection.Add(new CollectionModel(false) {Name = string.Format("Subitem {0}", j + 1)});
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<ObservableCollection<CollectionModel>> Items
        {
            get { return GetValue<ObservableCollection<ObservableCollection<CollectionModel>>>(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Register the Items property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ItemsProperty = RegisterProperty("Items", typeof(ObservableCollection<ObservableCollection<CollectionModel>>));


        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
    }
}