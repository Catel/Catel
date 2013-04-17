// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberInfoExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Member info extensions.
    /// </summary>
    public static class MemberInfoExtensions
    {
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

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(GetMethodBaseSignaturePrefix(constructorInfo));
            stringBuilder.Append("ctor(");
            var param = constructorInfo.GetParameters().Select(p => String.Format("{0} {1}", p.ParameterType.Name, p.Name)).ToArray();
            stringBuilder.Append(String.Join(", ", param));
            stringBuilder.Append(")");

            return stringBuilder.ToString();
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

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(GetMethodBaseSignaturePrefix(methodInfo));
            stringBuilder.Append(methodInfo.ReturnType.Name + " ");

            stringBuilder.Append(methodInfo.Name + "(");
            var param = methodInfo.GetParameters().Select(p => String.Format("{0} {1}", p.ParameterType.Name, p.Name)).ToArray();
            stringBuilder.Append(String.Join(", ", param));
            stringBuilder.Append(")");

            return stringBuilder.ToString();
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
        #endregion
    }
}