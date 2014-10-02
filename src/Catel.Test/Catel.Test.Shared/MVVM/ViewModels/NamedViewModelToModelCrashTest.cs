namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using Catel.MVVM;

    using TestClasses;

    using NUnit.Framework;
    using Catel.Data;

    [TestFixture]
    public class NamedViewModelToModelCrashTest
    {

        public class Dummy : ModelBase
        {
            public Dummy()
            {
            }

            /// <summary>Register the Id property so it is known in the class.</summary>
            public static readonly PropertyData IdProperty = RegisterProperty<Dummy, int>(model => model.Id);

            public Dummy(int I)
            {
                Id = I;
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
        }



        /// <summary>
        /// MainWindow view model.
        /// </summary>
        public class MainWindowViewModel : ViewModelBase
        {
            #region Fields
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
            /// </summary>
            public MainWindowViewModel()
                : base()
            {
                CurrentDummy = new Dummy(444);
                Reset = new Command(OnResetExecute);
                Create = new Command(OnCreateExecute);
            }

            private void OnCreateExecute()
            {
                CurrentDummy = new Dummy(111);
            }

            private void OnResetExecute()
            {
                CurrentDummy = null;
            }

            #endregion

            #region Properties
            /// <summary>
            /// Gets the title of the view model.
            /// </summary>
            /// <value>The title.</value>
            public override string Title { get { return "View model title"; } }

            /// <summary>Register the CurrentDummy property so it is known in the class.</summary>
            public static readonly PropertyData CurrentDummyProperty = RegisterProperty<MainWindowViewModel, Dummy>(model => model.CurrentDummy);

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
            public static readonly PropertyData IdentifierProperty = RegisterProperty<MainWindowViewModel, int>(model => model.Identifier, 123);

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

            ///// <summary>Register the Id property so it is known in the class.</summary>
            //public static readonly PropertyData IdProperty = RegisterProperty<MainWindowViewModel, int>(model => model.Id, default(int));
            //
            //[ViewModelToModel("CurrentDummy")]
            //public int Id
            //{
            //    get {
            //        return GetValue<int>(IdProperty);
            //    }
            //    set {
            //        SetValue(IdProperty, value);
            //    }
            //}


            // TODO: Register models with the vmpropmodel codesnippet
            // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
            #endregion

            #region Commands

            public Command Reset { get; private set; }
            public Command Create { get; private set; }
            // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
            #endregion

            #region Methods
            // TODO: Create your methods here
            #endregion
        }
        #region Methods
        [TestCase]
        public void OnSetModelToNullShouldSetDefaultValueForMappedProperties()
        {
            var vm = new MainWindowViewModel();
            Assert.AreEqual(444, vm.Identifier);

            vm.Reset.Execute();

            Assert.AreEqual(123, vm.Identifier);
        }

        #endregion
    }
}