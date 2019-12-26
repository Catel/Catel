// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Services;

    using Reflection;

    /// <summary>
    /// Resolver that will resolve view model types based on the view type. For example, if a view with the type
    /// name <c>MyAssembly.Views.PersonView</c> is inserted, this could result in the view model type
    /// <c>MyAssembly.ViewModels.PersonViewModel</c>.
    /// </summary>
    public class ViewModelLocator : LocatorBase, IViewModelLocator
    {
        /// <summary>
        /// Registers the specified view model in the local cache. This cache will also be used by the
        /// <see cref="ResolveViewModel"/> method.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        public void Register(Type viewType, Type viewModelType)
        {
            Argument.IsNotNull("viewType", viewType);
            Argument.IsNotNull("viewModelType", viewModelType);

            var viewTypeName = TypeHelper.GetTypeNameWithAssembly(viewType.AssemblyQualifiedName);
            var viewModelTypeName = TypeHelper.GetTypeNameWithAssembly(viewModelType.AssemblyQualifiedName);

            Register(viewTypeName, viewModelTypeName);
        }

        /// <summary>
        /// Determines whether the specified view type is compatible with the view model. A view is compatible
        /// if it's either resolved via naming conventions or registered manually.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>
        ///   <c>true</c> if the view is compatible with the view model; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsCompatible(Type viewType, Type viewModelType)
        {
            Argument.IsNotNull("viewType", viewType);
            Argument.IsNotNull("viewModelType", viewModelType);

            var viewTypeName = TypeHelper.GetTypeNameWithAssembly(viewType.AssemblyQualifiedName);
            var viewModelTypeName = TypeHelper.GetTypeNameWithAssembly(viewModelType.AssemblyQualifiedName);

            var values = ResolveValues(viewTypeName);
            return values.Contains(viewModelTypeName);
        }

        /// <summary>
        /// Resolves a view model type by the view and the registered <see cref="ILocator.NamingConventions"/>.
        /// </summary>
        /// <param name="viewType">Type of the view to resolve the view model for.</param>
        /// <returns>The resolved view model or <c>null</c> if the view model could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public virtual Type ResolveViewModel(Type viewType)
        {
            Argument.IsNotNull("viewType", viewType);

            var viewTypeName = TypeHelper.GetTypeNameWithAssembly(viewType.AssemblyQualifiedName);

            var resolvedType = Resolve(viewTypeName);
            return TypeCache.GetType(resolvedType);
        }

        /// <summary>
        /// Resolves a single naming convention.
        /// <para/>
        /// This method is abstract because each locator should or could use its own naming convention to resolve
        /// the type. The <see cref="LocatorBase.Resolve"/> method has prepared all the values such as the assembly name and the
        /// only thing this method has to do is to actually resolve a string value based on the specified naming convention.
        /// </summary>
        /// <param name="assembly">The assembly name.</param>
        /// <param name="typeToResolveName">The full type name of the type to resolve.</param>
        /// <param name="namingConvention">The naming convention to use for resolving.</param>
        /// <returns>The resolved naming convention.</returns>
        protected override string ResolveNamingConvention(string assembly, string typeToResolveName, string namingConvention)
        {
            return NamingConvention.ResolveViewModelByViewName(assembly, typeToResolveName, namingConvention);
        }

        /// <summary>
        /// Gets the default naming conventions.
        /// </summary>
        /// <returns>An enumerable of default naming conventions.</returns>
        /// <remarks></remarks>
        protected override IEnumerable<string> GetDefaultNamingConventions()
        {
            var namingConventions = new List<string>();

            namingConventions.Add(string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Up, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}ControlViewModel", NamingConvention.Up, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}WindowViewModel", NamingConvention.Up, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}PageViewModel", NamingConvention.Up, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}ActivityViewModel", NamingConvention.Up, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}FragmentViewModel", NamingConvention.Up, NamingConvention.ViewName));

            namingConventions.Add(string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}ControlViewModel", NamingConvention.Assembly, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}WindowViewModel", NamingConvention.Assembly, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}PageViewModel", NamingConvention.Assembly, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}ActivityViewModel", NamingConvention.Assembly, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.ViewModels.{1}FragmentViewModel", NamingConvention.Assembly, NamingConvention.ViewName));

            namingConventions.Add(string.Format("{0}.{1}ViewModel", NamingConvention.Current, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.{1}ControlViewModel", NamingConvention.Current, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.{1}WindowViewModel", NamingConvention.Current, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.{1}PageViewModel", NamingConvention.Current, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.{1}ActivityViewModel", NamingConvention.Current, NamingConvention.ViewName));
            namingConventions.Add(string.Format("{0}.{1}FragmentViewModel", NamingConvention.Current, NamingConvention.ViewName));

            return namingConventions;
        }
    }
}
