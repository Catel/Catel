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

                Assert.That(generator2.GetUniqueIdentifier(), Is.Not.EqualTo(generator1.GetUniqueIdentifier()));
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<Guid> generator = new GuidObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.That(generator.GetUniqueIdentifier(true), Is.EqualTo(uniqueIdentifier));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DifferentTypes()
            {
                IObjectIdGenerator<Guid> generator1 = new GuidObjectIdGenerator<PersonViewModel3>();
                IObjectIdGenerator<Guid> generator2 = new GuidObjectIdGenerator<PersonViewModel4>();

                Assert.That(generator2.GetUniqueIdentifier(), Is.Not.EqualTo(generator1.GetUniqueIdentifier()));
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
            [Test, Explicit]
            public void Returns_A_Released_Identifier_If_The_Instance_Is_Released_And_Reuse_Is_True()
            {
                var generator = new GuidObjectIdGenerator<PersonViewModel1>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel1());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.That(generator.GetUniqueIdentifierForInstance(new PersonViewModel1(), true), Is.EqualTo(uniqueIdentifierForInstance));
            }

            [Test]
            public void Returns_A_Unique_Identifier_Even_When_An_Instance_Is_Released_But_Reuse_Is_False()
            {
                var generator = new GuidObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel2());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.That(generator.GetUniqueIdentifierForInstance(new PersonViewModel2()), Is.Not.EqualTo(uniqueIdentifierForInstance));
            }

            [Test]
            public void Returns_A_Unique_Identifier_For_Different_Instances()
            {
                var generator = new GuidObjectIdGenerator<PersonViewModel3>();

                var a = generator.GetUniqueIdentifierForInstance(new PersonViewModel3());
                var b = generator.GetUniqueIdentifierForInstance(new PersonViewModel3());

                Assert.That(a, Is.Not.EqualTo(b));
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
