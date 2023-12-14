namespace Catel.Tests.MVVM.ViewModels
{
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public void GetAllModels_With_Null()
        {
            var person = new Person();
            person.FirstName = "first_name";
            person.LastName = "last_name";

            var viewModel = new TestViewModel(person);

            var models = viewModel.GetAllModelsForTest();

            Assert.That(models.Length, Is.EqualTo(1));
            Assert.That(models[0], Is.EqualTo(person));
        }

        [TestCase]
        public void GetAllModels()
        {
            var person = new Person();
            person.FirstName = "first_name";
            person.LastName = "last_name";

            var viewModel = new TestViewModel(person);

            var specialValidationModel = new SpecialValidationModel();
            viewModel.SpecialValidationModel = specialValidationModel;

            var models = viewModel.GetAllModelsForTest();

            Assert.That(models.Length, Is.EqualTo(2));
            Assert.That(models[0], Is.EqualTo(person));
            Assert.That(models[1], Is.EqualTo(specialValidationModel));
        }

        [TestCase]
        public async Task ModelsSavedBySaveAsync()
        {
            var person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var model = person as IModel;
            var viewModel = new TestViewModel(person);
            Assert.That(model.IsInEditSession, Is.True);

            viewModel.FirstName = "new";

            await viewModel.SaveAndCloseViewModelAsync();

            Assert.That(model.IsInEditSession, Is.False);
            Assert.That(person.FirstName, Is.EqualTo("new"));
        }

        [TestCase]
        public async Task ModelsCanceledByCancelAsync()
        {
            var person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var model = person as IModel;
            var viewModel = new TestViewModel(person);
            Assert.That(model.IsInEditSession, Is.True);

            viewModel.FirstName = "new first name";

            await viewModel.CancelAndCloseViewModelAsync();

            Assert.That(model.IsInEditSession, Is.False);
            Assert.That(person.FirstName, Is.EqualTo("first name"));
        }

        [TestCase]
        public void IsModelRegistered_ExistingModel()
        {
            var person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel(person);

            Assert.That(viewModel.IsModelRegisteredForTest("Person"), Is.True);
        }

        [TestCase]
        public void IsModelRegistered_NonExistingModel()
        {
            var person = new Person();
            person.FirstName = "first_name";
            person.LastName = "last_name";

            var viewModel = new TestViewModel(person);

            Assert.That(viewModel.IsModelRegisteredForTest("SecondPerson"), Is.False);
        }

    }
}
