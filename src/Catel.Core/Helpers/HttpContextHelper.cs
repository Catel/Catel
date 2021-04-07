// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpContextHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE || NETSTANDARD

namespace Catel
{
    using Reflection;
    using System.Reflection;

    /// <summary>
    /// Http context helper class.
    /// </summary>
    internal static class HttpContextHelper
    {
        public static bool HasHttpContext()
        {
            return GetHttpContext() is not null;
        }

        public static object GetHttpContext()
        {
            var httpContextType = TypeCache.GetTypeWithoutAssembly("System.Web.HttpContext", allowInitialization: false);
            if (httpContextType is not null)
            {
                var currentPropertyInfo = httpContextType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
                if (currentPropertyInfo is not null)
                {
                    return currentPropertyInfo.GetValue(null, null);
                }
            }

            return null;
        }

        public static object GetHttpApplicationInstance()
        {
            var httpContext = GetHttpContext();
            if (httpContext is not null)
            {
                var applicationInstanceProperty = httpContext.GetType().GetProperty("ApplicationInstance");
                if (applicationInstanceProperty is not null)
                {
                    var applicationInstance = applicationInstanceProperty.GetValue(httpContext, null);
                    if (applicationInstance is not null)
                    {
                        return applicationInstance;
                    }
                }
            }

            return null;
        }
    }
}

#endif
