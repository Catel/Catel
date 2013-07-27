// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataObjectBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
    /// IModelBase that the <see cref="ModelBase{TModel}"/> must implement to easily pass objects to methods as non-generic.
    /// </summary>
    /// <remarks>
    /// This interface defines all the non-generic interfaces that the <see cref="ModelBase"/> class implements.
    /// </remarks>
    public interface IModel : INotifyPropertyChanging, INotifyPropertyChanged, IParent, INotifyDataErrorInfo, INotifyDataWarningInfo
                                        , IDataErrorInfo, IDataWarningInfo, IAdvancedEditableObject, IXmlSerializable
#if NET
                                        , ICloneable, ISerializable
#endif
    {
        #region Properties
        /// <summary>
        /// Gets the name of the object. By default, this is the name of the inherited class.
        /// </summary>
        /// <value>The name of the key.</value>
        string KeyName { get; }

        /// <summary>
        /// Gets a value indicating whether this object is dirty.
        /// </summary>
        /// <value><c>true</c> if this object is dirty; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

        /// <summary>
        /// Gets the validation context which contains all information about the validation.
        /// </summary>
        /// <value>The validation context.</value>
        IValidationContext ValidationContext { get; }

        /// <summary>
        /// Gets a value indicating whether the object is currently hiding its validation results. If the object
        /// hides its validation results, it is still possible to retrieve the validation results using the
        /// <see cref="ValidationContext"/>.
        /// </summary>
        bool IsHidingValidationResults { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the default value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        object GetDefaultValue(string name);

        /// <summary>
        /// Returns the default value of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        object GetDefaultValue(PropertyData property);

        /// <summary>
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        TValue GetDefaultValue<TValue>(string name);

        /// <summary>
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        TValue GetDefaultValue<TValue>(PropertyData property);

        /// <summary>
        /// Returns the type of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Type of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        Type GetPropertyType(string name);

        /// <summary>
        /// Returns the type of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Type of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        Type GetPropertyType(PropertyData property);

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">if set to <c>true</c>, a validation is forced. When the validation is not forced, it means 
        /// that when the object is already validated, and no properties have been changed, no validation actually occurs 
        /// since there is no reason for any values to have changed.
        /// </param>
        /// <remarks>
        /// To check wether this object contains any errors, use the <see cref="INotifyDataErrorInfo.HasErrors"/> property.
        /// </remarks>
        void Validate(bool force = false);
        #endregion
    }
}