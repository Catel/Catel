// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Extensions.DynamicObjects
{
    using System;
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;

    public class DynamicObservableObjectFacts
    {
        public class CustomObject : DynamicObservableObject
        {
        }

        [TestFixture]
        public class TheGetValueAndSetValueProperties
        {
            [TestCase]
            public void CorrectlyReturnsTheRightValue_WhenSetViaDynamicProperty()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Settings value via dynamic property, getting via dynamic property and via GetValue<T> method.
                DateTime dt = DateTime.ParseExact("2016-01-01 01:01:01", "yyyy-MM-dd HH:mm:ss", null);
                dynamicObservableObject.Property1 = "test";
                dynamicObservableObject.Property2 = 100;
                dynamicObservableObject.Property3 = 3.14F;
                dynamicObservableObject.Property4 = 1.2M;
                dynamicObservableObject.Property5 = dt;

                Assert.AreEqual("test", dynamicObservableObject.Property1);
                Assert.AreEqual(100, dynamicObservableObject.Property2);
                Assert.AreEqual(3.14F, dynamicObservableObject.Property3);
                Assert.AreEqual(1.2M, dynamicObservableObject.Property4);
                Assert.AreEqual(dt, dynamicObservableObject.Property5);

                Assert.AreEqual("test", observableObject.GetValue<string>("Property1"));
                Assert.AreEqual(100, observableObject.GetValue<int>("Property2"));
                Assert.AreEqual(3.14F, observableObject.GetValue<float>("Property3"));
                Assert.AreEqual(1.2M, observableObject.GetValue<decimal>("Property4"));
                Assert.AreEqual(dt, observableObject.GetValue<DateTime>("Property5"));
            }

            [TestCase]
            public void CorrectlyReturnsTheRightValue_WhenSetViaSetValueMethod()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via SetValue method, getting via dynamic property and via GetValue<T> method.
                DateTime dt = DateTime.ParseExact("2016-01-01 01:01:01", "yyyy-MM-dd HH:mm:ss", null);
                observableObject.SetValue("Property1", "test");
                observableObject.SetValue("Property2", 100);
                observableObject.SetValue("Property3", 3.14F);
                observableObject.SetValue("Property4", 1.2M);
                observableObject.SetValue("Property5", dt);

                Assert.AreEqual("test", dynamicObservableObject.Property1);
                Assert.AreEqual(100, dynamicObservableObject.Property2);
                Assert.AreEqual(3.14F, dynamicObservableObject.Property3);
                Assert.AreEqual(1.2M, dynamicObservableObject.Property4);
                Assert.AreEqual(dt, dynamicObservableObject.Property5);

                Assert.AreEqual("test", observableObject.GetValue<string>("Property1"));
                Assert.AreEqual(100, observableObject.GetValue<int>("Property2"));
                Assert.AreEqual(3.14F, observableObject.GetValue<float>("Property3"));
                Assert.AreEqual(1.2M, observableObject.GetValue<decimal>("Property4"));
                Assert.AreEqual(dt, observableObject.GetValue<DateTime>("Property5"));
            }

            [TestCase]
            public void RaisesAdvancedPropertyChangingEvents_WhenSetViaDynamicProperty()
            {
                var counter = 0;
                var propertyName = default(string);
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via dynamic property.
                dynamicObservableObject.Property1 = "oldtest";
                observableObject.PropertyChanging += (sender, e) =>
                {
                    AdvancedPropertyChangingEventArgs args = e as AdvancedPropertyChangingEventArgs;
                    if (args != null)
                    {
                        counter++;
                        propertyName = args.PropertyName;
                    }
                };
                dynamicObservableObject.Property1 = "newtest";

                Assert.AreEqual(1, counter);
                Assert.AreEqual(propertyName, "Property1");
            }

            [TestCase]
            public void CorrectlyReturnsTheDefaultValue_WhenNotSet()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                Assert.AreEqual(null, observableObject.GetValue<string>("Property1"));
                Assert.AreEqual(0, observableObject.GetValue<int>("Property2"));
                Assert.AreEqual(0F, observableObject.GetValue<float>("Property3"));
                Assert.AreEqual(0M, observableObject.GetValue<decimal>("Property4"));
                Assert.AreEqual(DateTime.MinValue, observableObject.GetValue<DateTime>("Property5"));
                //
                Assert.AreEqual(null, observableObject.GetValue<string>("Property1"));
                Assert.AreEqual(null, observableObject.GetValue<int?>("Property2"));
                Assert.AreEqual(null, observableObject.GetValue<float?>("Property3"));
                Assert.AreEqual(null, observableObject.GetValue<decimal?>("Property4"));
                Assert.AreEqual(null, observableObject.GetValue<DateTime?>("Property5"));
            }

            [TestCase]
            public void RaisesAdvancedPropertyChangingEvents_WhenSetViaSetValueMethod()
            {
                var counter = 0;
                var propertyName = default(string);
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via SetValue method.
                observableObject.SetValue("Property1", "oldtest");
                observableObject.PropertyChanging += (sender, e) =>
                {
                    AdvancedPropertyChangingEventArgs args = e as AdvancedPropertyChangingEventArgs;
                    if (args != null)
                    {
                        counter++;
                        propertyName = args.PropertyName;
                    }
                };
                observableObject.SetValue("Property1", "newtest");

                Assert.AreEqual(1, counter);
                Assert.AreEqual(propertyName, "Property1");
            }

            [TestCase]
            public void RaisesAdvancedPropertyChangedEvents_WhenSetViaDynamicProperty()
            {
                var counter = 0;
                var propertyName = default(string);
                var oldValue = default(object);
                var newValue = default(object);
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via dynamic property.
                dynamicObservableObject.Property1 = "oldtest";
                observableObject.PropertyChanged += (sender, e) =>
                {
                    AdvancedPropertyChangedEventArgs args = e as AdvancedPropertyChangedEventArgs;
                    if (args != null)
                    {
                        counter++;
                        propertyName = args.PropertyName;
                        oldValue = args.OldValue;
                        newValue = args.NewValue;
                    }
                };
                dynamicObservableObject.Property1 = "newtest";

                Assert.AreEqual(1, counter);
                Assert.AreEqual(propertyName, "Property1");
                Assert.AreEqual(oldValue, "oldtest");
                Assert.AreEqual(newValue, "newtest");
            }

            [TestCase]
            public void RaisesAdvancedPropertyChangedEvents_WhenSetViaSetValueMethod()
            {
                var counter = 0;
                var propertyName = default(string);
                var oldValue = default(object);
                var newValue = default(object);
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via dynamic property.
                observableObject.SetValue("Property1", "oldtest");
                observableObject.PropertyChanged += (sender, e) =>
                {
                    AdvancedPropertyChangedEventArgs args = e as AdvancedPropertyChangedEventArgs;
                    if (args != null)
                    {
                        counter++;
                        propertyName = args.PropertyName;
                        oldValue = args.OldValue;
                        newValue = args.NewValue;
                    }
                };
                observableObject.SetValue("Property1", "newtest");

                Assert.AreEqual(1, counter);
                Assert.AreEqual(propertyName, "Property1");
                Assert.AreEqual(oldValue, "oldtest");
                Assert.AreEqual(newValue, "newtest");
            }

            [TestCase]
            public void ThrowsArgumentExceptionWhenPropertyNameIsNullOrWhitespace_WhenSetViaSetValueMethod()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                Assert.Throws<ArgumentException>(() => observableObject.SetValue(null, "test"));
                Assert.Throws<ArgumentException>(() => observableObject.SetValue("", "test"));
                Assert.Throws<ArgumentException>(() => observableObject.SetValue(" ", "test"));
            }
        }
    }
}
