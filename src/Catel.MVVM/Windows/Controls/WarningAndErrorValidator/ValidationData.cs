namespace Catel.Windows.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using Collections;

    /// <summary>
    /// Class containing all validation info (thus warnings and errors) about a specific object.
    /// </summary>
    internal class ValidationData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationData"/> class.
        /// </summary>
        /// <param name="parentEnumerable">The parent ParentEnumerable. <c>Null</c> if the object does not belong to an enumerable.</param>
        public ValidationData(IEnumerable? parentEnumerable)
        {
            FieldWarnings = new List<FieldWarningOrErrorInfo>();
            BusinessWarnings = new List<BusinessWarningOrErrorInfo>();
            FieldErrors = new List<FieldWarningOrErrorInfo>();
            BusinessErrors = new List<BusinessWarningOrErrorInfo>();

            ParentEnumerable = parentEnumerable;
        }

        /// <summary>
        /// Gets or sets the parent enumerable.
        /// </summary>
        /// <value>The parent enumerable.</value>
        public IEnumerable? ParentEnumerable { get; private set; }

        /// <summary>
        /// Gets the field warnings.
        /// </summary>
        /// <value>The field warnings.</value>
        public List<FieldWarningOrErrorInfo> FieldWarnings { get; private set; }

        /// <summary>
        /// Gets the business warnings.
        /// </summary>
        /// <value>The business warnings.</value>
        public List<BusinessWarningOrErrorInfo> BusinessWarnings { get; private set; }

        /// <summary>
        /// Gets the field errors.
        /// </summary>
        /// <value>The field errors.</value>
        public List<FieldWarningOrErrorInfo> FieldErrors { get; private set; }

        /// <summary>
        /// Gets the business errors.
        /// </summary>
        /// <value>The business errors.</value>
        public List<BusinessWarningOrErrorInfo> BusinessErrors { get; private set; }

        /// <summary>
        /// Clears the warnings and errors.
        /// </summary>
        public void ClearWarningsAndErrors()
        {
            BusinessWarnings.Clear();
            FieldWarnings.Clear();
            BusinessErrors.Clear();
            FieldErrors.Clear();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var validationData = new ValidationData(ParentEnumerable);

            validationData.FieldWarnings = new List<FieldWarningOrErrorInfo>();
            ((ICollection<FieldWarningOrErrorInfo>)validationData.FieldWarnings).AddRange(FieldWarnings);

            validationData.BusinessWarnings = new List<BusinessWarningOrErrorInfo>();
            ((ICollection<BusinessWarningOrErrorInfo>)validationData.BusinessWarnings).AddRange(BusinessWarnings);

            validationData.FieldErrors = new List<FieldWarningOrErrorInfo>();
            ((ICollection<FieldWarningOrErrorInfo>)validationData.FieldErrors).AddRange(FieldErrors);

            validationData.BusinessErrors = new List<BusinessWarningOrErrorInfo>();
            ((ICollection<BusinessWarningOrErrorInfo>)validationData.BusinessErrors).AddRange(BusinessErrors);

            return validationData;
        }
    }
}
