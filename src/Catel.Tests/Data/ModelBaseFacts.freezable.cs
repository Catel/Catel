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
            Assert.That(model.FirstName, Is.EqualTo("John"));
            Assert.That(freezable.IsFrozen, Is.False);

            freezable.Freeze();

            model.FirstName = "Jane";
            Assert.That(model.FirstName, Is.EqualTo("John"));
            Assert.That(freezable.IsFrozen, Is.True);

            freezable.Unfreeze();

            model.FirstName = "Jane";
            Assert.That(model.FirstName, Is.EqualTo("Jane"));
            Assert.That(freezable.IsFrozen, Is.False);
        }
    }
}
