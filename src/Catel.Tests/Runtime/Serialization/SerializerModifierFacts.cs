// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerModifierFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.Tests.Data;
    using Catel.Tests.Runtime.Serialization.TestModels;
    using NUnit.Framework;

    public class SerializerModifierFacts
    {
        [TestFixture]
        public class TheSerializerModifierFunctionality
        {
            [TestCase]
            public void ComplexInheritanceWorksWithXml()
            {
                var modelC = new TestModels.ModelC();

                Assert.AreEqual(string.Empty, modelC.ModelAProperty);
                Assert.AreEqual(string.Empty, modelC.ModelBProperty);
                Assert.AreEqual(string.Empty, modelC.ModelCProperty);

                var clonedModelC = SerializationTestHelper.SerializeAndDeserialize(modelC, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual("ModifiedA", clonedModelC.ModelAProperty);
                Assert.AreEqual("ModifiedB", clonedModelC.ModelBProperty);
                Assert.AreEqual("ModifiedC", clonedModelC.ModelCProperty);
            }

            [TestCase]
            public void MembersIgnoredViaModifier()
            {
                var modelC = new TestModels.ModelC();
                modelC.IgnoredMember = "test is a value";

                var clonedModelC = SerializationTestHelper.SerializeAndDeserialize(modelC, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(string.Empty, clonedModelC.IgnoredMember);
            }

            [TestCase]
            public void SerializesAndDeserializesCompletelyDifferentType()
            {
                var changingType = new ChangingType();

                for (int i = 0; i < 10; i++)
                {
                    changingType.CustomizedCollection.Add(i);
                }

                var clone = SerializationTestHelper.SerializeAndDeserialize(changingType, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(10, clone.CustomizedCollection.Count);
                for (int i = 0; i < 10; i++)
                {
                    Assert.AreEqual(i, clone.CustomizedCollection[i]);
                }
            }
        }
    }
}
