// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpContextHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

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
            return GetHttpContext() != null;
        }

        public static object GetHttpContext()
        {
            var httpContextType = TypeCache.GetTypeWithoutAssembly("System.Web.HttpContext");
            if (httpContextType != null)
            {
                var currentPropertyInfo = httpContextType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
                if (currentPropertyInfo != null)
                {
                    return currentPropertyInfo.GetValue(null, null);
                }
            }

            return null;
        }

        public static object GetHttpApplicationInstance()
        {
            var httpContext = GetHttpContext();
            if (httpContext != null)
            {

                var applicationInstanceProperty = httpContext.GetType().GetProperty("ApplicationInstance");
                if (applicationInstanceProperty != null)
                {
                    var applicationInstance = applicationInstanceProperty.GetValue(httpContext, null);
                    if (applicationInstance != null)
                    {
                        
                    }
                }
            }

            return null;
        }
    }
}

#endif