// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidObjectIdGeneratorFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Services
{
    using System;

    using Catel.Services;

    using NUnit.Framework;

    public class GuidObjectIdGeneratorFacts
    {
        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
            public class PersonViewModel2
            {
            }

            public class PersonViewModel4
            {
            }

            public class PersonViewModel1
            {
            }

            public class PersonViewModel3
            {
            }
            [Test]
            public void Returns_New_UniqueIdentifier_Even_If_Are_Generated_By_Different_Instances()
            {
                IObjectIdGenerator<Guid> generator1 = new GuidObjectIdGenerator<PersonViewModel1>();
                IObjectIdGenerator<Guid> generator2 = new GuidObjectIdGenerator<PersonViewModel1>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<Guid> generator = new GuidObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.AreEqual(uniqueIdentifier, generator.GetUniqueIdentifier(true));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DiferentTypes()
            {
                IObjectIdGenerator<Guid> generator1 = new GuidObjectIdGenerator<PersonViewModel3>();
                IObjectIdGenerator<Guid> generator2 = new GuidObjectIdGenerator<PersonViewModel4>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }
        }
    }
}