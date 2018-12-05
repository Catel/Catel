namespace Catel.Tests.Data
{
    using Catel.Data;
    using NUnit.Framework;

    public partial class ModelBaseFacts
    {
        [TestCase]
        public void TheFreezeImplementation()
        {
            var model = new PersonTestModel();
            var freezable = (IFreezable)model;

            model.FirstName = "John";
            Assert.AreEqual("John", model.FirstName);
            Assert.IsFalse(freezable.IsFrozen);

            freezable.Freeze();

            model.FirstName = "Jane";
            Assert.AreEqual("John", model.FirstName);
            Assert.IsTrue(freezable.IsFrozen);

            freezable.Unfreeze();

            model.FirstName = "Jane";
            Assert.AreEqual("Jane", model.FirstName);
            Assert.IsFalse(freezable.IsFrozen);
        }
    }
}
