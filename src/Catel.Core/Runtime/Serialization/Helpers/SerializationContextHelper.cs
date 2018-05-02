// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContextHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Helper class for serialization.
    /// </summary>
    public static class SerializationContextHelper
    {
        /// <summary>
        /// Gets the name of the serialization reference manager scope.
        /// </summary>
        /// <returns>The name of the scope.</returns>
        public static string GetSerializationReferenceManagerScopeName()
        {
            var scopeName = string.Format("Thread_{0}", ThreadHelper.GetCurrentThreadId());
            return scopeName;
        }
    }
}