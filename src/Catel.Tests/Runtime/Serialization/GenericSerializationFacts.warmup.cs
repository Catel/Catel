// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization
{
    using System.Linq;
    using Catel.Reflection;
    using NUnit.Framework;

    public partial class GenericSerializationFacts
    {
        [TestFixture, Explicit]
        public class TheWarmupMethod
        {
            [TestCase]
            public void WarmsUpSpecificTypes()
            {
                var typesToWarmup = new[] { typeof(CircularTestModel), typeof(TestModel) };

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    TimeMeasureHelper.MeasureAction(5, string.Format("{0} serializer warmup", serializer.GetType().Name),
                        () => serializer.Warmup(typesToWarmup),
                        () =>
                        {
                            TypeCache.InitializeTypes();

                            ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                            ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => x.IsModelBase()).Count());
                        });
                });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }

            [TestCase]
            public void WarmsUpAllTypes()
            {
                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    TimeMeasureHelper.MeasureAction(5, string.Format("{0} serializer warmup", serializer.GetType().Name),
                        () => serializer.Warmup(),
                        () =>
                        {
                            TypeCache.InitializeTypes();

                            ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                            ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => x.IsModelBase()).Count());
                        });
                });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }
        }
    }
}
