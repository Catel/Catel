Catel project template readme
=============================

Congratulations with creating a new Catel project:

$safeprojectname$

To get this project up and running, perform the following actions:

1) Right-click on the project => Manage NuGet packages...
2) Search for Catel.Extensions.Controls
3) Install the NuGet package
4) Build and run the application

Note that this project template assumes that you are using Catel 4.x.

For more information and support, visit http://www.catelproject.com



Automatically transforming regular properties to Catel properties
-----------------------------------------------------------------

To automatically transform a regular property into a Catel property, use
the following NuGet package:

* Catel.Fody

The following property definition:


	public string Name { get; set; }

	
will be weaved into:


	public string Name
	{
		get { return GetValue<string>(NameProperty); }
		set { SetValue(NameProperty, value); }
	}
	 
	public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));

	
For more information, visit http://catelproject.com/tools/catel-fody/
	
	
	
ReSharper support
-----------------

Catel provides a ReSharper plugin to ease the development of Catel. 

For more information, visit http://catelproject.com/tools/catel-resharper/