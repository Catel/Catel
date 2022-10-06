namespace Catel.Windows.Markup
{
    using System;
    using IoC;
    using System.Windows.Markup;

    /// <summary>
    /// Service dependency extension to allow service access in xaml for services with properties.
    /// </summary>
    public class ServiceDependencyExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDependencyExtension"/> class.
        /// </summary>
        public ServiceDependencyExtension()
            : this(typeof(object))
        {
            // Keep empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDependencyExtension"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ServiceDependencyExtension(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConstructorArgument("type")]
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object? Tag { get; set; }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object? ProvideValue(IServiceProvider? serviceProvider)
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return null;
            }

            var dependencyResolver = this.GetDependencyResolver();
            return dependencyResolver.Resolve(Type, Tag);
        }
    }
}
