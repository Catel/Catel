// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidObjectIdGeneratorFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Services
{
    using System;

    using Catel.Services;

    using NUnit.Framework;

    public class GuidObjectIdGeneratorFacts
    {
        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
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
            public void Returns_Unique_Identifier_For_DifferentTypes()
            {
                IObjectIdGenerator<Guid> generator1 = new GuidObjectIdGenerator<PersonViewModel3>();
                IObjectIdGenerator<Guid> generator2 = new GuidObjectIdGenerator<PersonViewModel4>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

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
        }

        [TestFixture]
        public class GetUniqueIdentifierForInstance_Method
        {
            [Test]
            public void Returns_A_Released_Identifier_If_The_Instance_Is_Released_And_Reuse_Is_True()
            {
                GuidObjectIdGenerator<PersonViewModel1> generator = new GuidObjectIdGenerator<PersonViewModel1>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel1());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.AreEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel1(), true));
            }

            [Test]
            public void Returns_A_Unique_Identifier_Even_When_An_Instance_Is_Released_But_Reuse_Is_False()
            {
                GuidObjectIdGenerator<PersonViewModel2> generator = new GuidObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel2());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.AreNotEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel2()));
            }

            [Test]
            public void Returns_A_Unique_Identifier_For_Different_Instances()
            {
                GuidObjectIdGenerator<PersonViewModel3> generator = new GuidObjectIdGenerator<PersonViewModel3>();
                Assert.AreNotEqual(generator.GetUniqueIdentifierForInstance(new PersonViewModel3()), generator.GetUniqueIdentifierForInstance(new PersonViewModel3())); 
            }

            public class PersonViewModel1
            {
            }
            public class PersonViewModel2
            {
            }
            public class PersonViewModel3
            {
            }
        }
    }
}