// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Navigate.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using IoC;
    using MVVM.Services;
    using System.Windows.Navigation;

    /// <summary>
    /// Navigate behavior to allow the execution of an url in non-pages for WPF.
    /// </summary>
    public class Navigate : Behavior<Hyperlink>
    {
        #region Methods
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.RequestNavigate += AssociatedObjectRequestNavigate;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.RequestNavigate -= AssociatedObjectRequestNavigate;

            base.OnDetaching();
        }

        private void AssociatedObjectRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var uri = AssociatedObject.NavigateUri;

            if (uri != null)
            {
                ServiceLocator.Default.ResolveType<IProcessService>().StartProcess(uri.ToString());
            }
        }
        #endregion
    }
}