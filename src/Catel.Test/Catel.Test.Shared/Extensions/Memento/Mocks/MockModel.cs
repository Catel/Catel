// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento.Mocks
{
    using Catel.Data;

    public class MockModel : ObservableObject
    {
        public static string Name { get; private set; }

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public static void Change(string value)
        {
            Name = value;
        }
    }
}