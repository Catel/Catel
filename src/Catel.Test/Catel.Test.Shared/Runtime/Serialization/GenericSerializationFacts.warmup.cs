// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Media;
    using Catel.Collections;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Binary;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;
    using Data;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TestModels;
    using JsonSerializer = Catel.Runtime.Serialization.Json.JsonSerializer;

    public partial class GenericSerializationFacts
    {
        [TestFixture, Explicit]
        public class TheWarmupMethod
        {
            [TestCase]
            public void WarmsUpSpecificTypes()
            {
                var typesToWarmup = new[] { typeof(CircularTestModel), typeof(TestModel) };

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    TimeMeasureHelper.MeasureAction(5, string.Format("{0} serializer warmup", serializer.GetType().Name),
                        () => serializer.Warmup(typesToWarmup),
                        () =>
                        {
                            TypeCache.InitializeTypes();

                            ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                            ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                        });
                });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }

            [TestCase]
            public void WarmsUpAllTypes()
            {
                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    TimeMeasureHelper.MeasureAction(5, string.Format("{0} serializer warmup", serializer.GetType().Name),
                        () => serializer.Warmup(),
                        () =>
                        {
                            TypeCache.InitializeTypes();

                            ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                            ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                        });
                });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }
        }
    }
}