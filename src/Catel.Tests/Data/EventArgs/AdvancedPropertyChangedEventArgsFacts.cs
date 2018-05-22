// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedPropertyChangedEventArgsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Data
{
    using Catel.Data;

    using NUnit.Framework;

    [TestFixture]
    public class AdvancedPropertyChangedEventArgsFacts
    {
        #region Nested type: TheConstructor
        [TestFixture]
        public class TheConstructor
        {
            #region Methods
            [TestCase]
            public void SetsValuesCorrectlyForNullPropertyName()
            {
                var iniEntry = new IniEntry();
                var nullEventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, (string) null);

                Assert.AreEqual(null, nullEventArgs.PropertyName);
            }

            [TestCase]
            public void SetsValuesCorrectlyForEmptyPropertyName()
            {
                var iniEntry = new IniEntry();
                var nullEventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, string.Empty);

                Assert.AreEqual(string.Empty, nullEventArgs.PropertyName);
            }

            [TestCase]
            public void SetsValuesCorrectlyWithTwoArguments()
            {
                var iniEntry = new IniEntry();
                var iniFile = new IniFile();

                var originalEventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, "PropertyName");

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniFile, originalEventArgs);

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniFile, eventArgs.LatestSender);
                Assert.AreEqual("PropertyName", eventArgs.PropertyName);
            }

            [TestCase]
            public void SetsValuesCorrectlyWithTheeArgumentsExceptOldValue()
            {
                var iniEntry = new IniEntry();

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, "PropertyName", (object) "new value");

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniEntry, eventArgs.LatestSender);
                Assert.AreEqual("PropertyName", eventArgs.PropertyName);
                Assert.AreEqual(null, eventArgs.OldValue);
                Assert.AreEqual("new value", eventArgs.NewValue);
                Assert.AreEqual(false, eventArgs.IsOldValueMeaningful);
                Assert.AreEqual(true, eventArgs.IsNewValueMeaningful);
            }

            [TestCase]
            public void SetsValuesCorrectlyWithAllARguments()
            {
                var iniEntry = new IniEntry();

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, "PropertyName", (object) "old value", "new value");

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniEntry, eventArgs.LatestSender);
                Assert.AreEqual("PropertyName", eventArgs.PropertyName);
                Assert.AreEqual("old value", eventArgs.OldValue);
                Assert.AreEqual("new value", eventArgs.NewValue);
                Assert.AreEqual(true, eventArgs.IsOldValueMeaningful);
                Assert.AreEqual(true, eventArgs.IsNewValueMeaningful);
            }

            [TestCase]
            public void SetsValuesCorrectlyWithPropertyNameOnly()
            {
                var iniEntry = new IniEntry();
                var iniFile = new IniFile();

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, iniFile, "PropertyName");

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniFile, eventArgs.LatestSender);
                Assert.AreEqual("PropertyName", eventArgs.PropertyName);
                Assert.AreEqual(null, eventArgs.OldValue);
                Assert.AreEqual(null, eventArgs.NewValue);
                Assert.AreEqual(false, eventArgs.IsOldValueMeaningful);
                Assert.AreEqual(false, eventArgs.IsNewValueMeaningful);
            }

            [TestCase]
            public void CanAutomaticallyDetectNewValue()
            {
                var iniEntry = new IniEntry();
                iniEntry.Key = "mykey";

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, "Key");

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniEntry, eventArgs.LatestSender);
                Assert.AreEqual("Key", eventArgs.PropertyName);
                Assert.AreEqual(null, eventArgs.OldValue);
                Assert.AreEqual("mykey", eventArgs.NewValue);
                Assert.AreEqual(false, eventArgs.IsOldValueMeaningful);
                Assert.AreEqual(true, eventArgs.IsNewValueMeaningful);
            }
            #endregion
        }
        #endregion
    }
}