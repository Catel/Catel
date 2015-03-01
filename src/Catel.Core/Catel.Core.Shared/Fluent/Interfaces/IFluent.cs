// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFluent.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Interface that is used to build fluent interfaces and hides methods declared by <see cref="object" /> from IntelliSense.
    /// </summary>
    /// <nuget id="IFluentInterface" />
    /// <remarks>Code that consumes implementations of this interface should expect one of two things:
    /// <list type="number">
    ///   <item>
    ///     When referencing the interface from within the same solution (project reference), you will still see the methods this interface is meant to hide.
    ///   </item>
    ///   <item>
    ///     When referencing the interface through the compiled output assembly (external reference), the standard Object methods will be hidden as intended.
    ///   </item>
    /// </list>
    /// See http://bit.ly/ifluentinterface for more information.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluent
    {
        #region Methods
        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetType()" /> method from IntelliSense.
        /// </summary>
        /// <returns>The <see cref="Type" />.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetHashCode()" /> method from IntelliSense.
        /// </summary>
        /// <returns>The <see cref="int" />.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        /// Redeclaration that hides the <see cref="object.ToString()" /> method from IntelliSense.
        /// </summary>
        /// <returns>The <see cref="string" />.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary>
        /// Redeclaration that hides the <see cref="object.Equals(object)" /> method from IntelliSense.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);
        #endregion
    }
}