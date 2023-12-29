namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System.ComponentModel;
    using Catel.Data;

    public class PersonObservableObject : ObservableObject, IPerson
    {
        private int _firstNameChangedCounter;
        private string _firstName;

        public int FirstNameChangedCounter { get => _firstNameChangedCounter; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnFirstNamePropertyChanged();
                RaisePropertyChanged(nameof(FirstName));
            }
        }

        private void OnFirstNamePropertyChanged()
        {
            _firstNameChangedCounter++;
        }

        public string MiddleName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string LastName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint Age { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IContactInfo ContactInfo { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
