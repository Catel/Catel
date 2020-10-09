namespace Catel.Tests.CTL1669
{
    using Catel.Data;
    using Catel.MVVM;
    using NUnit.Framework;

    [TestFixture]
    internal class TestFixture
    {
        [Test]
        public void ViewModelInit_DoesNotThrows()
        {
            Assert.DoesNotThrow(() =>
            {
                var dogModel = new DogModel
                {
                    Name = "name"
                };

                var vm = new DogViewModel(dogModel);

                Assert.AreEqual(vm.Name, dogModel.Name);
            });
        }
    }

    public abstract class AnimalModelBase : ModelBase
    {
    }

    public class DogModel : AnimalModelBase
    {
        public string Name
        {
            get => GetValue<string>(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string));
    }

    public abstract class AnimalViewModelBase : ViewModelBase
    {
        public AnimalViewModelBase(AnimalModelBase model)
        {
            Animal = model;
        }

        [Model]
        public AnimalModelBase Animal
        {
            get => GetValue<AnimalModelBase>(AnimalProperty);
            set => SetValue(AnimalProperty, value);
        }

        public static readonly PropertyData AnimalProperty = RegisterProperty(nameof(Animal), typeof(AnimalModelBase));
    }

    public class DogViewModel : AnimalViewModelBase
    {
        public DogViewModel(DogModel model)
            : base(model)
        {
        }

        [ViewModelToModel(nameof(Animal), nameof(DogModel.Name))]
        public string Name
        {
            get => GetValue<string>(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string));
    }
}
