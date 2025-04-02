﻿namespace Catel.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Assembly info helper class.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the build date time of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>DateTime.</returns>
        public static DateTime GetBuildDateTime(this Assembly assembly)
        {
            return AssemblyHelper.GetLinkerTimestamp(assembly.Location);
        }

        /// <summary>
        /// Gets the title of a specific assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The title of the assembly.</returns>
        public static string? Title(this Assembly assembly)
        {
            var title = GetAssemblyAttributeValue(assembly, typeof(AssemblyTitleAttribute), "Title");
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }

            return System.IO.Path.GetFileNameWithoutExtension(assembly.Location);
        }

        /// <summary>
        /// Gets the version of a specific assembly with a separator count.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="separatorCount">Number that determines how many version numbers should be returned.</param>
        /// <returns>The version of the assembly.</returns>
        public static string Version(this Assembly assembly, int separatorCount = 3)
        {
            separatorCount++;

            // Get full name, which is in [name], Version=[version], Culture=[culture], PublicKeyToken=[publickeytoken] format
            var assemblyFullName = assembly.FullName ?? assembly.GetName().Name;
            if (string.IsNullOrWhiteSpace(assemblyFullName))
            {
                return "unknown";
            }

            var splittedAssemblyFullName = assemblyFullName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedAssemblyFullName.Length < 2)
            {
                return "unknown";
            }

            string version = splittedAssemblyFullName[1].Replace("Version=", string.Empty).Trim();
            string[] versionSplit = version.Split(new[] { '.' });
            version = versionSplit[0];
            for (int i = 1; i < separatorCount; i++)
            {
                if (i >= versionSplit.Length)
                {
                    break;
                }

                version += string.Format(".{0}", versionSplit[i]);
            }

            return version;
        }

        /// <summary>
        /// Gets the informational version.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The informational version.</returns>
        public static string? InformationalVersion(this Assembly assembly)
        {
            var version = GetAssemblyAttribute<AssemblyInformationalVersionAttribute>(assembly);
            return version is null ? null : version.InformationalVersion;
        }

        /// <summary>
        /// Gets the description of a specific assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The description of the assembly.</returns>
        public static string? Description(this Assembly assembly)
        {
            return GetAssemblyAttributeValue(assembly, typeof(AssemblyDescriptionAttribute), "Description");
        }

        /// <summary>
        /// Gets the product of a specific assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The product of the assembly.</returns>
        public static string? Product(this Assembly assembly)
        {
            return GetAssemblyAttributeValue(assembly, typeof(AssemblyProductAttribute), "Product");
        }

        /// <summary>
        /// Gets the copyright of a specific assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The copyright of the assembly.</returns>
        public static string? Copyright(this Assembly assembly)
        {
            return GetAssemblyAttributeValue(assembly, typeof(AssemblyCopyrightAttribute), "Copyright");
        }

        /// <summary>
        /// Gets the company of a specific assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The company of the assembly.</returns>
        public static string? Company(this Assembly assembly)
        {
            return GetAssemblyAttributeValue(assembly, typeof(AssemblyCompanyAttribute), "Company");
        }

        /// <summary>
        /// Gets the directory of a specific assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The directory of the assembly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
        public static string GetDirectory(this Assembly assembly)
        {
            var location = assembly.Location;
            return location.Substring(0, location.LastIndexOf('\\'));
        }

        /// <summary>
        /// Gets the assembly attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The attribute that the assembly is decorated with or <c>null</c> if the assembly is not decorated with the attribute.</returns>
        private static TAttribute? GetAssemblyAttribute<TAttribute>(Assembly assembly) 
            where TAttribute : Attribute
        {
            var attributes = assembly.GetCustomAttributesEx(typeof(TAttribute));
            return attributes.Length > 0 ? attributes[0] as TAttribute : null;
        }

        /// <summary>
        /// Gets the specific <see cref="Attribute"/> value of the attribute type in the specified assembly.
        /// </summary>
        /// <param name="assembly">Assembly to read the information from.</param>
        /// <param name="attribute">Attribute to read.</param>
        /// <param name="property">Property to read from the attribute.</param>
        /// <returns>Value of the attribute or empty if the attribute is not found.</returns>
        private static string? GetAssemblyAttributeValue(Assembly assembly, Type attribute, string property)
        {
            var attributes = assembly.GetCustomAttributesEx(attribute);
            if (attributes.Length == 0)
            {
                return string.Empty;
            }

            var attributeValue = attributes[0];
            if (attributeValue is null)
            {
                return string.Empty;
            }

            var attributeType = attributeValue.GetType();
            var propertyInfo = attributeType.GetPropertyEx(property);
            if (propertyInfo is null)
            {
                return string.Empty;
            }

            var propertyValue = propertyInfo.GetValue(attributeValue, null);
            if (propertyValue is null)
            {
                return string.Empty;
            }

            return propertyValue.ToString();
        }
    }
}
