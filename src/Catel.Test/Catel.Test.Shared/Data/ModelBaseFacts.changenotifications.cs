// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.changenotifications.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using Catel.Data;
    using NUnit.Framework;
    using TestClasses;

    public partial class ModelBaseFacts
    {

        [TestFixture]
        public class SuspendChangeNotificationFacts
        {
            [TestCase]
            public void CorrectlyRaisesChangeNotifications()
            {
                var model = new SuspendableTestModel();

                // First change without change notifications so IsDirty = true
                model.FirstName = "1";

                var changedProperties = new List<string>();

                model.PropertyChanged += (sender, e) =>
                {
                    changedProperties.Add(e.PropertyName);
                };

                model.FirstName = "A";
                model.LastName = "B";

                Assert.AreEqual(2, changedProperties.Count);

                Assert.AreEqual("FirstName", changedProperties[0]);
                Assert.AreEqual("LastName", changedProperties[1]);
            }

            [TestCase]
            public void SuspendsChangeNotificationsAndRaisesPropertiesOnResume()
            {
                var model = new SuspendableTestModel();

                // First change without change notifications so IsDirty = true
                model.FirstName = "1";

                var changedProperties = new List<string>();

                model.PropertyChanged += (sender, e) =>
                {
                    changedProperties.Add(e.PropertyName);
                };

                using (model.SuspendChangeNotifications())
                {
                    model.FirstName = "A";
                    model.LastName = "B";

                    Assert.AreEqual(0, changedProperties.Count);
                }

                Assert.AreEqual(2, changedProperties.Count);

                Assert.AreEqual("FirstName", changedProperties[0]);
                Assert.AreEqual("LastName", changedProperties[1]);
            }

            [TestCase]
            public void SuspendsChangeNotificationsAndRaisesPropertiesOnResumeWithScopes()
            {
                var model = new SuspendableTestModel();

                // First change without change notifications so IsDirty = true
                model.FirstName = "1";

                var changedProperties = new List<string>();

                model.PropertyChanged += (sender, e) =>
                {
                    changedProperties.Add(e.PropertyName);
                };

                using (model.SuspendChangeNotifications())
                {
                    using (model.SuspendChangeNotifications())
                    {
                        model.FirstName = "A";
                        model.LastName = "B";

                        Assert.AreEqual(0, changedProperties.Count);
                    }

                    // We still haven't released all scopes
                    Assert.AreEqual(0, changedProperties.Count);
                }

                Assert.AreEqual(2, changedProperties.Count);

                Assert.AreEqual("FirstName", changedProperties[0]);
                Assert.AreEqual("LastName", changedProperties[1]);
            }
        }

        [TestFixture]
        public class SuspendChangeCallbacksFacts
        {
            [TestCase]
            public void CorrectlyInvokesCallbacks()
            {
                var model = new SuspendableTestModel();

                model.FirstName = "A";
                model.LastName = "B";

                Assert.IsTrue(model.IsFirstNameCallbackInvoked);
                Assert.IsTrue(model.IsLastNameCallbackInvoked);
            }

            [TestCase]
            public void SuspendsCallbacks()
            {
                var model = new SuspendableTestModel();

                using (model.SuspendChangeCallbacks())
                {
                    model.FirstName = "A";
                    model.LastName = "B";

                    Assert.IsFalse(model.IsFirstNameCallbackInvoked);
                    Assert.IsFalse(model.IsLastNameCallbackInvoked);
                }

                model.FirstName = "A1";
                model.LastName = "B1";

                Assert.IsTrue(model.IsFirstNameCallbackInvoked);
                Assert.IsTrue(model.IsLastNameCallbackInvoked);
            }

            [TestCase]
            public void SuspendsCallbacksWithScopes()
            {
                var model = new SuspendableTestModel();

                using (model.SuspendChangeCallbacks())
                {
                    using (model.SuspendChangeCallbacks())
                    {
                        model.FirstName = "A";
                        model.LastName = "B";

                        Assert.IsFalse(model.IsFirstNameCallbackInvoked);
                        Assert.IsFalse(model.IsLastNameCallbackInvoked);
                    }

                    model.FirstName = "A1";
                    model.LastName = "B1";

                    Assert.IsFalse(model.IsFirstNameCallbackInvoked);
                    Assert.IsFalse(model.IsLastNameCallbackInvoked);
                }

                model.FirstName = "A2";
                model.LastName = "B2";

                Assert.IsTrue(model.IsFirstNameCallbackInvoked);
                Assert.IsTrue(model.IsLastNameCallbackInvoked);
            }
        }
    }
}