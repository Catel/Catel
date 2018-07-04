// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectAdapter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Reflection;
    using Data;
    using Logging;
    using Reflection;

    /// <summary>
    /// Adapter to interact with objects.
    /// </summary>
    public class ObjectAdapter : IObjectAdapter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="modelInfo">The model information.</param>
        /// <returns>MemberValue.</returns>
        public virtual MemberValue GetMemberValue(object model, string memberName, SerializationModelInfo modelInfo)
        {
            var modelType = model.GetType();

            try
            {
                var modelEditor = model as IModelEditor;
                if (modelEditor != null && modelInfo.CatelPropertyNames.Contains(memberName))
                {
                    var memberMetadata = modelInfo.CatelPropertiesByName[memberName];
                    var actualPropertyValue = modelEditor.GetValueFastButUnsecure(memberName);

                    var propertyValue = new MemberValue(SerializationMemberGroup.CatelProperty, modelType, memberMetadata.MemberType, 
                        memberMetadata.MemberName, memberMetadata.MemberNameForSerialization, actualPropertyValue);
                    return propertyValue;
                }

                if (modelInfo.PropertiesByName.TryGetValue(memberName, out var propertyMemberMetadata))
                {
                    object value = null;
                    var get = false;

                    var propertySerializable = model as IPropertySerializable;
                    if (propertySerializable != null)
                    {
                        get = propertySerializable.GetPropertyValue(memberName, ref value);
                    }

                    if (!get)
                    {
                        value = ((PropertyInfo)propertyMemberMetadata.Tag).GetValue(model, null);
                    }

                    var propertyValue = new MemberValue(SerializationMemberGroup.RegularProperty, modelType, propertyMemberMetadata.MemberType, 
                        propertyMemberMetadata.MemberName, propertyMemberMetadata.MemberNameForSerialization, value);
                    return propertyValue;
                }

                if (modelInfo.FieldsByName.TryGetValue(memberName, out var fieldMemberMetadata))
                {
                    object value = null;
                    var get = false;

                    var fieldSerializable = model as IFieldSerializable;
                    if (fieldSerializable != null)
                    {
                        get = fieldSerializable.GetFieldValue(memberName, ref value);
                    }

                    if (!get)
                    {
                        value = ((FieldInfo)fieldMemberMetadata.Tag).GetValue(model);
                    }

                    var fieldValue = new MemberValue(SerializationMemberGroup.Field, modelType, fieldMemberMetadata.MemberType,
                        fieldMemberMetadata.MemberName, fieldMemberMetadata.MemberNameForSerialization, value);
                    return fieldValue;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to get value of member '{0}.{1}', skipping item during serialization", modelType.GetSafeFullName(false), memberName);
            }

            return null;
        }

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="member">The member.</param>
        /// <param name="modelInfo">The model information.</param>
        public virtual void SetMemberValue(object model, MemberValue member, SerializationModelInfo modelInfo)
        {
            var modelType = model.GetType();

            try
            {
                var modelEditor = model as IModelEditor;
                if (modelEditor != null && modelInfo.CatelPropertyNames.Contains(member.Name))
                {
                    modelEditor.SetValueFastButUnsecure(member.Name, member.Value);
                }
                else if (modelInfo.PropertyNames.Contains(member.Name))
                {
                    var set = false;

                    var propertySerializable = model as IPropertySerializable;
                    if (propertySerializable != null)
                    {
                        set = propertySerializable.SetPropertyValue(member.Name, member.Value);
                    }

                    if (!set)
                    {
                        var memberMetadata = modelInfo.PropertiesByName[member.Name];
                        if (memberMetadata != null)
                        {
                            ((PropertyInfo)memberMetadata.Tag).SetValue(model, member.Value, null);
                            set = true;
                        }
                    }

                    if (!set)
                    {
                        Log.Warning("Failed to set property '{0}.{1}' because the member cannot be found on the model", modelType.GetSafeFullName(false), member.Name);
                    }
                }
                else if (modelInfo.FieldNames.Contains(member.Name))
                {
                    var set = false;

                    var fieldSerializable = model as IFieldSerializable;
                    if (fieldSerializable != null)
                    {
                        set = fieldSerializable.SetFieldValue(member.Name, member.Value);
                    }

                    if (!set)
                    {
                        var memberMetadata = modelInfo.FieldsByName[member.Name];
                        if (memberMetadata != null)
                        {
                            ((FieldInfo)memberMetadata.Tag).SetValue(model, member.Value);
                            set = true;
                        }
                    }

                    if (!set)
                    {
                        Log.Warning("Failed to set field '{0}.{1}' because the member cannot be found on the model", modelType.GetSafeFullName(false), member.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to populate '{0}.{1}', setting the member value threw an exception", modelType.GetSafeFullName(false), member.Name);
            }
        }
    }
}
