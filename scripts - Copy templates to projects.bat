:: This script copies the item and project templates to the template projects so
:: they can be uploaded to the visual studio gallery


:: =========================================================
:: ITEM TEMPLATES
:: =========================================================

:: General
copy /Y templates\C#\ItemTemplates\Catel.ViewModel.zip "src\Catel.Templates\Catel.ItemTemplates.ViewModel\ItemTemplates\CSharp\General"

:: WPF
copy /Y templates\C#\ItemTemplates\Catel.WPF.DataWindow.zip "src\Catel.Templates\Catel.ItemTemplates.WPF.DataWindow\ItemTemplates\CSharp\WPF"
copy /Y templates\C#\ItemTemplates\Catel.WPF.UserControl.zip "src\Catel.Templates\Catel.ItemTemplates.WPF.UserControl\ItemTemplates\CSharp\WPF"


:: =========================================================
:: PROJECT TEMPLATES
:: =========================================================

:: WPF
copy /Y templates\C#\ProjectTemplates\Catel.WPF.Application.zip "src\Catel.Templates\Catel.ProjectTemplates.WPF.Application\ProjectTemplates\CSharp\Windows"

pause