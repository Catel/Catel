// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberInfoExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Catel.Caching;
    using IoC;

    /// <summary>
    /// Member info extensions.
    /// </summary>
    public static class MemberInfoExtensions
    {
        private static readonly ICacheStorage<ConstructorInfo, string> _constructorSignatureCache = new CacheStorage<ConstructorInfo, string>();
        private static readonly ICacheStorage<MethodInfo, string> _methodSignatureCache = new CacheStorage<MethodInfo, string>(); 

        #region Methods
        /// <summary>
        /// Gets the signature of a method.
        /// </summary>
        /// <param name="constructorInfo">The member info.</param>
        /// <returns>The signature of the member info.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="constructorInfo"/> is <c>null</c>.</exception>
        public static string GetSignature(this ConstructorInfo constructorInfo)
        {
            Argument.IsNotNull("constructorInfo", constructorInfo);

            return _constructorSignatureCache.GetFromCacheOrFetch(constructorInfo, () =>
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(GetMethodBaseSignaturePrefix(constructorInfo));
                stringBuilder.Append("ctor(");
                var param = constructorInfo.GetParameters().Select(p => string.Format("{0} {1}", p.ParameterType.Name, p.Name)).ToArray();
                stringBuilder.Append(string.Join(", ", param));
                stringBuilder.Append(")");

                return stringBuilder.ToString();
            });
        }

        /// <summary>
        /// Constructor distance tuple.
        /// </summary>
        private class ConstructorDistance
        {
            public ConstructorDistance(int distance, ConstructorInfo constructor)
            {
                Distance = distance;
                Constructor = constructor;
            }

            public int Distance { get; }
            public ConstructorInfo Constructor { get; }
        }


        /// <summary>
        /// Sort constructors
        /// </summary>
        /// <param name="constructors"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<ConstructorInfo> SortByParametersMatchDistance(this List<ConstructorInfo> constructors, object[] parameters)
        {
            List<ConstructorDistance> constructorDistances = new List<ConstructorDistance>();
            foreach (var constructor in constructors)
            {
                int distance;
                if (constructor.TryGetConstructorDistanceByParametersMatch(parameters, out distance))
                {
                    constructorDistances.Add(new ConstructorDistance(distance, constructor));
                }
            }

            return constructorDistances.OrderBy(constructor => constructor.Distance).Select(constructor => constructor.Constructor);
        }

        /// <summary>
        /// Try to get the constructor distance by parameters match.
        /// </summary>
        /// <param name="constructor">The constructor info</param>
        /// <param name="parameters"></param>
        /// <param name="distance">The distance</param>
        /// <returns></returns>
        public static bool TryGetConstructorDistanceByParametersMatch(this ConstructorInfo constructor, object[] parameters, out int distance)
        {
            var constructorParameters = constructor.GetParameters();
            distance = 0;

            int i = 0;
            var match = true;
            while (i < parameters.Length && match)
            {
                int value = 0;
                var parameter = parameters[i];
                var constructorParameter = constructorParameters[i];
                var constructorParameterType = constructorParameter.GetType();

                if (parameter == null && !constructorParameterType.IsClassEx() && !constructorParameterType.IsNullableType())
                {
                    match = false;
                }

                if (parameter != null && !constructorParameter.ParameterType.IsAssignableFromEx(parameter.GetType()))
                {
                    match = false;
                }

                if (match && parameter != null)
                {
                    var parameterType = parameter.GetType();
                    int idx;
                    if (constructorParameter.ParameterType.IsInterfaceEx() && (idx = Array.IndexOf(parameterType.GetInterfacesSorted(), constructorParameter.ParameterType)) > -1)
                    {
                        value = idx + 1;
                    }
                    else
                    {
                        while (parameterType != null && parameterType != constructorParameter.ParameterType)
                        {
                            value++;
                            parameterType = parameterType.GetBaseTypeEx();
                        }

                        if (parameterType == null)
                        {
                            match = false;
                        }
                    }

                    distance += value * value;
                }

                i++;
            }

            return match;
        }


        /// <summary>
        /// Gets the signature of a method.
        /// </summary>
        /// <param name="methodInfo">The member info.</param>
        /// <returns>The signature of the member info.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo"/> is <c>null</c>.</exception>
        public static string GetSignature(this MethodInfo methodInfo)
        {
            Argument.IsNotNull("methodInfo", methodInfo);

            return _methodSignatureCache.GetFromCacheOrFetch(methodInfo, () =>
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(GetMethodBaseSignaturePrefix(methodInfo));
                stringBuilder.Append(methodInfo.ReturnType.Name + " ");

                stringBuilder.Append(methodInfo.Name + "(");
                var param = methodInfo.GetParameters().Select(p => string.Format("{0} {1}", p.ParameterType.Name, p.Name)).ToArray();
                stringBuilder.Append(string.Join(", ", param));
                stringBuilder.Append(")");

                return stringBuilder.ToString();
            });
        }

        private static string GetMethodBaseSignaturePrefix(MethodBase methodBase)
        {
            var stringBuilder = new StringBuilder();

            if (methodBase.IsPrivate)
            {
                stringBuilder.Append("private ");
            }
            else if (methodBase.IsPublic)
            {
                stringBuilder.Append("public ");
            }
            if (methodBase.IsAbstract)
            {
                stringBuilder.Append("abstract ");
            }
            if (methodBase.IsStatic)
            {
                stringBuilder.Append("static ");
            }
            if (methodBase.IsVirtual)
            {
                stringBuilder.Append("virtual ");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns whether property is static.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
#if NETFX_CORE || PCL
            return (propertyInfo.CanRead && propertyInfo.GetMethod.IsStatic) || (propertyInfo.CanWrite && propertyInfo.SetMethod.IsStatic);
#else
            return (propertyInfo.CanRead && propertyInfo.GetGetMethod().IsStatic) || (propertyInfo.CanWrite && propertyInfo.GetSetMethod().IsStatic);
#endif
        }
        #endregion
    }
}