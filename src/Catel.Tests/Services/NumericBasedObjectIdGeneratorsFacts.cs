// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericBasedObjectIdGeneratorsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Services
{
    using System;

    using Catel.Services;

    using NUnit.Framework;

    public class IntegerObjectIdGeneratorFacts
    {
        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
            [Test]
            public void Returns_New_UniqueIdentifier_Even_If_Are_Generated_By_Different_Instances()
            {
                IObjectIdGenerator<int> generator1 = new IntegerObjectIdGenerator<PersonViewModel1>();
                IObjectIdGenerator<int> generator2 = new IntegerObjectIdGenerator<PersonViewModel1>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<int> generator = new IntegerObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.AreEqual(uniqueIdentifier, generator.GetUniqueIdentifier(true));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DifferentTypes()
            {
                IObjectIdGenerator<int> generator1 = new IntegerObjectIdGenerator<PersonViewModel3>();
                IObjectIdGenerator<int> generator2 = new IntegerObjectIdGenerator<PersonViewModel4>();

                Assert.AreEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
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
                var generator = new IntegerObjectIdGenerator<PersonViewModel1>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel1());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.AreEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel1(), true));
            }

            [Test]
            public void Returns_A_Unique_Identifier_Even_When_An_Instance_Is_Released_But_Reuse_Is_False()
            {
                var generator = new IntegerObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel2());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.AreNotEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel2()));
            }

            [Test]
            public void Returns_A_Unique_Identifier_For_Different_Instances()
            {
                var generator = new IntegerObjectIdGenerator<PersonViewModel3>();
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

    public class LongObjectIdGeneratorFacts
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

        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
            [Test]
            public void Returns_New_UniqueIdentifier_Even_If_Are_Generated_By_Different_Instances()
            {
                IObjectIdGenerator<long> generator1 = new LongObjectIdGenerator<PersonViewModel1>();
                IObjectIdGenerator<long> generator2 = new LongObjectIdGenerator<PersonViewModel1>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<long> generator = new LongObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.AreEqual(uniqueIdentifier, generator.GetUniqueIdentifier(true));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DifferentTypes()
            {
                IObjectIdGenerator<long> generator1 = new LongObjectIdGenerator<PersonViewModel3>();
                IObjectIdGenerator<long> generator2 = new LongObjectIdGenerator<PersonViewModel4>();

                Assert.AreEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }
        }

        [TestFixture]
        public class GetUniqueIdentifierForInstance_Method
        {
            [Test]
            public void Returns_A_Released_Identifier_If_The_Instance_Is_Released_And_Reuse_Is_True()
            {
                var generator = new LongObjectIdGenerator<PersonViewModel1>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel1());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.AreEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel1(), true));
            }

            [Test]
            public void Returns_A_Unique_Identifier_Even_When_An_Instance_Is_Released_But_Reuse_Is_False()
            {
                var generator = new LongObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel2());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.AreNotEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel2()));
            }

            [Test]
            public void Returns_A_Unique_Identifier_For_Different_Instances()
            {
                var generator = new LongObjectIdGenerator<PersonViewModel3>();
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

    public class ULongObjectIdGeneratorFacts
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

        [TestFixture]
        public class The_GetUniqueIdentifier_Method
        {
            [Test]
            public void Returns_New_UniqueIdentifier_Even_If_Are_Generated_By_Different_Instances()
            {
                IObjectIdGenerator<ulong> generator1 = new ULongObjectIdGenerator<PersonViewModel1>();
                IObjectIdGenerator<ulong> generator2 = new ULongObjectIdGenerator<PersonViewModel1>();

                Assert.AreNotEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }

            [Test]
            public void Returns_A_Released_Identifier_If_Requested()
            {
                IObjectIdGenerator<ulong> generator = new ULongObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifier = generator.GetUniqueIdentifier();

                generator.ReleaseIdentifier(uniqueIdentifier);

                Assert.AreEqual(uniqueIdentifier, generator.GetUniqueIdentifier(true));
            }

            [Test]
            public void Returns_Unique_Identifier_For_DifferentTypes()
            {
                IObjectIdGenerator<ulong> generator1 = new ULongObjectIdGenerator<PersonViewModel3>();
                IObjectIdGenerator<ulong> generator2 = new ULongObjectIdGenerator<PersonViewModel4>();

                Assert.AreEqual(generator1.GetUniqueIdentifier(), generator2.GetUniqueIdentifier());
            }
        }

        [TestFixture]
        public class GetUniqueIdentifierForInstance_Method
        {
            [Test]
            public void Returns_A_Released_Identifier_If_The_Instance_Is_Released_And_Reuse_Is_True()
            {
                var generator = new ULongObjectIdGenerator<PersonViewModel1>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel1());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.AreEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel1(), true));
            }

            [Test]
            public void Returns_A_Unique_Identifier_Even_When_An_Instance_Is_Released_But_Reuse_Is_False()
            {
                var generator = new ULongObjectIdGenerator<PersonViewModel2>();
                var uniqueIdentifierForInstance = generator.GetUniqueIdentifierForInstance(new PersonViewModel2());

                GC.Collect();

                Assert.AreNotEqual(uniqueIdentifierForInstance, generator.GetUniqueIdentifierForInstance(new PersonViewModel2()));
            }

            [Test]
            public void Returns_A_Unique_Identifier_For_Different_Instances()
            {
                var generator = new ULongObjectIdGenerator<PersonViewModel3>();
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