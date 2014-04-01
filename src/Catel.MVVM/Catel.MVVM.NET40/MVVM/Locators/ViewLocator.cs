// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;

    using Catel.Services;

    using Reflection;

    /// <summary>
    /// Resolver that will resolve view types based on the view model type. For example, if a view model with the type
    /// name <c>MyAssembly.ViewModels.PersonViewModel</c> is inserted, this could result in the view type
    /// <c>MyAssembly.Views.PersonView</c>.
    /// </summary>
    public class ViewLocator : LocatorBase, IViewLocator
    {
        /// <summary>
        /// Registers the specified view in the local cache. This cache will also be used by the
        /// <see cref="ResolveView"/> method.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public void Register(Type viewModelType, Type viewType)
        {
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.IsNotNull("viewType", viewType);

            var viewModelTypeName = TypeHelper.GetTypeNameWithAssembly(viewModelType.AssemblyQualifiedName);
            var viewTypeName = TypeHelper.GetTypeNameWithAssembly(viewType.AssemblyQualifiedName);

            Register(viewModelTypeName, viewTypeName);
        }

        /// <summary>
        /// Resolves a view type by the view model and the registered <see cref="ILocator.NamingConventions"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to resolve the view for.</param>
        /// <returns>The resolved view or <c>null</c> if the view could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        public virtual Type ResolveView(Type viewModelType)
        {
            Argument.IsNotNull("viewModelType", viewModelType);

            var viewModelTypeName = TypeHelper.GetTypeNameWithAssembly(viewModelType.AssemblyQualifiedName);

            string resolvedType = Resolve(viewModelTypeName);
            return GetTypeFromString(resolvedType);
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
            return NamingConvention.ResolveViewByViewModelName(assembly, typeToResolveName, namingConvention);
        }

        /// <summary>
        /// Gets the default naming conventions.
        /// </summary>
        /// <returns>An enumerable of default naming conventions.</returns>
        protected override IEnumerable<string> GetDefaultNamingConventions()
        {
            var namingConventions = new List<string>();

            namingConventions.Add(string.Format("[UP].Views.{0}", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Views.{0}View", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Views.{0}Control", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Views.{0}Window", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Views.{0}Page", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Views.{0}Activity", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Controls.{0}", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Controls.{0}Control", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Pages.{0}", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Pages.{0}Page", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Windows.{0}", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Windows.{0}Window", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Activities.{0}", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("[UP].Activities.{0}Activity", NamingConvention.ViewModelName));

            namingConventions.Add(string.Format("{0}.Views.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Views.{1}View", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Views.{1}Control", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Views.{1}Page", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Views.{1}Window", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Views.{1}Activity", NamingConvention.Assembly, NamingConvention.ViewModelName));

            namingConventions.Add(string.Format("{0}.Controls.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Controls.{1}Control", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Pages.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Pages.{1}Page", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Windows.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Windows.{1}Window", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Activities.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.Activities.{1}Activity", NamingConvention.Assembly, NamingConvention.ViewModelName));

            namingConventions.Add(string.Format("{0}.UI.Views.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Views.{1}View", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Views.{1}Control", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Views.{1}Page", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Views.{1}Window", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Views.{1}Activity", NamingConvention.Assembly, NamingConvention.ViewModelName));

            namingConventions.Add(string.Format("{0}.UI.Controls.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Controls.{1}Control", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Pages.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Pages.{1}Page", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Windows.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Windows.{1}Window", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Activities.{1}", NamingConvention.Assembly, NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("{0}.UI.Activities.{1}Activity", NamingConvention.Assembly, NamingConvention.ViewModelName));

            return namingConventions;
        }
    }
}