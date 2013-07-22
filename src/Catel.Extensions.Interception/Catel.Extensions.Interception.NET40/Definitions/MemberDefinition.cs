// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberDefinition.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class implements the type member definition interface.
    /// </summary>
    public class MemberDefinition : IMemberDefinition
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberDefinition" /> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentException">The <paramref name="memberName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameters"/> is <c>null</c>.</exception>
        public MemberDefinition(string memberName, IList<Type> parameters)
        {
            Argument.IsNotNullOrWhitespace("memberName", memberName);
            Argument.IsNotNull("parameters", parameters);

            Parameters = parameters; // TODO: Must consider kind of parameter: value, reference, or output
            MemberName = memberName;
        }
        #endregion

        #region IMemberDefinition Members
        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        /// <value>
        ///     The parameters.
        /// </value>
        public IList<Type> Parameters { get; private set; }

        /// <summary>
        ///     Gets the name of the member.
        /// </summary>
        /// <value>
        ///     The name of the member.
        /// </value>
        public string MemberName { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var member = obj as MemberDefinition;

            if (member == null)
            {
                return false;
            }

            if (Parameters.Count != member.Parameters.Count)
            {
                return false;
            }

            if (Parameters.Where((parameter, index) => !parameter.IsGenericParameter && parameter != member.Parameters[index]).Any())
            {
                return false;
            }

            return MemberName == member.MemberName;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hash = MemberName.GetHashCode();
            return Parameters.Aggregate(hash, (a, b) => a ^ b.GetHashCode());
        }
        #endregion
    }
}