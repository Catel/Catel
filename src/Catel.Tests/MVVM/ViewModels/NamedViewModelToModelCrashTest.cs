namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using Catel.Data;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class NamedViewModelToModelCrashTest
    {

        public class Dummy : ModelBase
        {
            public Dummy()
            {
            }

            /// <summary>Register the Id property so it is known in the class.</summary>
            public static readonly IPropertyData IdProperty = RegisterProperty<Dummy, int>(model => model.Id);
            public static readonly IPropertyData CommentProperty = RegisterProperty<Dummy, string>(model => model.Comment);

            public Dummy(int id)
            {
                Id = id;
            }

            public int Id
            {
                get
                {
                    return GetValue<int>(IdProperty);
                }
                set
                {
                    SetValue(IdProperty, value);
                }
            }

            public string Comment
            {
                get
                {
                    return GetValue<string>(CommentProperty);
                }
                set
                {
                    SetValue(CommentProperty, value);
                }
            }

        }



        /// <summary>
        /// MainWindow view model.
        /// </summary>
        public class MainWindowViewModel : FeaturedViewModelBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
            /// </summary>
            public MainWindowViewModel(IServiceProvider serviceProvider)
                : base(serviceProvider)
            {
                CurrentDummy = new Dummy(444);
                Reset = new Command(serviceProvider, OnResetExecute);
                Create = new Command(serviceProvider, OnCreateExecute);
            }

            private void OnCreateExecute()
            {
                CurrentDummy = new Dummy(111);
            }

            private void OnResetExecute()
            {
                CurrentDummy = null;
            }

            /// <summary>
            /// Gets the title of the view model.
            /// </summary>
            /// <value>The title.</value>
            public override string Title { get { return "View model title"; } }

            /// <summary>Register the CurrentDummy property so it is known in the class.</summary>
            public static readonly IPropertyData CurrentDummyProperty = RegisterProperty<MainWindowViewModel, Dummy>(model => model.CurrentDummy);

            [Model]
            public Dummy CurrentDummy
            {
                get
                {
                    return GetValue<Dummy>(CurrentDummyProperty);
                }
                set
                {
                    SetValue(CurrentDummyProperty, value);
                }
            }

            /// <summary>Register the Id property so it is known in the class.</summary>
            public static readonly IPropertyData IdentifierProperty = RegisterProperty<MainWindowViewModel, int>(model => model.Identifier, 123);
            public static readonly IPropertyData CommentProperty = RegisterProperty<MainWindowViewModel, string>(model => model.Comment, "asd");

            [ViewModelToModel("CurrentDummy", "Id")]
            public int Identifier
            {
                get
                {
                    return GetValue<int>(IdentifierProperty);
                }
                set
                {
                    SetValue(IdentifierProperty, value);
                }
            }

            [ViewModelToModel("CurrentDummy")]
            public string Comment
            {
                get
                {
                    return GetValue<string>(CommentProperty);
                }
                set
                {
                    SetValue(CommentProperty, value);
                }
            }

            public Command Reset { get; private set; }
            public Command Create { get; private set; }
        }

        #region Methods
        [TestCase]
        public void OnSetModelToNullShouldSetDefaultValueForMappedProperties()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var vm = new MainWindowViewModel(serviceProvider);
            Assert.That(vm.Identifier, Is.EqualTo(444));

            vm.Reset.Execute();

            Assert.That(vm.Identifier, Is.EqualTo(123));
            Assert.That(vm.Comment, Is.EqualTo("asd"));
        }

        #endregion
    }
}
