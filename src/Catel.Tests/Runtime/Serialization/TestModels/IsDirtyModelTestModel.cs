﻿namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    [Serializable]
    public class IsDirtyModelTestModel : SavableModelBase<IsDirtyModelTestModel>
    {
        /// <summary>
        /// Register the MyInteger property so it is known in the class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "This is declared by the Catel MVVM framework.")]
        public static readonly IPropertyData MyIntegerProperty = RegisterProperty<int>("MyInteger");

        /// <summary>
        /// Register the MyDecimal property so it is known in the class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "This is declared by the Catel MVVM framework.")]
        public static readonly IPropertyData MyDecimalProperty = RegisterProperty<decimal>("MyDecimal");

        /// <summary>
        /// Register the MyString property so it is known in the class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "This is declared by the Catel MVVM framework.")]
        public static readonly IPropertyData MyStringProperty = RegisterProperty<string>("MyString");

        public IsDirtyModelTestModel(ISerializer serializer)
            : base(serializer)
        {
            // Create a new object from scratch
        }

        /// <summary>
        /// Gets or sets the MyIntegerProperty value.
        /// </summary>
        public int MyInteger
        {
            get
            {
                return GetValue<int>(MyIntegerProperty);
            }

            set
            {
                SetValue(MyIntegerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MyDecimalProperty value.
        /// </summary>
        public decimal MyDecimal
        {
            get
            {
                return GetValue<decimal>(MyDecimalProperty);
            }

            set
            {
                SetValue(MyDecimalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MyString value.
        /// </summary>
        public string MyString
        {
            get
            {
                return GetValue<string>(MyStringProperty);
            }

            set
            {
                SetValue(MyStringProperty, value);
            }
        }
    }
}
