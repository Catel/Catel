namespace Catel.Tests.Data
{
    using Catel.Data;

    public class ClassWithPropertiesRegisteredByNonMagicStringOverload : ModelBase
    {
        public static readonly IPropertyData StringPropertyProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, string>(instance => instance.StringProperty);

        public static readonly IPropertyData StringPropertyWithSpecifiedDefaultValueProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, string>(instance => instance.StringPropertyWithSpecifiedDefaultValue, "NonNullOrEmptyDefaultValue");

        public static readonly IPropertyData IntPropertyWithPropertyChangeNoticationProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, int>(instance => instance.IntPropertyWithPropertyChangeNotification, default(int), (s, e) => s.OnIntPropertyWithPropertyChangeNotificationChanged());

        public static readonly IPropertyData IntPropertyExcludedFromSerializationAndBackupProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, int>(instance => instance.IntPropertyExcludedFromSerializationAndBackup, default(int), null, false, false);

        public int IntPropertyExcludedFromSerializationAndBackup
        {
            get { return GetValue<int>(IntPropertyExcludedFromSerializationAndBackupProperty); }
            set { SetValue(IntPropertyExcludedFromSerializationAndBackupProperty, value); }
        }

        public string StringPropertyWithSpecifiedDefaultValue
        {
            get { return GetValue<string>(StringPropertyWithSpecifiedDefaultValueProperty); }
            set { SetValue(StringPropertyWithSpecifiedDefaultValueProperty, value); }
        }

        public string StringProperty
        {
            get { return GetValue<string>(StringPropertyProperty); }
            set { SetValue(StringPropertyProperty, value); }
        }

        public int IntPropertyWithPropertyChangeNotification
        {
            get { return GetValue<int>(IntPropertyWithPropertyChangeNoticationProperty); }
            set { SetValue(IntPropertyWithPropertyChangeNoticationProperty, value); }
        }

        public int IntPropertyWithPropertyChangeNotificationsCount { get; private set; }

        private void OnIntPropertyWithPropertyChangeNotificationChanged()
        {
            IntPropertyWithPropertyChangeNotificationsCount++;
        }
    }
}
