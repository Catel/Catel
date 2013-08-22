// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.ComponentModel;
    using Catel.Data;
    using Catel.Runtime.Serialization;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class XmlSerializerFacts
    {
        [TestClass]
        public class AdvancedSerializationFacts
        {
            [TestMethod]
            public void CorrectlySerializesCustomizedModels()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(null, clonedModel._excludedField);
                Assert.AreEqual("included", clonedModel._includedField);

                Assert.AreEqual(null, clonedModel.ExcludedRegularProperty);
                Assert.AreEqual("included", clonedModel.IncludedRegularProperty);

                Assert.AreEqual(null, clonedModel.ExcludedCatelProperty);
                Assert.AreEqual("included", clonedModel.IncludedCatelProperty);
            }

            [TestMethod]
            public void CorrectlyHandlesNullValues()
            {
                var testModel = new TestModel();

                testModel.IncludedCatelProperty = null;

                var editableObject = testModel as IEditableObject;
                editableObject.BeginEdit();

                testModel.IncludedCatelProperty = "included";

                editableObject.CancelEdit();

                Assert.IsNull(testModel.IncludedCatelProperty);
            }
        }
    }
}