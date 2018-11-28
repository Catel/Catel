// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectTypeAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE


namespace Catel.Runtime.Serialization.Binary
{
    using System;
    using Reflection;

    /// <summary>
    /// Attribute that can be used to redirect types to other types to be able to rename / move property types.
    /// </summary>
    /// <remarks>
    /// This attribute should be appended to the property definition.
    /// <para />
    /// In case this attribute is used on a field or property, the <see cref="NewAssemblyName"/> and 
    /// <see cref="NewTypeName"/> are mandatory. In all other cases, the type and assembly will be
    /// loaded automatically.
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class RedirectTypeAttribute : Attribute
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectTypeAttribute"/> class.
        /// </summary>
        /// <param name="originalAssemblyName">Original assembly location..</param>
        /// <param name="originalTypeName">Original type name.</param>
        /// <exception cref="ArgumentException">The <paramref name="originalAssemblyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="originalTypeName"/> is <c>null</c> or whitespace.</exception>
        public RedirectTypeAttribute(string originalAssemblyName, string originalTypeName)
        {
            Argument.IsNotNullOrWhitespace("originalAssemblyName", originalAssemblyName);
            Argument.IsNotNullOrWhitespace("originalTypeName", originalTypeName);

            OriginalAssemblyName = originalAssemblyName;
            OriginalTypeName = originalTypeName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the original assembly name.
        /// </summary>
        public string OriginalAssemblyName { get; private set; }

        /// <summary>
        /// Gets or sets the new assembly name.
        /// </summary>
        /// <remarks>
        /// Leave empty if the assembly name is unchanged.
        /// </remarks>
        public string NewAssemblyName { get; set; }

        /// <summary>
        /// Gets the original type name.
        /// </summary>
        /// <remarks>
        /// List or Array types should be postfixed with a [[]].
        /// </remarks>
        public string OriginalTypeName { get; private set; }

        /// <summary>
        /// Gets or sets the new type name.
        /// </summary>
        /// <remarks>
        /// Leave empty if the type name is unchanged.
        /// <para />
        /// List or Array types should be postfixed with a [[]].
        /// </remarks>
        public string NewTypeName { get; set; }

        /// <summary>
        /// Gets the original type.
        /// </summary>
        public string OriginalType
        {
            get { return TypeHelper.FormatType(OriginalAssemblyName, OriginalTypeName); }
        }

        /// <summary>
        /// Gets the new type that should be loaded.
        /// </summary>
        public string TypeToLoad
        {
            get
            {
                string assemblyToLoad = string.IsNullOrEmpty(NewAssemblyName) ? OriginalAssemblyName : NewAssemblyName;
                string typeToLoad = string.IsNullOrEmpty(NewTypeName) ? OriginalTypeName : NewTypeName;

                return TypeHelper.FormatType(assemblyToLoad, typeToLoad);
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}

#endif
