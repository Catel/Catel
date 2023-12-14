namespace Catel.Tests.Data
{
    using System.ComponentModel;
    using Catel.Data;

    using NUnit.Framework;

    using Moq;

    public class EditableObjectHelperFacts
    {
        [TestFixture]
        public class TheBeginEditObjectMethod
        {
            [TestCase]
            public void DoesNotCallBeginEditOnNullObject()
            {
                EditableObjectHelper.BeginEditObject(null);
            }

            [TestCase]
            public void DoesNotCallBeginEditOnNonEditableObject()
            {
                var objectMock = new Mock<object>();
                EditableObjectHelper.BeginEditObject(objectMock.Object);
            }

            [TestCase]
            public void CallsBeginEditOnEditableObject()
            {
                var editableObjectMock = new Mock<IEditableObject>();

                EditableObjectHelper.BeginEditObject(editableObjectMock.Object);

                editableObjectMock.Verify(o => o.BeginEdit(), Times.Exactly(1));
            }
        }

        [TestFixture]
        public class TheEndEditObjectMethod
        {
            [TestCase]
            public void DoesNotCallEndEditOnNullObject()
            {
                EditableObjectHelper.BeginEditObject(null);
            }

            [TestCase]
            public void DoesNotCallEndEditOnNonEditableObject()
            {
                var objectMock = new Mock<object>();
                EditableObjectHelper.EndEditObject(objectMock.Object);
            }

            [TestCase]
            public void CallsEndEditOnEditableObject()
            {
                var editableObjectMock = new Mock<IEditableObject>();

                EditableObjectHelper.EndEditObject(editableObjectMock.Object);

                editableObjectMock.Verify(o => o.EndEdit(), Times.Exactly(1));
            }
        }

        [TestFixture]
        public class TheCancelEditObjectMethod
        {
            [TestCase]
            public void DoesNotCallCancelEditOnNullObject()
            {
                EditableObjectHelper.CancelEditObject(null);
            }

            [TestCase]
            public void DoesNotCallCancelEditOnNonEditableObject()
            {
                var objectMock = new Mock<object>();
                EditableObjectHelper.CancelEditObject(objectMock.Object);
            }

            [TestCase]
            public void CallsCancelEditOnEditableObject()
            {
                var editableObjectMock = new Mock<IEditableObject>();

                EditableObjectHelper.CancelEditObject(editableObjectMock.Object);

                editableObjectMock.Verify(o => o.CancelEdit(), Times.Exactly(1));
            }
        }
    }
}
