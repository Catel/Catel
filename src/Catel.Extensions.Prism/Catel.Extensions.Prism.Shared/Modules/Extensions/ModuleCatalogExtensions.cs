// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleCatalogExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

#if PRISM6
    using Prism.Modularity;
#else
    using Microsoft.Practices.Prism.Modularity;
#endif
    using Reflection;

    /// <summary>
    /// Module catalog extensions.
    /// </summary>
    public static class ModuleCatalogExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Determines whether the specified module catalog is the catalog type. Also checks inner catalogs
        /// when the catalog is a <see cref="CompositeModuleCatalog"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <returns><c>true</c> if the module catalog is of the specified type; otherwise, <c>false</c>.</returns>
        public static bool IsCatalogType<T>(this IModuleCatalog moduleCatalog)
        {
            return IsCatalogType(moduleCatalog, typeof(T));
        }

        /// <summary>
        /// Determines whether the specified module catalog is the catalog type. Also checks inner catalogs
        /// when the catalog is a <see cref="CompositeModuleCatalog" />.
        /// </summary>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <param name="typeToCheck">The type to check.</param>
        /// <returns><c>true</c> if the module catalog is of the specified type; otherwise, <c>false</c>.</returns>
        public static bool IsCatalogType(this IModuleCatalog moduleCatalog, Type typeToCheck)
        {
            Argument.IsNotNull(() => moduleCatalog);
            Argument.IsNotNull(() => typeToCheck);

            var moduleCatalogType = moduleCatalog.GetType();

            if (typeToCheck.IsInterfaceEx())
            {
                var implementsInterface = (from iface in moduleCatalogType.GetInterfacesEx()
                                           where string.Equals(iface.FullName, typeToCheck.FullName)
                                           select iface).Any();
                if (implementsInterface)
                {
                    return true;
                }
            }
            else if (typeToCheck.IsAssignableFromEx(moduleCatalogType))
            {
                return true;
            }

            var innerCatalogs = new List<IModuleCatalog>();

            var compositeModuleCatalog = moduleCatalog as CompositeModuleCatalog;
            if (compositeModuleCatalog != null)
            {
                innerCatalogs.AddRange(compositeModuleCatalog.LeafCatalogs);
            }
            else
            {
                if (moduleCatalogType.FullName.Contains("Composite"))
                {
                    try
                    {
                        var leafCatalogs = PropertyHelper.GetPropertyValue(moduleCatalog, "LeafCatalogs", false);
                        foreach (var leafCatalog in (IEnumerable)leafCatalogs)
                        {
                            var finalLeafCatalog = leafCatalog as IModuleCatalog;
                            if (finalLeafCatalog != null)
                            {
                                innerCatalogs.Add(finalLeafCatalog);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to get LeafCatalogs from the (expected) composite catalog '{0}'", moduleCatalogType.GetSafeFullName());
                    }
                }
            }

            foreach (var innerModuleCatalog in innerCatalogs)
            {
                if (innerModuleCatalog.IsCatalogType(typeToCheck))
                {
                    return true;
                }
            }

            return false;
        }
    }
}