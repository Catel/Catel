// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeBindingRedirect.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Logging;
    using Reflection;

    /// <summary>
    /// Automatically tries to resolve different versions of already loaded assemblies (runtime binding redirects).
    /// </summary>
    public class RuntimeBindingRedirect
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly AppDomain _appDomain;
        private readonly HashSet<string> _attemptedLoads = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBindingRedirect"/> class.
        /// </summary>
        public RuntimeBindingRedirect()
            : this(AppDomain.CurrentDomain)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBindingRedirect"/> class.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        public RuntimeBindingRedirect(AppDomain appDomain)
        {
            Argument.IsNotNull(() => appDomain);

            _appDomain = appDomain;
            _appDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name;

            if (!_attemptedLoads.Contains(assemblyName))
            {
                _attemptedLoads.Add(assemblyName);

                var simpleAssemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(assemblyName);

                Log.Debug("Checking if there is a different version loaded for '{0}' using simple assembly name '{1}'", assemblyName, simpleAssemblyName);

                var possibleAssembly = (from assembly in _appDomain.GetLoadedAssemblies()
                                        where assembly.FullName.StartsWith(simpleAssemblyName)
                                        select assembly).FirstOrDefault();
                if (possibleAssembly != null)
                {
                    Log.Debug("Found assembly '{0}'", possibleAssembly.FullName);
                    return possibleAssembly;
                }
            }

            return null;
        }
    }
}

#endif
