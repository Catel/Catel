namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;
    using NUnit.Framework;

    public partial class ModelBaseFacts
    {
        public class NonConstructableEditableObject : EditableObject
        {
            public NonConstructableEditableObject(string firstName)
            {
                FirstName = firstName;
            }


            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            public static readonly IPropertyData FirstNameProperty = RegisterProperty(nameof(FirstName), string.Empty);

        }

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
            public static readonly IPropertyData IgnoredPropertyInBackupProperty = RegisterProperty<int>("IgnoredPropertyInBackup", 42, null, true, false);

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

            public static readonly IPropertyData NameProperty = RegisterProperty<string>("Name", default(string));

            public ClearIsDirtyModel()
            {

            }

            internal void ClearIsDirty()
            {
                IsDirty = false;
            }
        }

        [TestFixture]
        public class BoxingFeature
        {
            [TestCase]
            public void CorrectlyCachesBoxedValues()
            {
                var model = new PersonTestModel();

                var isEnabled1 = ((IModel)model).GetValue<object>(nameof(PersonTestModel.IsEnabled));
                var isEnabled2 = ((IModel)model).GetValue<object>(nameof(PersonTestModel.IsEnabled));

                Assert.That(ReferenceEquals(isEnabled1, isEnabled2), Is.True);

                model.SetValue(nameof(PersonTestModel.IsEnabled), true);

                isEnabled1 = ((IModel)model).GetValue<object>(nameof(PersonTestModel.IsEnabled));
                isEnabled2 = ((IModel)model).GetValue<object>(nameof(PersonTestModel.IsEnabled));

                Assert.That(ReferenceEquals(isEnabled1, isEnabled2), Is.True);
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

                Assert.That(editableObject.BeginEditingCalled, Is.False);
                Assert.That(editableObject.OnBeginEditCalled, Is.False);

                editableObjectAsIEditableObject.BeginEdit();

                Assert.That(editableObject.BeginEditingCalled, Is.True);
                Assert.That(editableObject.OnBeginEditCalled, Is.True);
            }
        }

        [TestFixture]
        public class TheCancelEditMethod
        {
            [TestCase]
            public void CancelsChangesCorrectlyForSimpleTypes()
            {
                CancelEdit(() => ModelBaseTestHelper.CreateIniEntryObject(),
                    x => x.Value = "MyOldValue",
                    x => x.Value = "MyNewValue",
                    x => Assert.That(x.Value, Is.EqualTo("MyOldValue")));
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

                Assert.That(obj.Gender, Is.EqualTo(Gender.Female));
            }

            [TestCase]
            public void CancelsChangesForSelfReferencingTypes()
            {
                //Assert.Inconclusive("Fix in 3.1");
            }

            [TestCase]
            public void WorksForNonConstructableEditableObject()
            {
                var editableObject = new NonConstructableEditableObject("Geert");
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                editableObjectAsIEditableObject.BeginEdit();

                editableObject.FirstName = "John";

                Assert.That(editableObject.FirstName, Is.EqualTo("John"));

                editableObjectAsIEditableObject.CancelEdit();

                Assert.That(editableObject.CancelEditingCalled, Is.EqualTo(true));
                Assert.That(editableObject.FirstName, Is.EqualTo("Geert"));
            }

            [TestCase]
            public void DoesNotInvokeCancelEditingEventWhenBeginEditWasNotCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.That(editableObject.CancelEditingCalled, Is.False);
                Assert.That(editableObject.OnCancelEditCalled, Is.False);

                editableObjectAsIEditableObject.CancelEdit();

                Assert.That(editableObject.CancelEditingCalled, Is.False);
                Assert.That(editableObject.CancelEditingCompletedCalled, Is.False);
                Assert.That(editableObject.OnCancelEditCalled, Is.False);
                Assert.That(editableObject.OnCancelEditCompletedCalled, Is.False);
            }

            [TestCase]
            public void InvokesCancelEditingEventAfterBeginEditIsCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.That(editableObject.CancelEditingCalled, Is.False);
                Assert.That(editableObject.OnCancelEditCalled, Is.False);

                editableObjectAsIEditableObject.BeginEdit();
                editableObjectAsIEditableObject.CancelEdit();

                Assert.That(editableObject.CancelEditingCalled, Is.True);
                Assert.That(editableObject.CancelEditingCompletedCalled, Is.True);
                Assert.That(editableObject.OnCancelEditCalled, Is.True);
                Assert.That(editableObject.OnCancelEditCompletedCalled, Is.True);
            }

            [TestCase]
            public void InvokesCancelEditingCompletedEventAfterCancelEditIsCanceled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                editableObject.DoCancelCancel = true;

                Assert.That(editableObject.CancelEditingCalled, Is.False);
                Assert.That(editableObject.OnCancelEditCalled, Is.False);

                editableObjectAsIEditableObject.BeginEdit();
                editableObjectAsIEditableObject.CancelEdit();

                Assert.That(editableObject.CancelEditingCalled, Is.True);
                Assert.That(editableObject.CancelEditingCompletedCalled, Is.True);
                Assert.That(editableObject.OnCancelEditCalled, Is.True);
                Assert.That(editableObject.OnCancelEditCompletedCalled, Is.True);
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

                Assert.That(editableObject.IgnoredPropertyInBackup, Is.EqualTo(2));
            }

            public void CancelEdit<TModel>(Func<TModel> createModel, Action<TModel> beforeBeginEdit, Action<TModel> afterBeginEdit, Action<TModel> assert)
                where TModel : ModelBase
            {
                var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

                var serializers = new List<ISerializer>();
                serializers.Add(dependencyResolver.Resolve<XmlSerializer>());
                serializers.Add(dependencyResolver.Resolve<JsonSerializer>());

                foreach (var serializer in serializers)
                {
                    var model = createModel();
                    model._editableObjectSerializer = serializer;

                    var modelAsIEditableObject = (IEditableObject)model;

                    beforeBeginEdit(model);

                    modelAsIEditableObject.BeginEdit();

                    afterBeginEdit(model);

                    modelAsIEditableObject.CancelEdit();

                    assert(model);
                }
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

                Assert.That(iniEntry.Value, Is.EqualTo("MyNewValue"));
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

                Assert.That(obj.Gender, Is.EqualTo(Gender.Male));
            }

            [TestCase]
            public void AppliesChangesForSelfReferencingTypes()
            {
                //Assert.Inconclusive("Fix in 3.1");
            }

            [TestCase]
            public void DoesNotInvokeEndEditingEventWhenBeginEditWasNotCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.That(editableObject.EndEditingCalled, Is.False);
                Assert.That(editableObject.OnEndEditCalled, Is.False);

                editableObjectAsIEditableObject.EndEdit();

                Assert.That(editableObject.EndEditingCalled, Is.False);
                Assert.That(editableObject.OnEndEditCalled, Is.False);
            }

            [TestCase]
            public void InvokesEndEditingEventAfterBeginEditIsCalled()
            {
                var editableObject = new EditableObject();
                var editableObjectAsIEditableObject = (IEditableObject)editableObject;

                Assert.That(editableObject.EndEditingCalled, Is.False);
                Assert.That(editableObject.OnEndEditCalled, Is.False);

                editableObjectAsIEditableObject.BeginEdit();
                editableObjectAsIEditableObject.EndEdit();

                Assert.That(editableObject.EndEditingCalled, Is.True);
                Assert.That(editableObject.OnEndEditCalled, Is.True);
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

                Assert.That(isDirtyChangedCalls, Is.EqualTo(4));
            }
        }
    }
}
