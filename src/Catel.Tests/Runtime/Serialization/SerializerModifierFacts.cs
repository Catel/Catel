namespace Catel.Tests.Runtime.Serialization
{
    using Catel.Runtime.Serialization;
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

                Assert.That(modelC.ModelAProperty, Is.EqualTo(string.Empty));
                Assert.That(modelC.ModelBProperty, Is.EqualTo(string.Empty));
                Assert.That(modelC.ModelCProperty, Is.EqualTo(string.Empty));

                var clonedModelC = SerializationTestHelper.SerializeAndDeserialize(modelC, SerializationFactory.GetXmlSerializer());

                Assert.That(clonedModelC.ModelAProperty, Is.EqualTo("ModifiedA"));
                Assert.That(clonedModelC.ModelBProperty, Is.EqualTo("ModifiedB"));
                Assert.That(clonedModelC.ModelCProperty, Is.EqualTo("ModifiedC"));
            }

            [TestCase]
            public void MembersIgnoredViaModifier()
            {
                var modelC = new TestModels.ModelC();
                modelC.IgnoredMember = "test is a value";

                var clonedModelC = SerializationTestHelper.SerializeAndDeserialize(modelC, SerializationFactory.GetXmlSerializer());

                Assert.That(clonedModelC.IgnoredMember, Is.EqualTo(string.Empty));
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

                Assert.That(clone.CustomizedCollection.Count, Is.EqualTo(10));
                for (int i = 0; i < 10; i++)
                {
                    Assert.That(clone.CustomizedCollection[i], Is.EqualTo(i));
                }
            }
        }
    }
}
