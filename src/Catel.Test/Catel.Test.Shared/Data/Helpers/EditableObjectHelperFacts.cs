// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditableObjectHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Data
{
    using System.ComponentModel;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    using Moq;

    public class EditableObjectHelperFacts
    {
        [TestClass]
        public class TheBeginEditObjectMethod
        {
            [TestMethod]
            public void DoesNotCallBeginEditOnNullObject()
            {
                EditableObjectHelper.BeginEditObject(null);
            }

            [TestMethod]
            public void DoesNotCallBeginEditOnNonEditableObject()
            {
                var objectMock = new Mock<object>();
                EditableObjectHelper.BeginEditObject(objectMock.Object);                
            }

            [TestMethod]
            public void CallsBeginEditOnEditableObject()
            {
                var editableObjectMock = new Mock<IEditableObject>();

                EditableObjectHelper.BeginEditObject(editableObjectMock.Object);

                editableObjectMock.Verify(o => o.BeginEdit(), Times.Exactly(1));
            }
        }

        [TestClass]
        public class TheEndEditObjectMethod
        {
            [TestMethod]
            public void DoesNotCallEndEditOnNullObject()
            {
                EditableObjectHelper.BeginEditObject(null);
            }

            [TestMethod]
            public void DoesNotCallEndEditOnNonEditableObject()
            {
                var objectMock = new Mock<object>();
                EditableObjectHelper.EndEditObject(objectMock.Object);
            }

            [TestMethod]
            public void CallsEndEditOnEditableObject()
            {
                var editableObjectMock = new Mock<IEditableObject>();

                EditableObjectHelper.EndEditObject(editableObjectMock.Object);

                editableObjectMock.Verify(o => o.EndEdit(), Times.Exactly(1));
            }
        }

        [TestClass]
        public class TheCancelEditObjectMethod
        {
            [TestMethod]
            public void DoesNotCallCancelEditOnNullObject()
            {
                EditableObjectHelper.CancelEditObject(null);
            }

            [TestMethod]
            public void DoesNotCallCancelEditOnNonEditableObject()
            {
                var objectMock = new Mock<object>();
                EditableObjectHelper.CancelEditObject(objectMock.Object);
            }

            [TestMethod]
            public void CallsCancelEditOnEditableObject()
            {
                var editableObjectMock = new Mock<IEditableObject>();

                EditableObjectHelper.CancelEditObject(editableObjectMock.Object);

                editableObjectMock.Verify(o => o.CancelEdit(), Times.Exactly(1));
            }
        }
    }
}

#endif