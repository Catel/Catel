// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialValidationModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class SpecialValidationModel
#if !WINDOWS_PHONE
        : INotifyDataErrorInfo, INotifyDataWarningInfo
#endif
    {
        private readonly Dictionary<string, List<string>> _fieldErrors = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, List<string>> _fieldWarnings = new Dictionary<string, List<string>>();

        private readonly List<string> _businessRuleErrors = new List<string>();
        private readonly List<string> _businessRuleWarnings = new List<string>();

        private string _fieldErrorWhenEmpty = "noerror";

        public string FieldErrorWhenEmpty
        {
            get { return _fieldErrorWhenEmpty; }
            set
            {
                if (!_fieldErrors.ContainsKey("FieldErrorWhenEmpty"))
                {
                    _fieldErrors.Add("FieldErrorWhenEmpty", new List<string>());
                }

                _fieldErrors["FieldErrorWhenEmpty"].Clear();

                _fieldErrorWhenEmpty = value;

                if (string.IsNullOrEmpty(_fieldErrorWhenEmpty))
                {
                    _fieldErrors["FieldErrorWhenEmpty"].Add("Field error");
                }

                RaiseErrorsChanged("FieldErrorWhenEmpty");
            }
        }

        private string _fieldWarningWhenEmpty = "nowarning";

        public string FieldWarningWhenEmpty
        {
            get { return _fieldWarningWhenEmpty; }
            set
            {
                if (!_fieldWarnings.ContainsKey("FieldWarningWhenEmpty"))
                {
                    _fieldWarnings.Add("FieldWarningWhenEmpty", new List<string>());
                }

                _fieldWarnings["FieldWarningWhenEmpty"].Clear();

                _fieldWarningWhenEmpty = value;

                if (string.IsNullOrEmpty(_fieldWarningWhenEmpty))
                {
                    _fieldWarnings["FieldWarningWhenEmpty"].Add("Field warning");
                }

                RaiseWarningsChanged("FieldWarningWhenEmpty");
            }
        }

        private string _businessRuleErrorWhenEmpty = "noerror";

        public string BusinessRuleErrorWhenEmpty
        {
            get { return _businessRuleErrorWhenEmpty; }
            set
            {
                _businessRuleErrors.Clear();

                _businessRuleErrorWhenEmpty = value;

                if (string.IsNullOrEmpty(_businessRuleErrorWhenEmpty))
                {
                    _businessRuleErrors.Add("Business rule error");
                }

                RaiseErrorsChanged(string.Empty);
            }
        }

        private string _businessRuleWarningWhenEmpty = "nowarning";

        public string BusinessRuleWarningWhenEmpty
        {
            get { return _businessRuleWarningWhenEmpty; }
            set
            {
                _businessRuleWarnings.Clear();

                _businessRuleWarningWhenEmpty = value;

                if (string.IsNullOrEmpty(_businessRuleWarningWhenEmpty))
                {
                    _businessRuleWarnings.Add("Business rule warning");
                }

                RaiseWarningsChanged(string.Empty);
            }
        }

        public bool HasErrors
        {
            get { throw new NotImplementedException("No need to implement this for testing"); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(propertyName))
            {
                errors.AddRange(_businessRuleErrors);
            }
            else
            {
                if (_fieldErrors.ContainsKey(propertyName))
                {
                    errors.AddRange(_fieldErrors[propertyName]);
                }
            }

            return errors;
        }

        public bool HasWarnings
        {
            get { throw new NotImplementedException("No need to implement this for testing"); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> WarningsChanged;

        public IEnumerable GetWarnings(string propertyName)
        {
            List<string> warnings = new List<string>();

            if (string.IsNullOrEmpty(propertyName))
            {
                warnings.AddRange(_businessRuleWarnings);
            }
            else
            {
                if (_fieldWarnings.ContainsKey(propertyName))
                {
                    warnings.AddRange(_fieldWarnings[propertyName]);
                }
            }

            return warnings;
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void RaiseWarningsChanged(string propertyName)
        {
            if (WarningsChanged != null)
            {
                WarningsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}