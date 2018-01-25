// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericBasedObjectIdGeneratorsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Services
{
    using Catel.Services;
    using Catel.Test.ViewModels;

    using NUnit.Framework;

    public class IntegerObjectIdGeneratorFacts
    {
        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
            [Test]
            public void Returns_New_UniqueIdentifier_Even_If_Are_Generated_By_Different_Instances()
            {
                IObjectIdGenerator<int> generator1 = new IntegerObjectIdGenerator<PersonViewModel>();
                IObjectIdGenerator<int> generator2 = new IntegerObjectIdGenerator<PersonViewModel>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<int> generator = new IntegerObjectIdGenerator<PersonViewModel>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.AreEqual(uniqueIdentifier, generator.GetUniqueIdentifier(true));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DiferentTypes()
            {
                IObjectIdGenerator<int> generator1 = new IntegerObjectIdGenerator<PersonViewModel>();
                IObjectIdGenerator<int> generator2 = new IntegerObjectIdGenerator<SameNamespacePersonViewModel>();

                Assert.AreEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }
        }
    }

    public class LongObjectIdGeneratorFacts
    {
        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
            [Test]
            public void Returns_New_UniqueIdentifier_Even_If_Are_Generated_By_Different_Instances()
            {
                IObjectIdGenerator<long> generator1 = new LongObjectIdGenerator<PersonViewModel>();
                IObjectIdGenerator<long> generator2 = new LongObjectIdGenerator<PersonViewModel>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<long> generator = new LongObjectIdGenerator<PersonViewModel>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.AreEqual(uniqueIdentifier, generator.GetUniqueIdentifier(true));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DiferentTypes()
            {
                IObjectIdGenerator<int> generator1 = new IntegerObjectIdGenerator<PersonViewModel>();
                IObjectIdGenerator<int> generator2 = new IntegerObjectIdGenerator<SameNamespacePersonViewModel>();

                Assert.AreEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

        }
    }

    public class ULongObjectIdGeneratorFacts
    {
        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
            [Test]
            public void Returns_New_UniqueIdentifier_Even_If_Are_Generated_By_Different_Instances()
            {
                IObjectIdGenerator<ulong> generator1 = new ULongObjectIdGenerator<PersonViewModel>();
                IObjectIdGenerator<ulong> generator2 = new ULongObjectIdGenerator<PersonViewModel>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<ulong> generator = new ULongObjectIdGenerator<PersonViewModel>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.AreEqual(uniqueIdentifier, generator.GetUniqueIdentifier(true));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DiferentTypes()
            {
                IObjectIdGenerator<int> generator1 = new IntegerObjectIdGenerator<PersonViewModel>();
                IObjectIdGenerator<int> generator2 = new IntegerObjectIdGenerator<SameNamespacePersonViewModel>();

                Assert.AreEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

        }
    }
}