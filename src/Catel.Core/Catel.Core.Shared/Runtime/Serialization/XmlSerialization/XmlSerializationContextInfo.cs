// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationContextInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;
    using Catel.Data;
    using Catel.IoC;

    /// <summary>
    /// Class containing all information about the binary serialization context.
    /// </summary>
    public class XmlSerializationContextInfo : ISerializationContextInfo
    {
        private readonly object _lockObject = new object();
        //private DataContractSerializer _dataContractSerializer;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="model">The model, is allowed to be null for value types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element" /> is <c>null</c>.</exception>
        public XmlSerializationContextInfo(XElement element, object model)
        {
            Argument.IsNotNull("element", element);

            Initialize(element, model);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="xmlReader">The XML reader.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="xmlReader" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        public XmlSerializationContextInfo(XmlReader xmlReader, ModelBase model)
        {
            Argument.IsNotNull("xmlReader", xmlReader);
            Argument.IsNotNull("model", model);

            var modelType = model.GetType();
            var elementStart = string.Format("<{0}", modelType.Name);

            if (xmlReader.HasAttributes)
            {
                for (int i = 0; i < xmlReader.AttributeCount; i++)
                {
                    xmlReader.MoveToAttribute(i);

                    var attributeName = xmlReader.LocalName;
                    var attributeValue = xmlReader.Value;

                    elementStart += string.Format(" {0}=\"{1}\"", attributeName, attributeValue);
                }

                xmlReader.MoveToElement();
            }

            elementStart += ">";

            xmlReader.MoveToContent();

            var xmlContent = xmlReader.ReadInnerXml();
            if (xmlContent.StartsWith("&lt;"))
            {
#if SL5
                xmlContent = System.Windows.Browser.HttpUtility.HtmlDecode(xmlContent);
#else
                xmlContent = System.Net.WebUtility.HtmlDecode(xmlContent);
#endif
            }

            var elementEnd = string.Format("</{0}>", modelType.Name);

            var finalXmlContent = string.Format("{0}{1}{2}", elementStart, xmlContent, elementEnd);
            Initialize(finalXmlContent, model);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationContextInfo"/> class.
        /// </summary>
        /// <param name="xmlContent">Content of the XML.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="xmlContent" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        public XmlSerializationContextInfo(string xmlContent, ModelBase model)
        {
            Argument.IsNotNull("xmlContent", xmlContent);
            Argument.IsNotNull("model", model);

            Initialize(xmlContent, model);
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
        public object Model { get; private set; }

        #region Methods
        private void Initialize(string xmlContent, object model)
        {
            Initialize(XElement.Parse(xmlContent), model);
        }

        private void Initialize(XElement element, object model)
        {
            Element = element;
            Model = model;
        }
        #endregion
    }
}