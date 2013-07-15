// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using IoC;

    /// <summary>
    /// Factory responsible to create serializers. Internally this will query the <see cref="ServiceLocator"/>
    /// the retrieve the registered serializers.
    /// </summary>
    public static class SerializationFactory
    {
        /// <summary>
        /// Gets the binary serializer.
        /// </summary>
        /// <returns>The registered <see cref="IBinarySerializer"/>.</returns>
        public static IBinarySerializer GetBinarySerializer()
        {
            return ServiceLocator.Default.ResolveType<IBinarySerializer>();
        }

        /// <summary>
        /// Gets the XML serializer.
        /// </summary>
        /// <returns>The registered <see cref="IXmlSerializer"/>.</returns>
        public static IXmlSerializer GetXmlSerializer()
        {
            return ServiceLocator.Default.ResolveType<IXmlSerializer>();
        }
    }
}