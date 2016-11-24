// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeanAndMeanModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;

    public class LeanAndMeanModel : ModelBase
    {
        public bool LeanAndMeanModelWrapper
        {
            get { return LeanAndMeanModel; }
            set { LeanAndMeanModel = value; }
        }

        [Required]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);
    }
}