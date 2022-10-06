namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// A mapper attribute to map a <see cref="IView"/> (such as the UserControl or the DataWindow) 
    /// property to a view model property.
    /// <para />
    /// This class is very useful when creating custom user controls that need more parameterized settings than just the
    /// <c>DataContext</c> property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ViewToViewModelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewToViewModelAttribute"/> class.
        /// </summary>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        public ViewToViewModelAttribute(string viewModelPropertyName = "")
        {
            MappingType = ViewToViewModelMappingType.TwoWayViewModelWins;

            ViewModelPropertyName = viewModelPropertyName;
        }

        /// <summary>
        /// Gets or sets the view model property name.
        /// </summary>
        /// <value>The view model property name.</value>
        public string ViewModelPropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the type of the mapping.
        /// </summary>
        /// <value>The type of the mapping.</value>
        public ViewToViewModelMappingType MappingType { get; set; }
    }
}
