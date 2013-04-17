// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataObjectBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Abstract class that serves as a base class for serializable objects.
    /// </summary>
#if NET
    [Serializable]
#endif
    [ObsoleteEx(Replacement = "ModelBase", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
    public abstract class DataObjectBase : ModelBase, IDataObject
    {
    }

    /// <summary>
    /// Abstract class that serves as a base class for serializable objects.
    /// </summary>
    /// <typeparam name="TDataObject">Type that the class should hold (same as the defined type).</typeparam>
#if NET
    [Serializable]
#endif
    [ObsoleteEx(Replacement = "ModelBase{TModel}", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
    public abstract class DataObjectBase<TDataObject> : ModelBase<TDataObject>, IDataObject<TDataObject>
        where TDataObject : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectBase{TDataObject}"/> class.
        /// </summary>
        protected DataObjectBase()
        {
        }

#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectBase{TDataObject}"/> class.
        /// </summary>
        /// <para />
        /// Only constructor for the DataObjectBase.
        /// <param name="info">SerializationInfo object, null if this is the first time construction.</param>
        /// <param name="context">StreamingContext object, simple pass a default new StreamingContext() if this is the first time construction.</param>
        /// <remarks>
        /// Call this method, even when constructing the object for the first time (thus not deserializing).
        /// </remarks>
        protected DataObjectBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
