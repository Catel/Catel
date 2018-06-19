// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System;
    using Catel.Runtime.Serialization;

    public class NonCatelTestModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class NonCatelTestModelWithIFieldSerializable : IFieldSerializable
    {
        private bool _getViaInterface;
        private bool _setViaInterface;

        public string _firstName;

        public string _lastName;

        bool IFieldSerializable.GetFieldValue(string fieldName, ref object value)
        {
            if (fieldName == "_firstName")
            {
                _getViaInterface = true;
                value = FirstName;
                return true;
            }

            if (fieldName == "_lastName")
            {
                _getViaInterface = true;
                value = LastName;
                return true;
            }

            return false;
        }

        bool IFieldSerializable.SetFieldValue(string fieldName, object value)
        {
            if (fieldName == "_firstName")
            {
                FirstName = value as string;
                _setViaInterface = true;
                return true;
            }

            if (fieldName == "_lastName")
            {
                LastName = value as string;
                _setViaInterface = true;
                return true;
            }

            return false;
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public bool GetViaInterface { get { return _getViaInterface; } }

        public bool SetViaInterface { get { return _setViaInterface; } }
    }

    public class NonCatelTestModelWithIPropertySerializable : IPropertySerializable
    {
        private bool _getViaInterface;
        private bool _setViaInterface;

        bool IPropertySerializable.GetPropertyValue(string propertyName, ref object value)
        {
            if (propertyName == "FirstName")
            {
                _getViaInterface = true;
                value = FirstName;
                return true;
            }

            if (propertyName == "LastName")
            {
                _getViaInterface = true;
                value = LastName;
                return true;
            }

            throw new NotSupportedException();
        }

        bool IPropertySerializable.SetPropertyValue(string propertyName, object value)
        {
            if (propertyName == "FirstName")
            {
                FirstName = value as string;
                _setViaInterface = true;
                return true;
            }

            if (propertyName == "LastName")
            {
                LastName = value as string;
                _setViaInterface = true;
                return true;
            }

            return false;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool GetViaInterface { get { return _getViaInterface; } }

        public bool SetViaInterface { get { return _setViaInterface; } }
    }
}
