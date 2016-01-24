// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.editableobject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using System.ComponentModel;
    using Catel.Data;

    using NUnit.Framework;

    public partial class ModelBaseFacts
    {
        public class EditableObject : ModelBase
        {
            public EditableObject()
            {
                var advancedEditableObject = (IAdvancedEditableObject)this;
                advancedEditableObject.BeginEditing += (sender, e) => BeginEditingCalled = true;
                advancedEditableObject.CancelEditing += (sender, e) =>
                {
                    e.Cancel = DoCancelCancel;
                    CancelEditingCalled = true;
                };
                advancedEditableObject.CancelEditingCompleted += (sender, e) => CancelEditingCompletedCalled = true;
                advancedEditableObject.EndEditing += (sender, e) => EndEditingCalled = true;
            }

            public bool OnBeginEditCalled
            {
                get { return _onBeginEditCalled; }
                private set { _onBeginEditCalled = value; }
            }

            public bool BeginEditingCalled
            {
                get { return _beginEditingCalled; }
                private set { _beginEditingCalled = value; }
            }

            protected override void OnBeginEdit(BeginEditEventArgs e)
            {
                OnBeginEditCalled = true;
            }

            public bool OnCancelEditCalled
            {
                get { return _onCancelEditCalled; }
                private set { _onCancelEditCalled = value; }
            }

            public bool OnCancelEditCompletedCalled
            {
                get;
                private set;
            }

            public bool CancelEditingCalled
            {
                get { return _cancelEditingCalled; }
                private set { _cancelEditingCalled = value; }
            }

            public bool CancelEditingCompletedCalled
            {
                get;
                private set;
            }

            protected override void OnCancelEdit(EditEventArgs e)
            {
                OnCancelEditCalled = true;
            }

            protected override void OnCancelEditCompleted(CancelEditCompletedEventArgs e)
            {
                OnCancelEditCompletedCalled = true;
            }

            public bool DoCancelCancel { get; set; }

            public bool OnEndEditCalled
            {
                get { return _onEndEditCalled; }
                private set { _onEndEditCalled = value; }
            }

            public bool EndEditingCalled
            {
                get { return _endEditingCalled; }
                private set { _endEditingCalled = value; }
            }

            protected override void OnEndEdit(EditEventArgs e)
            {
                OnEndEditCalled = true;
            }

            /// <summary>
            /// Gets or sets the ignored property.
            /// </summary>
            public int IgnoredPropertyInBackup
            {
                get { return GetValue<int>(IgnoredPropertyInBackupProperty); }
                set { SetValue(IgnoredPropertyInBackupProperty, value); }
            }

            /// <summary>
            /// Register the IgnoredPropertyInBackup property so it is known in the class.
            /// </summary>
            public static readonly PropertyData IgnoredPropertyInBackupProperty = RegisterProperty("IgnoredPropertyInBackup", typeof(int), 42,
                null, true, false);

            private bool _onBeginEditCalled;
            private bool _beginEditingCalled;
            private bool _onEndEditCalled;
            private bool _endEditingCalled;
            private bool _onCancelEditCalled;
            private bool _cancelEditingCalled;
        }

        public class ClearIsDirtyModel : ModelBase
        {
            public string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), default(string));

            public ClearIsDirtyModel()
            {

            }

            internal void ClearIsDirty()
            {
                IsDirty = false;
            }
        }

        [TestFixture]
        public class TheBeginEditMethod
        {
            [TestCase]
            public void AllowsDoubleCalls()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                editableObjectAsIEditableObject.BeginEdit();
                editableObjectAsIEditableObject.BeginEdit();
            }

            [TestCase]
            public void InvokesBeginEditingEvent()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.IsFalse(editableObject.BeginEditingCalled);
                Assert.IsFalse(editableObject.OnBeginEditCalled);

                editableObjectAsIEditableObject.BeginEdit();

                Assert.IsTrue(editableObject.BeginEditingCalled);
                Assert.IsTrue(editableObject.OnBeginEditCalled);
            }
        }

        [TestFixture]
        public class TheCancelEditMethod
        {
            [TestCase]
            public void CancelsChangesCorrectlyForSimpleTypes()
            {
                var iniEntry = ModelBaseTestHelper.CreateIniEntryObject();
                var iniEntryAsIEditableObject = (IEditableObject)iniEntry;

                iniEntry.Value = "MyOldValue";

                iniEntryAsIEditableObject.BeginEdit();

                iniEntry.Value = "MyNewValue";

                iniEntryAsIEditableObject.CancelEdit();

                Assert.AreEqual("MyOldValue", iniEntry.Value);
            }

            [TestCase]
            public void CancelsChangesCorrectlyForObjectWithCustomType()
            {
                var obj = new ObjectWithCustomType();
                var objEntryAsIEditableObject = (IEditableObject)obj;

                obj.Gender = Gender.Female;

                objEntryAsIEditableObject.BeginEdit();

                obj.Gender = Gender.Male;

                objEntryAsIEditableObject.CancelEdit();

                Assert.AreEqual(Gender.Female, obj.Gender);
            }

            [TestCase]
            public void CancelsChangesForSelfReferencingTypes()
            {
                //Assert.Inconclusive("Fix in 3.1");
            }

            [TestCase]
            public void DoesNotInvokeCancelEditingEventAfterBeginEditIsCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.IsFalse(editableObject.CancelEditingCalled);
                Assert.IsFalse(editableObject.OnCancelEditCalled);

                editableObjectAsIEditableObject.CancelEdit();

                Assert.IsFalse(editableObject.CancelEditingCalled);
                Assert.IsFalse(editableObject.CancelEditingCompletedCalled);
                Assert.IsFalse(editableObject.OnCancelEditCalled);
                Assert.IsFalse(editableObject.OnCancelEditCompletedCalled);
            }

            [TestCase]
            public void InvokesCancelEditingEventAfterBeginEditIsCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.IsFalse(editableObject.CancelEditingCalled);
                Assert.IsFalse(editableObject.OnCancelEditCalled);

                editableObjectAsIEditableObject.BeginEdit();
                editableObjectAsIEditableObject.CancelEdit();

                Assert.IsTrue(editableObject.CancelEditingCalled);
                Assert.IsTrue(editableObject.CancelEditingCompletedCalled);
                Assert.IsTrue(editableObject.OnCancelEditCalled);
                Assert.IsTrue(editableObject.OnCancelEditCompletedCalled);
            }

            [TestCase]
            public void InvokesCancelEditingCompletedEventAfterCancelEditIsCanceled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                editableObject.DoCancelCancel = true;

                Assert.IsFalse(editableObject.CancelEditingCalled);
                Assert.IsFalse(editableObject.OnCancelEditCalled);

                editableObjectAsIEditableObject.BeginEdit();
                editableObjectAsIEditableObject.CancelEdit();

                Assert.IsTrue(editableObject.CancelEditingCalled);
                Assert.IsTrue(editableObject.CancelEditingCompletedCalled);
                Assert.IsTrue(editableObject.OnCancelEditCalled);
                Assert.IsTrue(editableObject.OnCancelEditCompletedCalled);
            }

            [TestCase]
            public void IgnoresPropertiesNotInBackup()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                editableObject.IgnoredPropertyInBackup = 1;

                editableObjectAsIEditableObject.BeginEdit();

                editableObject.IgnoredPropertyInBackup = 2;

                editableObjectAsIEditableObject.CancelEdit();

                Assert.AreEqual(2, editableObject.IgnoredPropertyInBackup);
            }
        }

        [TestFixture]
        public class TheEndEditMethod
        {
            [TestCase]
            public void AppliesChangesCorrectlyForSimpleTypes()
            {
                var iniEntry = ModelBaseTestHelper.CreateIniEntryObject();
                var iniEntryAsIEditableObject = (IEditableObject)iniEntry;

                iniEntry.Value = "MyOldValue";

                iniEntryAsIEditableObject.BeginEdit();

                iniEntry.Value = "MyNewValue";

                iniEntryAsIEditableObject.EndEdit();

                Assert.AreEqual("MyNewValue", iniEntry.Value);
            }

            [TestCase]
            public void AppliesChangesCorrectlyForObjectWithCustomType()
            {
                var obj = new ObjectWithCustomType();
                var objAsIEditableObject = (IEditableObject)obj;

                obj.Gender = Gender.Female;

                objAsIEditableObject.BeginEdit();

                obj.Gender = Gender.Male;

                ((IEditableObject)obj).EndEdit();

                Assert.AreEqual(Gender.Male, obj.Gender);
            }

            [TestCase]
            public void AppliesChangesForSelfReferencingTypes()
            {
                //Assert.Inconclusive("Fix in 3.1");
            }

            [TestCase]
            public void DoesNotInvokeEndEditingEventAfterBeginEditIsCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.IsFalse(editableObject.EndEditingCalled);
                Assert.IsFalse(editableObject.OnEndEditCalled);

                editableObjectAsIEditableObject.EndEdit();

                Assert.IsFalse(editableObject.EndEditingCalled);
                Assert.IsFalse(editableObject.OnEndEditCalled);
            }

            [TestCase]
            public void InvokesEndEditingEventAfterBeginEditIsCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.IsFalse(editableObject.EndEditingCalled);
                Assert.IsFalse(editableObject.OnEndEditCalled);

                editableObjectAsIEditableObject.BeginEdit();
                editableObjectAsIEditableObject.EndEdit();

                Assert.IsTrue(editableObject.EndEditingCalled);
                Assert.IsTrue(editableObject.OnEndEditCalled);
            }
        }

        [TestFixture]
        public class TheClearIsDirtyMethod
        {
            [TestCase]
            public void CorrectlyRaisesPropertyChangedForIsDirty()
            {
                int isDirtyChangedCalls = 0;
                var model = new ClearIsDirtyModel();

                ((INotifyPropertyChanged)model).PropertyChanged += (sender, e) =>
                {
                    if (string.Equals(e.PropertyName, "IsDirty"))
                    {
                        isDirtyChangedCalls++;
                    }
                };

                ((IEditableObject)model).BeginEdit();

                // IsDirty change 1
                model.Name = "Me";

                // IsDirty change 2
                model.ClearIsDirty();

                // IsDirty change 3 + 4 (Name change back to null, and restoreof IsDirty)
                ((IEditableObject)model).CancelEdit();

                Assert.AreEqual(4, isDirtyChangedCalls);
            }
        }
    }
}