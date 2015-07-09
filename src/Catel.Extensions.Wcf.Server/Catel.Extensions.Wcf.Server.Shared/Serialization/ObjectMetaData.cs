// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectMetaData.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
    [ObsoleteEx(Message = "We are considering to remove Wcf.Server support. See https://catelproject.atlassian.net/browse/CTL-672",
    TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public class ObjectMetaData : IObjectMetaData
    {
        #region IObjectMetaData Members
        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Gets or sets the properties meta data.
        /// </summary>
        /// <value>
        /// The properties meta data.
        /// </value>
        public IList<IObjectMetaData> PropertiesMetaData { get; set; }

        /// <summary>
        /// Gets or sets the type of the collection item.
        /// </summary>
        /// <value>
        /// The type of the collection item.
        /// </value>
        public Type CollectionItemType { get; set; }

        /// <summary>
        /// Gets or sets the child object meta data.
        /// </summary>
        /// <value>
        /// The child object meta data.
        /// </value>
        public ObjectMetaData ChildObjMetaData { get; set; }
        #endregion
    }
}