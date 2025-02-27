﻿namespace Catel.Windows.Interactivity
{
    using System.Windows.Documents;
    using System.Windows.Navigation;
    using Catel.Services;
    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// Navigate behavior to allow the execution of an url in non-pages for WPF.
    /// </summary>
    public class Navigate : Behavior<Hyperlink>
    {
        private readonly IProcessService _processService;


        public Navigate(IProcessService processService)
        {
            _processService = processService;
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

        private void AssociatedObjectRequestNavigate(object? sender, RequestNavigateEventArgs e)
        {
            var uri = AssociatedObject.NavigateUri;
            if (uri is not null)
            {
                _processService.StartProcess(uri.ToString());
            }
        }
    }
}
