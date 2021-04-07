// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.models.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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
        public void GetAllModels()
        {
            var person = new Person();
            person.FirstName = "first_name";
            person.LastName = "last_name";

            var viewModel = new TestViewModel(person);

            var models = viewModel.GetAllModelsForTest();

            Assert.AreEqual(2, models.Length);
            Assert.AreEqual(person, models[0]);
        }

        [TestCase]
        public async Task ModelsSavedBySaveAsync()
        {
            var person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var model = person as IModel;
            var viewModel = new TestViewModel(person);
            Assert.IsTrue(model.IsInEditSession);

            viewModel.FirstName = "new";

            await viewModel.SaveAndCloseViewModelAsync();

            Assert.IsFalse(model.IsInEditSession);
            Assert.AreEqual("new", person.FirstName);
        }

        [TestCase]
        public async Task ModelsCanceledByCancelAsync()
        {
            var person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var model = person as IModel;
            var viewModel = new TestViewModel(person);
            Assert.IsTrue(model.IsInEditSession);

            viewModel.FirstName = "new first name";

            await viewModel.CancelAndCloseViewModelAsync();

            Assert.IsFalse(model.IsInEditSession);
            Assert.AreEqual("first name", person.FirstName);
        }

        [TestCase]
        public void IsModelRegistered_ExistingModel()
        {
            var person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel(person);

            Assert.IsTrue(viewModel.IsModelRegisteredForTest("Person"));
        }

        [TestCase]
        public void IsModelRegistered_NonExistingModel()
        {
            var person = new Person();
            person.FirstName = "first_name";
            person.LastName = "last_name";

            var viewModel = new TestViewModel(person);

            Assert.IsFalse(viewModel.IsModelRegisteredForTest("SecondPerson"));
        }

    }
}