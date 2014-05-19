// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationContextInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Linq;
    using Catel.Data;
    using Catel.IoC;

    /// <summary>
    /// Class containing all information about the binary serialization context.
    /// </summary>
    public class XmlSerializationContextInfo
    {
        private readonly object _lockObject = new object();
        //private DataContractSerializer _dataContractSerializer;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element" /> is <c>null</c>.</exception>
        public XmlSerializationContextInfo(XElement element, ModelBase model)
        {
            Argument.IsNotNull("element", element);
            Argument.IsNotNull("model", model);

            Element = element;
            Model = model;
        }
        #endregion

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value>The element.</value>
        public XElement Element { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ModelBase Model { get; private set; }

        ///// <summary>
        ///// Gets the data contract serializer.
        ///// </summary>
        ///// <returns>DataContractSerializer.</returns>
        //public DataContractSerializer GetDataContractSerializer()
        //{
        //    lock (_lockObject)
        //    {
        //        if (_dataContractSerializer == null)
        //        {
        //            var modelType = Model.GetType();

        //            //var serializer = dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToDeserialize, xmlName);
        //            //_dataContractSerializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, modelType);
        //            //_dataContractSerializer.DataContractSurrogate
                        
        //        }
        //    }

        //    return _dataContractSerializer;
        //}
    }
}