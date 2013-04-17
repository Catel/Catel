// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataObjectBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

#if NET
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// IDataObjectBase that the <see cref="DataObjectBase{TDataObject}"/> must implement to easily pass objects to methods as non-generic.
    /// </summary>
    /// <remarks>
    /// This interface defines all the non-generic interfaces that the <see cref="DataObjectBase{TDataObject}"/> class implements.
    /// </remarks>
    [ObsoleteEx(Replacement = "IModel{TModel}", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
    public interface IDataObject : IModel
    {
    }

    /// <summary>
    /// IDataObjectBase that the <see cref="DataObjectBase{TDataObject}"/> must implement to easily mock objects.
    /// </summary>
    /// <typeparam name="TModel">Type that the class should hold (same as the defined type).</typeparam>
    /// <remarks>
    /// This interface defines all the generic interfaces that the <see cref="DataObjectBase{TDataObject}"/> class implements.
    /// </remarks>
    [ObsoleteEx(Replacement = "IModel{TModel}", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
    public interface IDataObject<TModel>
        where TModel : class
    {
    }
}