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
        /// Sort constructors by parameters match distance.
        /// </summary>
        /// <param name="constructors">The constructors</param>
        /// <param name="parameters">The constructor parameters</param>
        /// <returns>
        /// The constructors sorted by match distance.
        /// </returns>
        public static IEnumerable<ConstructorInfo> SortByParametersMatchDistance(this List<ConstructorInfo> constructors, object[] parameters)
        {
            if (constructors != null && constructors.Count > 1)
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

            return constructors;
        }

        /// <summary>
        /// Try to get the constructor distance by parameters match.
        /// </summary>
        /// <param name="constructor">The constructor info</param>
        /// <param name="parameters"></param>
        /// <param name="distance">The distance</param>
        /// <returns><c>true</c> whether the constructor match with the parameters and distance can be computed; otherwise <c>false</c></returns>
        public static bool TryGetConstructorDistanceByParametersMatch(this ConstructorInfo constructor, object[] parameters, out int distance)
        {
            Argument.IsNotNull("constructor", constructor);

            distance = 0;
            var constructorParameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var constructorParameterType = constructorParameters[i].ParameterType;

                if (parameter is null && !constructorParameterType.IsClassEx() && !constructorParameterType.IsNullableType())
                {
                    return false;
                }

                if (parameter != null && !constructorParameterType.IsAssignableFromEx(parameter.GetType()))
                {
                    return false;
                }

                if (parameter != null)
                {
                    var parameterType = parameter.GetType();
                    int typeDistance = parameterType.GetTypeDistance(constructorParameterType);
                    if (typeDistance == -1)
                    {
                        return false;
                    }

                    distance += typeDistance * typeDistance;
                }
            }

            return true;
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
#if NETFX_CORE
            return (propertyInfo.CanRead && propertyInfo.GetMethod.IsStatic) || (propertyInfo.CanWrite && propertyInfo.SetMethod.IsStatic);
#else
            return (propertyInfo.CanRead && propertyInfo.GetGetMethod().IsStatic) || (propertyInfo.CanWrite && propertyInfo.GetSetMethod().IsStatic);
#endif
        }
        #endregion

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

            /// <summary>
            /// Gets the distance.
            /// </summary>
            public int Distance { get; }

            /// <summary>
            /// Get the constructor.
            /// </summary>
            public ConstructorInfo Constructor { get; }
        }
    }
}
