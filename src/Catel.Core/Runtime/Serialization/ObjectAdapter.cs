// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectAdapter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Reflection;
    using Catel.Data;
    using Logging;
    using Reflection;

    /// <summary>
    /// Adapter to interact with objects.
    /// </summary>
    public class ObjectAdapter : IObjectAdapter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Data.IObjectAdapter _objectAdapter;

        public ObjectAdapter(Data.IObjectAdapter objectAdapter)
        {
            Argument.IsNotNull(nameof(objectAdapter), objectAdapter);

            _objectAdapter = objectAdapter;
        }

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
                object value = null;

                var modelEditor = model as IModelEditor;
                if (modelEditor != null && modelInfo.CatelPropertyNames.Contains(memberName))
                {
                    var memberMetadata = modelInfo.CatelPropertiesByName[memberName];
                    if (_objectAdapter.GetMemberValue(model, memberName, out value))
                    {
                        var propertyValue = new MemberValue(SerializationMemberGroup.CatelProperty, modelType, memberMetadata.MemberType,
                            memberMetadata.MemberName, memberMetadata.MemberNameForSerialization, value);

                        return propertyValue;
                    }
                }

                if (modelInfo.PropertiesByName.TryGetValue(memberName, out var propertyMemberMetadata))
                {
                    if (_objectAdapter.GetMemberValue(model, memberName, out value))
                    {
                        var propertyValue = new MemberValue(SerializationMemberGroup.RegularProperty, modelType, propertyMemberMetadata.MemberType,
                            propertyMemberMetadata.MemberName, propertyMemberMetadata.MemberNameForSerialization, value);

                        return propertyValue;
                    }
                }

                if (modelInfo.FieldsByName.TryGetValue(memberName, out var fieldMemberMetadata))
                {
                    if (_objectAdapter.GetMemberValue(model, memberName, out value))
                    {
                        var fieldValue = new MemberValue(SerializationMemberGroup.Field, modelType, fieldMemberMetadata.MemberType,
                            fieldMemberMetadata.MemberName, fieldMemberMetadata.MemberNameForSerialization, value);

                        return fieldValue;
                    }
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
                var finalValue = BoxingCache.GetBoxedValue(member.Value);

                // In this very special occasion, we will not use ObjectAdapter since it 
                // will cause property change notifications (which we don't want during deserialization)
                var modelEditor = model as IModelEditor;
                if (modelEditor != null && modelInfo.CatelPropertyNames.Contains(member.Name))
                {
                    modelEditor.SetValueFastButUnsecure(member.Name, finalValue);
                    return;
                }

                _objectAdapter.SetMemberValue(model, member.Name, finalValue);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to populate '{modelType.GetSafeFullName(false)}.{member.Name}', setting the member value threw an exception");
            }
        }
    }
}
