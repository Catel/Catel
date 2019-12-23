namespace Catel.Tests.Reflection.Models
{
    using System.Globalization;
    using Catel;

    public class RecordDetailItemValue
    {
        public RecordDetailItemValue(RecordDetailItem parent)
        {
            Parent = parent;
        }

        public RecordDetailItem Parent { get; private set; }

        public object Value { get; set; }

        public object Delta { get; set; }

        public bool IsDifferent { get; set; }

        public override string ToString()
        {
            return ObjectToStringHelper.ToString(Value, CultureInfo.CurrentCulture);
        }
    }
}
