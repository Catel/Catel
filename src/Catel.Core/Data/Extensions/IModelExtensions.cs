// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// IModel extensions.
    /// </summary>
    public static partial class IModelExtensions
    {
        /// <summary>
        /// Clears the <see cref="ModelBase.IsDirty" /> on all childs.
        /// </summary>
        /// <param name="model">The model.</param>
        [ObsoleteEx(ReplacementTypeOrMember = "ClearIsDirtyOnAllChildren(IModel, bool)", TreatAsErrorFromVersion = "5.10", RemoveInVersion = "6.0")]
        public static void ClearIsDirtyOnAllChilds(this IModel model)
        {
            ClearIsDirtyOnAllChildren(model, false);
        }

        /// <summary>
        /// Clears the <see cref="ModelBase.IsDirty" /> on all childs.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="suspendNotifications">If set to <c>true</c>, the change will not be raised using the <see cref="INotifyPropertyChanged"/> interface.</param>
        public static void ClearIsDirtyOnAllChildren(this IModel model, bool suspendNotifications = false)
        {
            Argument.IsNotNull("model", model);

            ClearIsDirtyOnAllChildren(model, new HashSet<object>(), suspendNotifications);
        }

        /// <summary>
        /// Clears the <see cref="ModelBase.IsDirty"/> on all childs.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="handledReferences">The already handled references, required to prevent circular stackoverflows.</param>
        /// <param name="suspendNotifications">If set to <c>true</c>, the change will not be raised using the <see cref="INotifyPropertyChanged"/> interface.</param>
        private static void ClearIsDirtyOnAllChildren(object obj, HashSet<object> handledReferences, bool suspendNotifications)
        {
            var modelEditor = obj as IModelEditor;
            var objAsIEnumerable = obj as IEnumerable;
            if (objAsIEnumerable is string)
            {
                objAsIEnumerable = null;
            }

            if (!(modelEditor is null))
            {
                if (handledReferences.Contains(modelEditor))
                {
                    return;
                }

                if (suspendNotifications)
                {
                    modelEditor.SetValueFastButUnsecure(nameof(ModelBase.IsDirty), false);
                }
                else
                {
                    modelEditor.SetValue(nameof(ModelBase.IsDirty), false);
                }

                handledReferences.Add(modelEditor);

                var catelTypeInfo = ModelBase.PropertyDataManager.GetCatelTypeInfo(obj.GetType());
                foreach (var property in catelTypeInfo.GetCatelProperties())
                {
                    var value = modelEditor.GetValue(property.Value.Name);

                    ClearIsDirtyOnAllChildren(value, handledReferences, suspendNotifications);
                }
            }
            else if (objAsIEnumerable != null)
            {
                foreach (var childItem in objAsIEnumerable)
                {
                    if (childItem is IModel)
                    {
                        ClearIsDirtyOnAllChildren(childItem, handledReferences, suspendNotifications);
                    }
                }
            }
        }
    }
}
