// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSchemaHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Catel.IoC;
    using Data;
    using Logging;
    using Reflection;

    /// <summary>
    /// Helper class for xml schemas.
    /// </summary>
    public static class XmlSchemaHelper
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        public static ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default catel xml namespace.
        /// </summary>
        public const string DefaultNs = "http://schemas.datacontract.org/2004/07/";

        /// <summary>
        /// Default xml schema.
        /// </summary>
        public const string Xmlns = "http://www.w3.org/2001/XMLSchema";
        #endregion

        #region Methods
        /// <summary>
        /// Gets the XML schema.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="generateFlatSchema">A value indicating whether the schema should be generated as flat schema.</param>
        /// <returns>The qualified name of the type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="schemaSet"/> is <c>null</c>.</exception>
        public static XmlQualifiedName GetXmlSchema(Type type, XmlSchemaSet schemaSet, bool generateFlatSchema)
        {
            Argument.IsNotNull("schemaSet", schemaSet);

            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var serializationManager = dependencyResolver.Resolve<ISerializationManager>();

            string typeNs = GetTypeNamespaceForSchema(type);
            var schema = GetOrCreateSchema(typeNs, schemaSet, serializationManager);

            // Check if it already exists
            string typeName = GetTypeNameForSchema(type);
            var existingType = (from x in schema.Items.Cast<object>()
                                where x is XmlSchemaComplexType && ((XmlSchemaComplexType)x).Name == typeName
                                select (XmlSchemaComplexType)x).FirstOrDefault();
            if (existingType != null)
            {
                return new XmlQualifiedName(existingType.Name, typeNs);
            }

            var typeSchema = CreateSchemaComplexType(type, schema, schemaSet, serializationManager, generateFlatSchema);

            var root = new XmlSchemaElement();
            root.Name = string.Format("{0}", typeSchema.Name);
            root.SchemaTypeName = new XmlQualifiedName(typeSchema.Name, typeNs);
            root.IsNillable = true;
            schema.Items.Add(root);

            return new XmlQualifiedName(typeSchema.Name, typeNs);
        }

        /// <summary>
        /// Adds the type to schema set by reading the <see cref="XmlSchemaProviderAttribute" /> or by
        /// using the known schema sets information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <returns>The xml qualified name.</returns>
        private static XmlQualifiedName AddTypeToSchemaSet(Type type, XmlSchemaSet schemaSet, ISerializationManager serializationManager)
        {
            var attribute = (from x in type.GetCustomAttributesEx(typeof(XmlSchemaProviderAttribute), false)
                             select x as XmlSchemaProviderAttribute).FirstOrDefault();
            if (attribute == null)
            {
                if (IsBasicType(type))
                {
                    return new XmlQualifiedName(type.Name.ToLower(), Xmlns);
                }

                return null;
            }

            var methodToInvoke = type.GetMethodEx(attribute.MethodName, BindingFlags.Public | BindingFlags.Static);
            if (methodToInvoke == null)
            {
                Log.Error("Expected method '{0}.{1}' because of the XmlSchemaProvider attribute, but it was not found", type.FullName, attribute.MethodName);
                return null;
            }

            var qualifiedName = (XmlQualifiedName)methodToInvoke.Invoke(null, new object[] { schemaSet });
            return qualifiedName;
        }

        /// <summary>
        /// Returns the sequence of properties of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <returns>Sequence containing all properties.</returns>
        private static XmlSchemaSequence GetPropertiesSequence(Type type, XmlSchema schema, XmlSchemaSet schemaSet, ISerializationManager serializationManager)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("schema", schema);
            Argument.IsNotNull("schemaSet", schemaSet);

            var propertiesSequence = new XmlSchemaSequence();

            if (typeof(ModelBase).IsAssignableFromEx(type))
            {
                var typeNs = GetTypeNamespaceForSchema(type);

                var members = new List<MemberInfo>();
                members.AddRange(from field in serializationManager.GetFieldsToSerialize(type)
                                 select type.GetFieldEx(field));
                members.AddRange(from property in serializationManager.GetPropertiesToSerialize(type)
                                 select type.GetPropertyEx(property));

                foreach (var member in members)
                {
                    var propertySchemaElement = new XmlSchemaElement();
                    propertySchemaElement.Name = member.Name;

                    var memberType = typeof(object);
                    var fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        memberType = fieldInfo.FieldType;
                    }

                    var propertyInfo = member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        memberType = propertyInfo.PropertyType;
                    }

                    if (memberType.ImplementsInterfaceEx(typeof(IEnumerable)) && memberType != typeof(string))
                    {
                        propertySchemaElement.SchemaTypeName = new XmlQualifiedName(string.Format("{0}", member.Name), typeNs);

                        var collectionPropertyType = new XmlSchemaComplexType();
                        collectionPropertyType.Name = string.Format("{0}", member.Name);
                        schema.Items.Add(collectionPropertyType);

                        foreach (var genericArgument in memberType.GetGenericArguments())
                        {
                            AddTypeToSchemaSet(genericArgument, schemaSet, serializationManager);
                        }
                    }
                    else
                    {
                        propertySchemaElement.SchemaTypeName = AddTypeToSchemaSet(memberType, schemaSet, serializationManager);
                        propertySchemaElement.IsNillable = IsNullableType(memberType);
                        propertySchemaElement.MinOccurs = 0;
                    }

                    propertiesSequence.Items.Add(propertySchemaElement);
                }
            }

            return propertiesSequence;
        }

        /// <summary>
        /// Creates the an xml schema for a complex type. This method automatically takes care of
        /// any base classes that must be added.
        /// <para />
        /// This method already adds the <see cref="XmlSchemaComplexType" /> to the <see cref="XmlSchemaSet" />.
        /// </summary>
        /// <param name="type">The type to create the complex schema for.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="generateFlatSchema">A value indicating whether the schema should be generated as flat schema.</param>
        /// <returns>The complex schema for the specified type.</returns>
        private static XmlSchemaComplexType CreateSchemaComplexType(Type type, XmlSchema schema, XmlSchemaSet schemaSet, ISerializationManager serializationManager, bool generateFlatSchema)
        {
            // Determine name, which is complex in generic types
            string typeName = GetTypeNameForSchema(type);

            // First, add the type, otherwise we might get into a stackoverflow when using generic base types
            // <xs:complexType>
            var modelBaseType = new XmlSchemaComplexType();
            modelBaseType.Name = typeName;
            modelBaseType.IsMixed = false;

            schema.Items.Add(modelBaseType);

            var propertiesSequence = GetPropertiesSequence(type, schema, schemaSet, serializationManager);

            // If flat, don't generate base classes, just the type itself
            if (generateFlatSchema)
            {
                modelBaseType.Particle = propertiesSequence;
                return modelBaseType;
            }

            if (type.IsGenericType)
            {
                var genericComplexType = new XmlSchemaComplexType();
                genericComplexType.Name = typeName;

                // <xs:annotation>
                var annotation = new XmlSchemaAnnotation();

                // <xs:appinfo>
                var appInfo = new XmlSchemaAppInfo();

                //<GenericType xmlns="http://schemas.microsoft.com/2003/10/Serialization/" Name="DataContractBaseOf{0}{#}" Namespace="http://schemas.datacontract.org/2004/07/STC">
                //  <GenericParameter Name="ProfileDateRange2" Namespace="http://schemas.datacontract.org/2004/07/STC.Meter.ProfileData"/>
                //</GenericType>
                var genericTypeElement = new XElement("GenericType");
                genericTypeElement.Add(new XAttribute("Name", string.Format("{0}Of{{0}}{{#}}", typeName)));
                genericTypeElement.Add(new XAttribute("Namespace", GetTypeNamespaceForSchema(type)));

                foreach (var genericArgument in type.GetGenericArgumentsEx())
                {
                    var genericArgumentQualifiedName = AddTypeToSchemaSet(genericArgument, schemaSet, serializationManager);
                    var genericArgumentElement = new XElement("GenericParameter");
                    genericArgumentElement.Add(new XAttribute("Name", genericArgumentQualifiedName.Name));
                    genericArgumentElement.Add(new XAttribute("Namespace", genericArgumentQualifiedName.Namespace));
                    genericTypeElement.Add(genericArgumentElement);
                }

                var conversionDoc = new XmlDocument();
                appInfo.Markup = new XmlNode[] { conversionDoc.CreateTextNode(genericTypeElement.ToString()) };

                annotation.Items.Add(appInfo);
            }

            var baseTypeQualifiedName = AddTypeToSchemaSet(type.BaseType, schemaSet, serializationManager);
            if (baseTypeQualifiedName != null)
            {
                // <xs:extensions base="address">
                var complexContentExtension = new XmlSchemaComplexContentExtension();
                complexContentExtension.BaseTypeName = baseTypeQualifiedName;
                complexContentExtension.Particle = propertiesSequence;

                // <xs:complexContent>
                var complexContent = new XmlSchemaComplexContent();
                complexContent.Content = complexContentExtension;

                modelBaseType.ContentModel = complexContent;
            }

            return modelBaseType;
        }

        /// <summary>
        /// Gets the or create schema from the schema set.
        /// <para />
        /// If the namespace does not yet exists, it is created and added. Otherwise the existing
        /// one is returned.
        /// </summary>
        /// <param name="xmlns">The xml namespace.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <returns>The xml schema to use.</returns>
        private static XmlSchema GetOrCreateSchema(string xmlns, XmlSchemaSet schemaSet, ISerializationManager serializationManager)
        {
            var schemas = schemaSet.Schemas(xmlns);

            foreach (XmlSchema schema in schemas)
            {
                return schema;
            }

            var newSchema = new XmlSchema();
            newSchema.TargetNamespace = xmlns;
            newSchema.ElementFormDefault = XmlSchemaForm.Qualified;

            schemaSet.Add(newSchema);

            return newSchema;
        }

        /// <summary>
        /// Gets the name of the type to be used inside an xml schema.
        /// </summary>
        /// <param name="type">The type to determine the name for.</param>
        /// <returns>The name.</returns>
        private static string GetTypeNameForSchema(Type type)
        {
            string typeName = type.Name;

            if (type.IsGenericType)
            {
                int firstPosition = typeName.IndexOf("`");
                typeName = typeName.Substring(0, firstPosition);
                typeName = string.Format("{0}Of{1}", typeName, type.GetGenericArguments()[0].Name);
            }

            return typeName;
        }

        /// <summary>
        /// Gets the type namespace to be used inside an xml schema.
        /// </summary>
        /// <param name="type">The type to determine the namespace for.</param>
        /// <returns>The namespace.</returns>
        private static string GetTypeNamespaceForSchema(Type type)
        {
            return string.Format("{0}{1}", DefaultNs, type.Namespace);
        }

        /// <summary>
        /// Determines whether the specified type is a basic type. A basic type is one that can be wholly expressed
        /// as an XML attribute. All primitive data types and <see cref="String"/> and <see cref="DateTime"/> are basic types.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is a basic type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static bool IsBasicType(Type type)
        {
            Argument.IsNotNull("type", type);

            if (type == typeof(string) || type.IsPrimitive || type.IsEnum || type == typeof(DateTime) || type == typeof(decimal) || type == typeof(Guid))
            {
                return true;
            }

            Type nullableValueType;
            if (IsNullable(type, out nullableValueType))
            {
                return IsBasicType(nullableValueType);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified type is a nullable type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is a nullable; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static bool IsNullableType(Type type)
        {
            Argument.IsNotNull("type", type);

            if (!IsBasicType(type))
            {
                return true;
            }

            var typeToCheck = type;
            if (!IsNullable(type, out typeToCheck))
            {
                typeToCheck = type;
            }

            if (typeToCheck == typeof(string))
            {
                return true;
            }

            if (typeToCheck == typeof(Guid))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="valueType">The value type of the corresponding nullable type.</param>
        /// <returns><c>true</c> if the specified type is nullable; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="valueType"/> is <c>null</c>.</exception>
        public static bool IsNullable(Type type, out Type valueType)
        {
            Argument.IsNotNull("type", type);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                valueType = type.GetGenericArguments()[0];
                return true;
            }

            valueType = null;
            return false;
        }
        #endregion
    }
}