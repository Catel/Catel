// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Navigate.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Windows.Interactivity
{
    using System.Windows.Documents;
    using Microsoft.Xaml.Behaviors;
    using IoC;
    using System.Windows.Navigation;
    using Catel.Services;

    /// <summary>
    /// Navigate behavior to allow the execution of an url in non-pages for WPF.
    /// </summary>
    public class Navigate : Behavior<Hyperlink>
    {
        private static readonly IProcessService _processService;

        #region Methods
        /// <summary>
        /// Initializes static members of the <see cref="Navigate"/> class.
        /// </summary>
        static Navigate()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            _processService = dependencyResolver.Resolve<IProcessService>();
        }

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
                _processService.StartProcess(uri.ToString());
            }
        }
        #endregion
    }
}

#endif
