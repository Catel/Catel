// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRecipientAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRecipientAttribute"/> class.
        /// </summary>
        public MessageRecipientAttribute()
        {
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        public object Tag { get; set; }
    }
}