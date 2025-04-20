namespace Catel.Tests.Data
{
    using Catel.Data;

    public class ModelWithChangeHandler : ModelBase
    {
        public ModelWithChangeHandler()
        {
            B = "default value";
        }

        public int ChangeCount { get; private set; }

        public string B
        {
            get { return GetValue<string>(BProperty); }
            set { SetValue(BProperty, value); }
        }

        public static readonly IPropertyData BProperty = RegisterProperty("B", string.Empty,
            (sender, e) => ((ModelWithChangeHandler)sender).OnBChanged());

        private void OnBChanged()
        {
            ChangeCount++;
        }
    }
}
