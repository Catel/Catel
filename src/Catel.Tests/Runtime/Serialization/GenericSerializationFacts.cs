namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;

    public partial class GenericSerializationFacts
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static void TestSerializationOnXmlSerializer(Action<ISerializer, ISerializationConfiguration, string> action,
            bool testWithoutGraphIdsAsWell = true, ISerializationManager serializationManager = null)
        {
            var serializers = new List<ISerializer>();

            serializers.Add(SerializationTestHelper.GetXmlSerializer(serializationManager));

            TestSerializationOnSerializers(serializers, action, serializationManager);
        }

        private static void TestSerializationOnJsonSerializer(Action<ISerializer, ISerializationConfiguration, string> action,
            bool testWithoutGraphIdsAsWell = true, ISerializationManager serializationManager = null)
        {
            var serializers = new List<ISerializer>();

            serializers.Add(SerializationTestHelper.GetJsonSerializer(serializationManager));

            if (testWithoutGraphIdsAsWell)
            {
                var basicJsonSerializer = SerializationTestHelper.GetJsonSerializer(serializationManager);
                basicJsonSerializer.PreserveReferences = false;
                basicJsonSerializer.WriteTypeInfo = false;
                serializers.Add(basicJsonSerializer);
            }

            TestSerializationOnSerializers(serializers, action, serializationManager);
        }

        private static void TestSerializationOnAllSerializers(Action<ISerializer, ISerializationConfiguration, string> action,
            bool testWithoutGraphIdsAsWell = true, ISerializationManager serializationManager = null)
        {
            var serializers = new List<ISerializer>();

            serializers.Add(SerializationTestHelper.GetXmlSerializer(serializationManager));
            serializers.Add(SerializationTestHelper.GetJsonSerializer(serializationManager));

            if (testWithoutGraphIdsAsWell)
            {
                var basicJsonSerializer = SerializationTestHelper.GetJsonSerializer(serializationManager);
                basicJsonSerializer.PreserveReferences = false;
                basicJsonSerializer.WriteTypeInfo = false;
                serializers.Add(basicJsonSerializer);
            }

            TestSerializationOnSerializers(serializers, action, serializationManager);
        }

        private static void TestSerializationOnSerializers(List<ISerializer> serializers, Action<ISerializer, ISerializationConfiguration, string> action,
            ISerializationManager serializationManager = null)
        {
            var serializerConfigurations = new Dictionary<Type, List<ISerializationConfiguration>>();

            serializerConfigurations[typeof(XmlSerializer)] = new List<ISerializationConfiguration>(new[]
            {
                new XmlSerializationConfiguration
                {
                    // Default config
                },
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
