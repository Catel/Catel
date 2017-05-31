// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Binary;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;

    public partial class GenericSerializationFacts
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static void TestSerializationOnAllSerializers(Action<ISerializer, ISerializationConfiguration, string> action, 
            bool testWithoutGraphIdsAsWell = true, ISerializationManager serializationManager = null)
        {
            if (serializationManager == null)
            {
                serializationManager = new SerializationManager();
            }

            var serializerConfigurations = new Dictionary<Type, List<ISerializationConfiguration>>();

            serializerConfigurations[typeof(XmlSerializer)] = new List<ISerializationConfiguration>(new[]
            {
                new XmlSerializationConfiguration
                {
                    OptimalizationMode = XmlSerializerOptimalizationMode.Performance
                },
                new XmlSerializationConfiguration
                {
                    OptimalizationMode = XmlSerializerOptimalizationMode.PrettyXml
                },
                //new XmlSerializationConfiguration
                //{
                //    OptimalizationMode = XmlSerializerOptimalizationMode.PrettyXmlAgressive
                //},
            });

            serializerConfigurations[typeof(BinarySerializer)] = new List<ISerializationConfiguration>(new[]
            {
                new SerializationConfiguration()
            });

            serializerConfigurations[typeof(JsonSerializer)] = new List<ISerializationConfiguration>(new[]
            {
                new JsonSerializationConfiguration
                {
                    UseBson = false
                },
                new JsonSerializationConfiguration
                {
                    UseBson = true
                },
            });

            var serializers = new List<ISerializer>();

            serializers.Add(SerializationTestHelper.GetXmlSerializer(serializationManager));
            serializers.Add(SerializationTestHelper.GetBinarySerializer(serializationManager));
            serializers.Add(SerializationTestHelper.GetJsonSerializer(serializationManager));

            if (testWithoutGraphIdsAsWell)
            {
                var basicJsonSerializer = SerializationTestHelper.GetJsonSerializer(serializationManager);
                basicJsonSerializer.PreserveReferences = false;
                basicJsonSerializer.WriteTypeInfo = false;
                serializers.Add(basicJsonSerializer);
            }

            foreach (var serializer in serializers)
            {
                var type = serializer.GetType();
                var typeName = type.GetSafeFullName(false);

                var configurations = serializerConfigurations[type];
                foreach (var configuration in configurations)
                {
                    Log.Info();
                    Log.Info();
                    Log.Info();
                    Log.Info("=== TESTING SERIALIZER: {0} ===", typeName);
                    Log.Info();
                    Log.Info();
                    Log.Info();

                    action(serializer, configuration, typeName);
                }
            }
        }
    }
}