// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento.Mocks
{
    using Catel.Memento;

    public class MockUndo : IMementoSupport
    {
        public bool UndoCalled, RedoCalled;

        public MockUndo(bool canRedo = false, object target = null)
        {
            Target = target;
            CanRedo = canRedo;
        }

        public object Target { get; private set; }

        /// <summary>
        /// Gets or sets the description, which is option and can be used to display a text to the end-user.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        public object Tag { get; set; }

        public void Undo()
        {
            UndoCalled = true;
        }

        public bool CanRedo { get; private set; }

        public void Redo()
        {
            RedoCalled = true;
        }

        public int Value { get; set; }
    }
}