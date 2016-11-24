// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System.Collections.ObjectModel;
    using Catel.Data;

    public class TestModel : ModelBase
    {
        public TestModel()
        {
            CollectionModel = new ObservableCollection<LeanAndMeanModel>();
            CollectionModel.Add(new LeanAndMeanModel());
        }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public LeanAndMeanModel ModelWithPropertyChanged
        {
            get { return GetValue<LeanAndMeanModel>(ModelWithPropertyChangedProperty); }
            set { SetValue(ModelWithPropertyChangedProperty, value); }
        }

        /// <summary>
        /// Register the ModelWithPropertyChanged property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ModelWithPropertyChangedProperty = RegisterProperty("ModelWithPropertyChanged", typeof(LeanAndMeanModel), () => new LeanAndMeanModel());


        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<LeanAndMeanModel> CollectionModel
        {
            get { return GetValue<ObservableCollection<LeanAndMeanModel>>(CollectionModelProperty); }
            set { SetValue(CollectionModelProperty, value); }
        }

        /// <summary>
        /// Register the CollectionModel property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CollectionModelProperty = RegisterProperty("CollectionModel", typeof(ObservableCollection<LeanAndMeanModel>), null);
    }
}