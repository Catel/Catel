// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedPropertyChangedEventArgsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class AdvancedPropertyChangedEventArgsFacts
    {
        #region Nested type: TheConstructor
        [TestClass]
        public class TheConstructor
        {
            #region Methods
            [TestMethod]
            public void SetsValuesCorrectlyForNullPropertyName()
            {
                var iniEntry = new IniEntry();
                var nullEventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, (string) null);

                Assert.AreEqual(null, nullEventArgs.PropertyName);
            }

            [TestMethod]
            public void SetsValuesCorrectlyForEmptyPropertyName()
            {
                var iniEntry = new IniEntry();
                var nullEventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, string.Empty);

                Assert.AreEqual(string.Empty, nullEventArgs.PropertyName);
            }

            [TestMethod]
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

            [TestMethod]
            public void SetsValuesCorrectlyWithTheeArgumentsExceptOldValue()
            {
                IniEntry iniEntry = new IniEntry();

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, "PropertyName", (object) "new value");

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniEntry, eventArgs.LatestSender);
                Assert.AreEqual("PropertyName", eventArgs.PropertyName);
                Assert.AreEqual(null, eventArgs.OldValue);
                Assert.AreEqual("new value", eventArgs.NewValue);
                Assert.AreEqual(false, eventArgs.IsOldValueMeaningful);
                Assert.AreEqual(true, eventArgs.IsNewValueMeaningful);
            }

            [TestMethod]
            public void SetsValuesCorrectlyWithAllARguments()
            {
                IniEntry iniEntry = new IniEntry();

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, "PropertyName", (object) "old value", "new value");

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniEntry, eventArgs.LatestSender);
                Assert.AreEqual("PropertyName", eventArgs.PropertyName);
                Assert.AreEqual("old value", eventArgs.OldValue);
                Assert.AreEqual("new value", eventArgs.NewValue);
                Assert.AreEqual(true, eventArgs.IsOldValueMeaningful);
                Assert.AreEqual(true, eventArgs.IsNewValueMeaningful);
            }

            [TestMethod]
            public void SetsValuesCorrectlyWithPropertyNameOnly()
            {
                IniEntry iniEntry = new IniEntry();
                IniFile iniFile = new IniFile();

                var eventArgs = new AdvancedPropertyChangedEventArgs(iniEntry, iniFile, "PropertyName");

                Assert.AreEqual(iniEntry, eventArgs.OriginalSender);
                Assert.AreEqual(iniFile, eventArgs.LatestSender);
                Assert.AreEqual("PropertyName", eventArgs.PropertyName);
                Assert.AreEqual(null, eventArgs.OldValue);
                Assert.AreEqual(null, eventArgs.NewValue);
                Assert.AreEqual(false, eventArgs.IsOldValueMeaningful);
                Assert.AreEqual(false, eventArgs.IsNewValueMeaningful);
            }

            [TestMethod]
            public void CanAutomaticallyDetectNewValue()
            {
                IniEntry iniEntry = new IniEntry();
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