// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using Data;

    /// <summary>
    /// Extension methods for view model classes.
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Gets the validation summary for the specified <paramref name="viewModel"/> and, if specified, the children as well.
        /// <para />
        /// This method does not filter on any tag.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="includeChildViewModelValidations">if set to <c>true</c>, all validation from all child view models should be gathered as well.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public static IValidationSummary GetValidationSummary(this ViewModelBase viewModel, bool includeChildViewModelValidations)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var validationContext = GetNestedValidationContext(viewModel, includeChildViewModelValidations);

            return new ValidationSummary(validationContext);
        }

        /// <summary>
        /// Gets the validation summary for the specified <paramref name="viewModel"/> and, if specified, the children as well.
        /// <para/>
        /// This method also filters on the specified tag.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="includeChildViewModelValidations">if set to <c>true</c>, all validation from all child view models should be gathered as well.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public static IValidationSummary GetValidationSummary(this ViewModelBase viewModel, bool includeChildViewModelValidations, object tag)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var validationContext = GetNestedValidationContext(viewModel, includeChildViewModelValidations);

            return new ValidationSummary(validationContext, tag);
        }

        /// <summary>
        /// Gets the nested validation context. If <paramref name="recursive"/> is <c>true</c>, the validation context returned
        /// will include all validation from all registered children.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="recursive">if set to <c>true</c>, the validation context will be merged with all children.</param>
        /// <returns>
        /// A combined <see cref="IValidationContext"/> of all the child view models and the <paramref name="viewModel"/> itself.
        /// </returns>
        /// <remarks>
        /// This method does not check for arguments for performance reasons and because it's private.
        /// </remarks>
        private static IValidationContext GetNestedValidationContext(ViewModelBase viewModel, bool recursive)
        {
            var validationContext = new ValidationContext();

            validationContext.SynchronizeWithContext(viewModel.ValidationContext, true);

            if (recursive)
            {
                foreach (var childViewModel in viewModel.ChildViewModels)
                {
                    var childAsViewModelBase = childViewModel as ViewModelBase;
                    if (childAsViewModelBase != null)
                    {
                        validationContext.SynchronizeWithContext(GetNestedValidationContext(childAsViewModelBase, true), true);
                    }
                }
            }

            return validationContext;
        }
    }
}
