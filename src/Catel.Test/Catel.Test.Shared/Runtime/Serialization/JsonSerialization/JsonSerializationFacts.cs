// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using NUnit.Framework;

    public class JsonSerializationFacts
    {
        [TestFixture]
        public class BasicSerializationFacts
        {
        }

        [TestFixture]
        public class AdvancedSerializationFacts
        {
            [TestCase]
            public void CorrectlySerializesToJsonString()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                var json = testModel.ToJson(null);

                Assert.IsFalse(json.Contains("Excluded"));
            }
        }
    }
}