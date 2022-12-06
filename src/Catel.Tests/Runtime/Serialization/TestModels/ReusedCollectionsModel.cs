namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System.Collections.Generic;
    using Catel.Data;

    public class ReusedCollectionsModel : ModelBase
    {
        public ReusedCollectionsModel()
        {
            var collection = new List<int>();

            for (var i = 0; i < 5; i++)
            {
                collection.Add(i + 1);
            }

            Collection1 = Collection2 = collection;
        }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public List<int> Collection1
        {
            get { return GetValue<List<int>>(Collection1Property); }
            set { SetValue(Collection1Property, value); }
        }

        /// <summary>
        /// Register the Collection1 property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData Collection1Property = RegisterProperty("Collection1", () => new List<int>());

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public List<int> Collection2
        {
            get { return GetValue<List<int>>(Collection2Property); }
            set { SetValue(Collection2Property, value); }
        }

        /// <summary>
        /// Register the Collection1 property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData Collection2Property = RegisterProperty("Collection2", () => new List<int>());
    }
}
