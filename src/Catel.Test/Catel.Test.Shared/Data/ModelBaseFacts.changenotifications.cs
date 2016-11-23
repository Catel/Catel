// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.changenotifications.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using NUnit.Framework;

    public partial class ModelBaseFacts
    {
        [TestFixture]
        public class ChangeNotificationFacts
        {
            [TestCase]
            public void CorrectlyRaisesChangeNotifications()
            {
                var model = new PersonTestModel();

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
                var model = new PersonTestModel();

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
                var model = new PersonTestModel();

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
    }
}