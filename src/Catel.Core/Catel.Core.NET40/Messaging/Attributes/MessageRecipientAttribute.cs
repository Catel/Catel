// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRecipientAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Messaging
{
    using System;

    /// <summary>
    /// Attribute defining a method as recipient for the <see cref="MessageMediator"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class MessageRecipientAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRecipientAttribute"/> class.
        /// </summary>
        public MessageRecipientAttribute()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the tag.
        /// </summary>
        public object Tag { get; set; }
        #endregion
    }
}