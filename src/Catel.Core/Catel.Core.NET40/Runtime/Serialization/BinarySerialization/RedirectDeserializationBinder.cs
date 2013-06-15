// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectDeserializationBinder.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Logging;
    using Reflection;

    /// <summary>
    /// <see cref="SerializationBinder"/> class that supports backwards compatible serialization.
    /// </summary>
    internal sealed class RedirectDeserializationBinder : SerializationBinder
    {
        #region Fields
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// A dictionary of all <see cref="RedirectTypeAttribute"/> found.
        /// </summary>
        private readonly Dictionary<string, RedirectTypeAttribute> _redirectAttributes = new Dictionary<string, RedirectTypeAttribute>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectDeserializationBinder"/> class.
        /// <para />
        /// Creates a custom binder that redirects all the types to new types if required. All properties
        /// decorated with the <see cref="RedirectTypeAttribute"/> will be redirected.
        /// </summary>
        /// <remarks>
        /// This constructor searches for attributes in a specific application domain.
        /// </remarks>
        public RedirectDeserializationBinder()
        {
            Log.Debug("Creating redirect deserialization binder");

            Initialize();

            Log.Debug("Created redirect deserialization binder");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the binder by searching for all <see cref="RedirectTypeAttribute"/> in the
        /// assemblies passed to this method.
        /// </summary>
        private void Initialize()
        {
            Log.Debug("Initializing redirect deserialization binder");

            var attributeType = typeof(RedirectTypeAttribute);

            foreach (var type in TypeCache.GetTypes())
            {
                InitializeAttributes(type, (RedirectTypeAttribute[])type.GetCustomAttributes(attributeType, true));

                var members = new List<MemberInfo>();

                try
                {
                    members.AddRange(type.GetMembers());
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }

                foreach (var member in members)
                {
                    InitializeAttributes(member, (RedirectTypeAttribute[])member.GetCustomAttributes(attributeType, true));
                }
            }

            Log.Debug("Initialized redirect deserialization binder");
        }

        /// <summary>
        /// Initializes the binder by searching for all <see cref="RedirectTypeAttribute"/> in the
        /// attributes passed to this method.
        /// </summary>
        /// <param name="decoratedObject">object that was decorated with the attribute.</param>
        /// <param name="attributes">Array of attributes to search for.</param>
        private void InitializeAttributes(object decoratedObject, RedirectTypeAttribute[] attributes)
        {
            if (decoratedObject == null)
            {
                return;
            }

            if (attributes.Length == 0)
            {
                return;
            }

            var type = decoratedObject as Type;

            foreach (var attribute in attributes)
            {
                if (type != null)
                {
                    string typeName = TypeHelper.FormatType(type.Assembly.FullName, type.FullName);
                    typeName = TypeHelper.ConvertTypeToVersionIndependentType(typeName);

                    string finalTypeName = TypeHelper.GetTypeName(typeName);
                    string finalAssemblyName = TypeHelper.GetAssemblyName(typeName);

                    attribute.NewTypeName = finalTypeName;
                    attribute.NewAssemblyName = finalAssemblyName;
                }

                if (_redirectAttributes.ContainsKey(attribute.OriginalType))
                {
                    Log.Warning("A redirect for type '{0}' is already added to '{1}'. The redirect to '{2}' will not be added.",
                        attribute.OriginalType, _redirectAttributes[attribute.OriginalType].TypeToLoad, attribute.TypeToLoad);
                }
                else
                {
                    Log.Debug("Adding redirect from '{0}' to '{1}'", attribute.OriginalTypeName, attribute.NewTypeName);

                    _redirectAttributes.Add(attribute.OriginalType, attribute);
                }
            }
        }

        /// <summary>
        /// Binds an assembly and typename to a specific type.
        /// </summary>
        /// <param name="assemblyName">Original assembly name.</param>
        /// <param name="typeName">Original type name.</param>
        /// <returns><see cref="Type"/> that the serialization should actually use.</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            string currentType = TypeHelper.FormatType(assemblyName, typeName);
            string currentTypeVersionIndependent = TypeHelper.ConvertTypeToVersionIndependentType(currentType);
            string newType = ConvertTypeToNewType(currentTypeVersionIndependent);

            Type typeToDeserialize = LoadType(newType) ?? (LoadType(currentTypeVersionIndependent) ?? LoadType(currentType));

            if (typeToDeserialize == null)
            {
                Log.Error("Could not load type '{0}' as '{1}'", currentType, newType);
            }

            return typeToDeserialize;
        }

        /// <summary>
        /// Tries to load a type on a safe way.
        /// </summary>
        /// <param name="type">The type to load.</param>
        /// <returns>The type or <c>null</c> if this method fails.</returns>
        /// <remarks>
        /// In case this method fails to load the type, a warning will be traced with additional information.
        /// </remarks>
        private static Type LoadType(string type)
        {
            Type loadedType = null;

            try
            {
                loadedType = TypeCache.GetType(type);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load type '{0}'", type);
            }

            return loadedType;
        }

        /// <summary>
        /// Converts a string representation of a type to a redirected type.
        /// </summary>
        /// <param name="type">Type to convert.</param>
        /// <returns>String representing the type that represents the redirected type.</returns>
        private string ConvertTypeToNewType(string type)
        {
            const string InnerTypesEnd = ",";

            string newType = type;
            string[] innerTypes = TypeHelper.GetInnerTypes(newType);

            if (innerTypes.Length > 0)
            {
                newType = newType.Replace(string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes)), string.Empty);
                for (int i = 0; i < innerTypes.Length; i++)
                {
                    innerTypes[i] = ConvertTypeToNewType(innerTypes[i]);
                }
            }

            if (_redirectAttributes.ContainsKey(newType))
            {
                newType = _redirectAttributes[newType].TypeToLoad;
            }

            if (innerTypes.Length > 0)
            {
                int innerTypesIndex = newType.IndexOf(InnerTypesEnd);
                if (innerTypesIndex >= 0)
                {
                    newType = newType.Insert(innerTypesIndex, string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes)));
                }
            }

            return newType;
        }
        #endregion
    }
}