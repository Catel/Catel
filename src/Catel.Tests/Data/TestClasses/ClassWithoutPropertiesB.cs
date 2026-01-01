namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    public class ClassWithoutPropertiesB : ComparableModelBase
    {
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ClassWithoutPropertiesB()
            : base(new ModelEqualityComparer())
        {
        }
    }
}
