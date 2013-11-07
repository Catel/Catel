// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObjectMetaData.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public interface IObjectMetaData
    {
        #region Properties
        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; set; }

        /// <summary>
        /// Gets or sets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Gets or sets the properties meta data.
        /// </summary>
        /// <value>
        /// The properties meta data.
        /// </value>
        IList<IObjectMetaData> PropertiesMetaData { get; set; }

        /// <summary>
        /// Gets or sets the type of the collection item.
        /// </summary>
        /// <value>
        /// The type of the collection item.
        /// </value>
        Type CollectionItemType { get; set; }

        /// <summary>
        /// Gets or sets the child object meta data.
        /// </summary>
        /// <value>
        /// The child object meta data.
        /// </value>
        ObjectMetaData ChildObjMetaData { get; set; }
        #endregion
    }
}