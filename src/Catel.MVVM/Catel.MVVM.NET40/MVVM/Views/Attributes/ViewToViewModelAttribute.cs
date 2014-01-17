// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewToViewModelAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using System.Windows;

    #region Enums
    /// <summary>
    /// Mapping types for the <see cref="ViewToViewModelAttribute"/>.
    /// </summary>
    public enum ViewToViewModelMappingType
    {
        /// <summary>
        /// Two way, which means that either the view or the view model will update
        /// the values of the other party as soon as they are updated.
        /// <para />
        /// When this value is used, nothing happens when the view model of the view
        /// changes. This way, it might be possible that the values of the view and the
        /// view model are different. The first one to update next will update the other.
        /// </summary>
        TwoWayDoNothing,

        /// <summary>
        /// Two way, which means that either the view or the view model will update
        /// the values of the other party as soon as they are updated.
        /// <para />
        /// When this value is used, the value of the view is used when the view model 
        /// of the view is changed, and is directly transferred to the view model value.
        /// </summary>
        TwoWayViewWins,

        /// <summary>
        /// Two way, which means that either the view or the view model will update
        /// the values of the other party as soon as they are updated.
        /// <para />
        /// When this value is used, the value of the view model is used when the view model 
        /// of the view is changed, and is directly transferred to the view value.
        /// </summary>
        TwoWayViewModelWins,

        /// <summary>
        /// The mapping is from the view to the view model only.
        /// </summary>
        ViewToViewModel,

        /// <summary>
        /// The mapping is from the view model to the view only.
        /// </summary>
        ViewModelToView
    }
    #endregion

    /// <summary>
    /// A mapper attribute to map a <see cref="FrameworkElement"/> (such as the UserControl or the DataWindow) 
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
