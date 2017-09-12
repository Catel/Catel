// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildAwareModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Data;
    using NUnit.Framework;
    using TestClasses;

    [TestFixture]
    public class ChildAwareModelBaseFacts
    {
        [TestCase]
        public void AllowsRegistrationOfObservableCollection()
        {
            var model = new ModelWithObservableCollection();
            model.Collection = new ObservableCollection<int>(new List<int>() { 1, 2, 3 });

            model.Collection.Add(4);

            Assert.IsTrue(model.HasCollectionChanged);
        }

#if NET
        [TestCase]
        public void AllowsRegistrationOfCollectionViewSource()
        {
            var model = new ModelWithCollectionViewSource();
            model.Collection = new CollectionView(new List<int>() { 1, 2, 3 });
        }
#endif

        [TestCase]
        public void RegistersChangeNotificationsOfDefaultValues()
        {
            var model = new ModelWithObservableCollection();

            model.Collection.Add(4);

            Assert.IsTrue(model.HasCollectionChanged);
        }

        [TestCase]
        public void ValidatesChildAndParent()
        {
            var c = new ValidatableChild();
            var p = new ValidatableParent();

            p.Child = c;

            Assert.IsFalse(p.HasErrors);
            Assert.IsFalse(c.HasErrors);

            c.Name = string.Empty;

            Assert.IsTrue(p.HasErrors);
            Assert.IsTrue(c.HasErrors);

            c.Name = "Funny";

            Assert.IsFalse(p.HasErrors);
            Assert.IsFalse(c.HasErrors);
        }

        //[TestCase]
        //public void IsDirtyWithChildrenWhenSavingChild()
        //{
        //    // Create a collection
        //    var computerSettings = ModelBaseTestHelper.CreateComputerSettingsObject();
        //    computerSettings.SaveObjectToDummyMemoryStream(SerializationFactory.GetXmlSerializer());
        //    Assert.IsFalse(computerSettings.IsDirty);

        //    // Make a chance in the lowest level (but only if ObservableCollection, that is the only supported type)
        //    computerSettings.IniFileCollection[0].FileName = "is dirty should be enabled now";
        //    Assert.IsTrue(computerSettings.IniFileCollection[0].IsDirty);
        //    Assert.IsTrue(computerSettings.IsDirty);

        //    // Save the lowest level (so the parent stays dirty)
        //    computerSettings.IniFileCollection[0].IniEntryCollection[0].SaveObjectToDummyMemoryStream(SerializationFactory.GetXmlSerializer());
        //    Assert.IsFalse(computerSettings.IniFileCollection[0].IniEntryCollection[0].IsDirty);
        //    Assert.IsTrue(computerSettings.IsDirty);
        //}

        //[TestCase]
        //public void IsDirtyWithChildrenWhenSavingParent()
        //{
        //    // Create a collection
        //    var computerSettings = ModelBaseTestHelper.CreateComputerSettingsObject();
        //    computerSettings.SaveObjectToDummyMemoryStream(SerializationFactory.GetXmlSerializer());
        //    Assert.IsFalse(computerSettings.IsDirty);

        //    // Make a chance in the lowest level (but only if ObservableCollection, that is the only supported type)
        //    computerSettings.IniFileCollection[0].FileName = "is dirty should be enabled now 2";
        //    Assert.IsTrue(computerSettings.IniFileCollection[0].IsDirty);
        //    Assert.IsTrue(computerSettings.IsDirty);

        //    // Save the top level
        //    computerSettings.SaveObjectToDummyMemoryStream(SerializationFactory.GetXmlSerializer());
        //    Assert.IsFalse(computerSettings.IniFileCollection[0].IniEntryCollection[0].IsDirty);
        //    Assert.IsFalse(computerSettings.IsDirty);
        //}
    }
}