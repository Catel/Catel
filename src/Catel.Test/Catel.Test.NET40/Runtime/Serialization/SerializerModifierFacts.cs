// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerModifierFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.Test.Data;
    using Catel.Test.Runtime.Serialization.TestModels;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class SerializerModifierFacts
    {
        [TestClass]
        public class TheSerializerModifierFunctionality
        {
            [TestMethod]
            public void ComplexInheritanceWorksWithXml()
            {
                var modelC = new TestModels.ModelC();

                Assert.IsNull(modelC.ModelAProperty);
                Assert.IsNull(modelC.ModelBProperty);
                Assert.IsNull(modelC.ModelCProperty);

                var clonedModelC = SerializationTestHelper.SerializeAndDeserialize(modelC, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual("ModifiedA", clonedModelC.ModelAProperty);
                Assert.AreEqual("ModifiedB", clonedModelC.ModelBProperty);
                Assert.AreEqual("ModifiedC", clonedModelC.ModelCProperty);
            }

#if NET
            [TestMethod]
            public void ComplexInheritanceWorksWithBinary()
            {
                var modelC = new TestModels.ModelC();

                Assert.IsNull(modelC.ModelAProperty);
                Assert.IsNull(modelC.ModelBProperty);
                Assert.IsNull(modelC.ModelCProperty);

                var clonedModelC = SerializationTestHelper.SerializeAndDeserialize(modelC, SerializationFactory.GetBinarySerializer());

                Assert.AreEqual("ModifiedA", clonedModelC.ModelAProperty);
                Assert.AreEqual("ModifiedB", clonedModelC.ModelBProperty);
                Assert.AreEqual("ModifiedC", clonedModelC.ModelCProperty);
            }
#endif

            [TestMethod]
            public void MembersIgnoredViaModifier()
            {
                var modelC = new TestModels.ModelC();
                modelC.IgnoredMember = "test is a value";

                var clonedModelC = SerializationTestHelper.SerializeAndDeserialize(modelC, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(null, clonedModelC.IgnoredMember);
            }

            [TestMethod]
            public void SerializesAndDeserializesCompletelyDifferentType()
            {
                var changingType = new ChangingType();

                for (int i = 0; i < 10; i++)
                {
                    changingType.CustomizedCollection.Add(i);
                }

                var clone = SerializationTestHelper.SerializeAndDeserialize(changingType, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(10, clone.CustomizedCollection.Count);
            }
        }
    }
}