namespace Catel.Tests.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;
    using Catel.Data;
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    [TestFixture]
    public class ChildAwareModelBaseFacts
    {
        [TestCase]
        public void Allows_Registration_Of_ObservableCollection()
        {
            var model = new ModelWithObservableCollection();
            model.Collection = new ObservableCollection<int>(new List<int>() { 1, 2, 3 });

            model.Collection.Add(4);

            Assert.That(model.HasCollectionChanged, Is.True);
        }

        [TestCase]
        public void Allows_Registration_Of_CollectionViewSource()
        {
            var model = new ModelWithCollectionViewSource();
            model.Collection = new CollectionView(new List<int>() { 1, 2, 3 });
        }

        [TestCase]
        public void Registers_Change_Notifications_Of_Default_Values()
        {
            var model = new ModelWithObservableCollection();

            model.Collection.Add(4);

            Assert.That(model.HasCollectionChanged, Is.True);
        }

        [Test]
        public void Validates_Parent_On_CollectionChanged()
        {
            var c = new ValidatableChild();
            var p = new ValidatableParent();

            p.Collection = new ObservableCollection<ValidatableChild>();

            Assert.That(p.HasErrors, Is.False);
            Assert.That(c.HasErrors, Is.False);

            c.Name = string.Empty;
            p.Collection.Add(c);

            Assert.That(p.HasErrors, Is.True);
            Assert.That(c.HasErrors, Is.True);

            c.Name = "Funny";
            p.Collection.Clear();

            Assert.That(p.HasErrors, Is.False);
            Assert.That(c.HasErrors, Is.False);
        }

        [Test]
        public void Validates_Parent_On_CollectionItemPropertyChanged()
        {
            var c = new ValidatableChild();
            var p = new ValidatableParent();

            p.Collection = new ObservableCollection<ValidatableChild>();
            p.Collection.Add(c);

            Assert.That(p.HasErrors, Is.False);
            Assert.That(c.HasErrors, Is.False);

            c.Name = string.Empty;

            Assert.That(p.HasErrors, Is.True);
            Assert.That(c.HasErrors, Is.True);

            c.Name = "Bunny";

            Assert.That(p.HasErrors, Is.False);
            Assert.That(c.HasErrors, Is.False);
        }

        [Test]
        public void Validates_Parent_On_PropertyChanged()
        {
            var c = new ValidatableChild();
            var p = new ValidatableParent();

            p.Child = c;

            Assert.That(p.HasErrors, Is.False);
            Assert.That(c.HasErrors, Is.False);

            c.Name = string.Empty;

            Assert.That(p.HasErrors, Is.True);
            Assert.That(c.HasErrors, Is.True);

            c.Name = "Funny";

            Assert.That(p.HasErrors, Is.False);
            Assert.That(c.HasErrors, Is.False);
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

        [Test]
        public void Child_Changes_Propagate_To_GrandParent()
        {
            var c = new TestClasses.Child();
            var p = new TestClasses.Parent();
            var g = new GrandParent();
            g.Parents.Add(p);
            p.Children.Add(c);

            c.ResetDirtyFlag();
            p.ResetDirtyFlag();
            g.ResetDirtyFlag();

            Assert.That(c.IsDirty, Is.False);
            Assert.That(p.IsDirty, Is.False);
            Assert.That(g.IsDirty, Is.False);

            c.Name = "Pietje";

            Assert.That(c.IsDirty, Is.True);
            Assert.That(p.IsDirty, Is.True);
            Assert.That(g.IsDirty, Is.True);
        }

        [Test]
        public void Serializing_Delegates_Exception_Test()
        {
            var model = new TestModel();

            Assert.DoesNotThrow(() => (model as IEditableObject).BeginEdit());
        }

        private class TestModel : ChildAwareModelBase
        {
            public TestModel()
            {
                ChildProp = new ObservableObject();
            }

            public ObservableObject? ChildProp
            {
                get { return GetValue<ObservableObject?>(ChildPropProperty); }
                set { SetValue(ChildPropProperty, value); }
            }

            public static readonly IPropertyData ChildPropProperty = RegisterProperty<ObservableObject?>(nameof(ChildProp), (ObservableObject?)null);
        }
    }
}
