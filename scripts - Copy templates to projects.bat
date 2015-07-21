:: This script copies the item and project templates to the template projects so
:: they can be uploaded to the visual studio gallery


:: =========================================================
:: ITEM TEMPLATES
:: =========================================================

:: General
copy /Y templates\C#\ItemTemplates\Catel.ViewModel.zip "src\Catel.Templates\Catel.ItemTemplates.ViewModel\ItemTemplates\CSharp\General"

:: Silverlight
copy /Y templates\C#\ItemTemplates\Catel.Silverlight.DataWindow.zip "src\Catel.Templates\Catel.ItemTemplates.Silverlight.DataWindow\ItemTemplates\CSharp\Silverlight"
copy /Y templates\C#\ItemTemplates\Catel.Silverlight.UserControl.zip "src\Catel.Templates\Catel.ItemTemplates.Silverlight.UserControl\ItemTemplates\CSharp\Silverlight"

:: Windows Phone
copy /Y templates\C#\ItemTemplates\Catel.WP.PortraitPage.zip "src\Catel.Templates\Catel.ItemTemplates.WindowsPhone.PortraitPage\ItemTemplates\CSharp\Windows Phone"
copy /Y templates\C#\ItemTemplates\Catel.WP.UserControl.zip "src\Catel.Templates\Catel.ItemTemplates.WindowsPhone.UserControl\ItemTemplates\CSharp\Windows Phone"

:: Windows Store
copy /Y templates\C#\ItemTemplates\Catel.WindowsStore.BlankPage.zip "src\Catel.Templates\Catel.ItemTemplates.WindowsStore.BlankPage\ItemTemplates\CSharp\Windows Store"
copy /Y templates\C#\ItemTemplates\Catel.WindowsStore.UserControl.zip "src\Catel.Templates\Catel.ItemTemplates.WindowsStore.UserControl\ItemTemplates\CSharp\Windows Store"

:: WPF
copy /Y templates\C#\ItemTemplates\Catel.WPF.DataWindow.zip "src\Catel.Templates\Catel.ItemTemplates.WPF.DataWindow\ItemTemplates\CSharp\WPF"
copy /Y templates\C#\ItemTemplates\Catel.WPF.UserControl.zip "src\Catel.Templates\Catel.ItemTemplates.WPF.UserControl\ItemTemplates\CSharp\WPF"


:: =========================================================
:: PROJECT TEMPLATES
:: =========================================================

:: Silverlight
copy /Y templates\C#\ProjectTemplates\Catel.Silverlight.Application.zip "src\Catel.Templates\Catel.ProjectTemplates.Silverlight.Application\ProjectTemplates\CSharp\Silverlight"

:: Windows Phone
copy /Y templates\C#\ProjectTemplates\Catel.WP80.Application.zip "src\Catel.Templates\Catel.ProjectTemplates.WindowsPhone.Application\ProjectTemplates\CSharp\Windows Phone"

:: Windows Store
copy /Y templates\C#\ProjectTemplates\Catel.WindowsStore.Application.zip "src\Catel.Templates\Catel.ProjectTemplates.WindowsStore.Application\ProjectTemplates\CSharp\Windows Store"

:: WPF
copy /Y templates\C#\ProjectTemplates\Catel.WPF.Application.zip "src\Catel.Templates\Catel.ProjectTemplates.WPF.Application\ProjectTemplates\CSharp\Windows"

pause