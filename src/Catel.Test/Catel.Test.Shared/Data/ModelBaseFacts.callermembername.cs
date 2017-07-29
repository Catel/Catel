// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.callermembername.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using NUnit.Framework;

    public partial class ModelBaseFacts
    {
        [TestFixture]
        public class CallerMemberName_Usage
        {
            [Test]
            public void Correctly_Raise_Change()
            {
                bool notified = false;
                var person = new PersonModelUsingCallerMemberName();
                person.PropertyChanged += (sender, args) =>
                {
                    if (nameof(person.Name) == args.PropertyName)
                    {
                        notified = true;
                    }
                };

                person.Name = "Alex";
                Assert.IsTrue(notified);
            }

            [Test]
            public void Correctly_Get_The_Value()
            {
                var person = new PersonModelUsingCallerMemberName();
                var expectedValue = "Alex";
                person.Name = expectedValue;
                Assert.AreEqual(expectedValue, person.Name);
            }
        }
    }
}
