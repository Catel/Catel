// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// IModel extensions.
    /// </summary>
    public static partial class IModelExtensions
    {
        /// <summary>
        /// Clears the <see cref="ModelBase.IsDirty" /> on all childs.
        /// </summary>
        /// <param name="model">The model.</param>
        public static void ClearIsDirtyOnAllChilds(this IModel model)
        {
            Argument.IsNotNull("model", model);

            ClearIsDirtyOnAllChilds(model, new HashSet<IModel>());
        }

        /// <summary>
        /// Clears the <see cref="ModelBase.IsDirty"/> on all childs.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="handledReferences">The already handled references, required to prevent circular stackoverflows.</param>
        private static void ClearIsDirtyOnAllChilds(object obj, HashSet<IModel> handledReferences)
        {
            var objAsModelBase = obj as IModel;
            var objAsIEnumerable = obj as IEnumerable;
            if (objAsIEnumerable is string)
            {
                objAsIEnumerable = null;
            }

            if (objAsModelBase != null)
            {
                if (handledReferences.Contains(objAsModelBase))
                {
                    return;
                }

                objAsModelBase.SetValue(nameof(ModelBase.IsDirty), false);
                handledReferences.Add(objAsModelBase);

                var catelTypeInfo = ModelBase.PropertyDataManager.GetCatelTypeInfo(obj.GetType());
                foreach (var property in catelTypeInfo.GetCatelProperties())
                {
                    var value = objAsModelBase.GetValue(property.Value.Name);

                    ClearIsDirtyOnAllChilds(value, handledReferences);
                }
            }
            else if (objAsIEnumerable != null)
            {
                foreach (var childItem in objAsIEnumerable)
                {
                    if (childItem is ModelBase)
                    {
                        ClearIsDirtyOnAllChilds(childItem, handledReferences);
                    }
                }
            }
        }
    }
}