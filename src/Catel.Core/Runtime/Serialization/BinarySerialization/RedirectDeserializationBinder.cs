// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectDeserializationBinder.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Runtime.Serialization.Binary
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Caching;
    using Logging;
    using Reflection;

    /// <summary>
    /// <see cref="SerializationBinder"/> class that supports backwards compatible serialization.
    /// </summary>
    public sealed class RedirectDeserializationBinder : SerializationBinder
    {
        #region Fields
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly CacheStorage<Type, bool> TypeBinarySerializableCache = new CacheStorage<Type, bool>();

        /// <summary>
        /// A dictionary of all <see cref="RedirectTypeAttribute"/> found.
        /// </summary>
        private readonly Dictionary<string, RedirectTypeAttribute> _redirectAttributes = new Dictionary<string, RedirectTypeAttribute>();

        /// <summary>
        /// The number of types per thread to initialize. If <c>-1</c>, the types will be initialized in a single thread.
        /// </summary>
        private readonly int _typesPerThread;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectDeserializationBinder" /> class.
        /// <para />
        /// Creates a custom binder that redirects all the types to new types if required. All properties
        /// decorated with the <see cref="RedirectTypeAttribute" /> will be redirected.
        /// </summary>
        /// <param name="typesPerThread">The number of types per thread to initialize. If <c>-1</c>, the types will be initialized in a single thread.</param>
        /// <remarks>This constructor searches for attributes in a specific application domain.</remarks>
        public RedirectDeserializationBinder(int typesPerThread = 2500)
        {
            _typesPerThread = typesPerThread;

            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the binder by searching for all <see cref="RedirectTypeAttribute"/> in the
        /// assemblies passed to this method.
        /// </summary>
        private void Initialize()
        {
            var allTypes = new List<Type>(TypeCache.GetTypes());
            var attributeType = typeof(RedirectTypeAttribute);

            ParallelHelper.ExecuteInParallel(allTypes, type =>
            {
                if (!IsTypeBinarySerializable(type))
                {
                    return;
                }

                var typeRedirectAttributes = (RedirectTypeAttribute[])type.GetCustomAttributes(attributeType, true);
                if (typeRedirectAttributes.Length > 0)
                {
                    InitializeAttributes(type, typeRedirectAttributes);
                }

                var members = new List<MemberInfo>();

                try
                {
                    var typeMembers = type.GetMembers();
                    members.AddRange(typeMembers);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }

                foreach (var member in members)
                {
                    var memberRedirectAttributes = (RedirectTypeAttribute[])member.GetCustomAttributes(attributeType, true);
                    if (memberRedirectAttributes.Length > 0)
                    {
                        InitializeAttributes(member, memberRedirectAttributes);
                    }
                }
            }, _typesPerThread, "Initialize redirect deserialization binder");
        }

        private bool IsTypeBinarySerializable(Type type)
        {
            if (type is null)
            {
                return false;
            }

            return TypeBinarySerializableCache.GetFromCacheOrFetch(type, () => type.GetConstructor(TypeArray.From<SerializationInfo, StreamingContext>()) != null);
        }

        /// <summary>
        /// Initializes the binder by searching for all <see cref="RedirectTypeAttribute"/> in the
        /// attributes passed to this method.
        /// </summary>
        /// <param name="decoratedObject">object that was decorated with the attribute.</param>
        /// <param name="attributes">Array of attributes to search for.</param>
        private void InitializeAttributes(object decoratedObject, RedirectTypeAttribute[] attributes)
        {
            if (decoratedObject is null)
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
                    var typeName = TypeHelper.FormatType(type.Assembly.FullName, type.FullName);
                    typeName = TypeHelper.ConvertTypeToVersionIndependentType(typeName);

                    var finalTypeName = TypeHelper.GetTypeName(typeName);
                    var finalAssemblyName = TypeHelper.GetAssemblyName(typeName);

                    attribute.NewTypeName = finalTypeName;
                    attribute.NewAssemblyName = finalAssemblyName;
                }

                if (_redirectAttributes.ContainsKey(attribute.OriginalType))
                {
                    Log.Warning("A redirect for type '{0}' is already added to '{1}'. The redirect to '{2}' will not be added",
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

            var typeToDeserialize = LoadType(newType) ?? (LoadType(currentTypeVersionIndependent) ?? LoadType(currentType));

            if (typeToDeserialize is null)
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
                newType = newType.Replace(string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes, false)), string.Empty);
                for (int i = 0; i < innerTypes.Length; i++)
                {
                    innerTypes[i] = ConvertTypeToNewType(innerTypes[i]);
                }
            }

            if (_redirectAttributes.TryGetValue(newType, out var redirectAttribute))
            {
                newType = redirectAttribute.TypeToLoad;
            }

            if (innerTypes.Length > 0)
            {
                int innerTypesIndex = newType.IndexOf(InnerTypesEnd);
                if (innerTypesIndex >= 0)
                {
                    newType = newType.Insert(innerTypesIndex, string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes, false)));
                }
            }

            return newType;
        }
        #endregion
    }
}

#endif
