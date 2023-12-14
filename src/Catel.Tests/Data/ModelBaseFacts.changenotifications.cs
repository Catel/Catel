namespace Catel.Tests.Data
{
    using System.Collections.Generic;
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

                Assert.That(changedProperties.Count, Is.EqualTo(2));

                Assert.That(changedProperties[0], Is.EqualTo("FirstName"));
                Assert.That(changedProperties[1], Is.EqualTo("LastName"));
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

                    Assert.That(changedProperties.Count, Is.EqualTo(0));
                }

                Assert.That(changedProperties.Count, Is.EqualTo(2));

                Assert.That(changedProperties[0], Is.EqualTo("FirstName"));
                Assert.That(changedProperties[1], Is.EqualTo("LastName"));
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

                        Assert.That(changedProperties.Count, Is.EqualTo(0));
                    }

                    // We still haven't released all scopes
                    Assert.That(changedProperties.Count, Is.EqualTo(0));
                }

                Assert.That(changedProperties.Count, Is.EqualTo(2));

                Assert.That(changedProperties[0], Is.EqualTo("FirstName"));
                Assert.That(changedProperties[1], Is.EqualTo("LastName"));
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

                Assert.That(model.IsFirstNameCallbackInvoked, Is.True);
                Assert.That(model.IsLastNameCallbackInvoked, Is.True);
            }

            [TestCase]
            public void SuspendsCallbacks()
            {
                var model = new SuspendableTestModel();

                using (model.SuspendChangeCallbacks())
                {
                    model.FirstName = "A";
                    model.LastName = "B";

                    Assert.That(model.IsFirstNameCallbackInvoked, Is.False);
                    Assert.That(model.IsLastNameCallbackInvoked, Is.False);
                }

                model.FirstName = "A1";
                model.LastName = "B1";

                Assert.That(model.IsFirstNameCallbackInvoked, Is.True);
                Assert.That(model.IsLastNameCallbackInvoked, Is.True);
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

                        Assert.That(model.IsFirstNameCallbackInvoked, Is.False);
                        Assert.That(model.IsLastNameCallbackInvoked, Is.False);
                    }

                    model.FirstName = "A1";
                    model.LastName = "B1";

                    Assert.That(model.IsFirstNameCallbackInvoked, Is.False);
                    Assert.That(model.IsLastNameCallbackInvoked, Is.False);
                }

                model.FirstName = "A2";
                model.LastName = "B2";

                Assert.That(model.IsFirstNameCallbackInvoked, Is.True);
                Assert.That(model.IsLastNameCallbackInvoked, Is.True);
            }
        }
    }
}