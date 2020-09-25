namespace Catel.Tests.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Catel.Data;

#if NET || NETCORE
    [Serializable]
#endif
    public class Parent : SavableModelBase<Parent>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Parent()
        {
        }

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public Parent(string name)
        {
            // Store values
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the name of the parent.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the child collection.
        /// </summary>
        public Collection<Child> Children
        {
            get { return GetValue<Collection<Child>>(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ChildrenProperty = RegisterProperty("Children", typeof(Collection<Child>), new Collection<Child>());
        #endregion

        #region Methods
        /// <summary>
        ///   Creates a new child object.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <returns>New created child.</returns>
        public Child CreateChild(string name)
        {
            Child child = new Child(this, name);
            Children.Add(child);
            return child;
        }
        #endregion
    }
}
