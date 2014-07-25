namespace Catel.MVVM.ViewModels
{
    class ViewModelToModelCopyConverter : ViewModelToModelConverter
    {
        public ViewModelToModelCopyConverter(string propertyName) : base(propertyName)
        {
        }

        public override bool CanConvert(object value, IViewModel viewModel)
        {
            return true;
        }

        public override object Convert(object value, IViewModel viewModel)
        {
            return value;
        }

        public override bool CanConvertBack(object value, IViewModel viewModel)
        {
            return true;
        }

        public override object ConvertBack(object value, IViewModel viewModel)
        {
            return value;
        }
    }
}