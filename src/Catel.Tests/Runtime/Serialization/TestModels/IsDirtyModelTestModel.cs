namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    [Serializable]
    public class IsDirtyModelTestModel : SavableModelBase<IsDirtyModelTestModel>
    {
        #region Fields

        /// <summary>
        /// Register the MyInteger property so it is known in the class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "This is declared by the Catel MVVM framework.")]
        public static readonly PropertyData MyIntegerProperty = RegisterProperty("MyInteger", typeof(int));

        /// <summary>
        /// Register the MyDecimal property so it is known in the class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "This is declared by the Catel MVVM framework.")]
        public static readonly PropertyData MyDecimalProperty = RegisterProperty("MyDecimal", typeof(decimal));

        /// <summary>
        /// Register the MyString property so it is known in the class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "This is declared by the Catel MVVM framework.")]
        public static readonly PropertyData MyStringProperty = RegisterProperty("MyString", typeof(string));

        #endregion

        #region Constructors

        public IsDirtyModelTestModel()
        {
            // Create a new object from scratch
        }

        protected IsDirtyModelTestModel(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            // TBD
        }

        #endregion

        #region Properties

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

        #endregion
    }
}
