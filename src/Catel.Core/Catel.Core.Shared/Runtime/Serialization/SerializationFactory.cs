// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
#if NET
    using Catel.Runtime.Serialization.Binary;
#endif

    using Catel.Runtime.Serialization.Xml;
    using IoC;

    /// <summary>
    /// Factory responsible to create serializers. Internally this will query the <see cref="ServiceLocator"/>
    /// the retrieve the registered serializers.
    /// </summary>
    public static class SerializationFactory
    {
#if NET
        /// <summary>
        /// Gets the binary serializer.
        /// </summary>
        /// <returns>The registered <see cref="IBinarySerializer"/>.</returns>
        public static IBinarySerializer GetBinarySerializer()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            return dependencyResolver.Resolve<IBinarySerializer>();
        }
#endif

        /// <summary>
        /// Gets the XML serializer.
        /// </summary>
        /// <returns>The registered <see cref="IXmlSerializer"/>.</returns>
        public static IXmlSerializer GetXmlSerializer()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            return dependencyResolver.Resolve<IXmlSerializer>();
        }
    }
}