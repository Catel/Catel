// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignTimeCodeAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Attribute to support code at design time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DesignTimeCodeAttribute : Attribute
    {
        private static readonly Dictionary<Type, bool> InitializedTypes = new Dictionary<Type, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignTimeCodeAttribute"/> class.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        public DesignTimeCodeAttribute(Type typeToConstruct)
        {
            if (InitializedTypes.ContainsKey(typeToConstruct))
            {
                return;
            }

            InitializedTypes[typeToConstruct] = true;

            if (CatelEnvironment.IsInDesignMode)
            {
                Activator.CreateInstance(typeToConstruct);
            }
        }
    }
}