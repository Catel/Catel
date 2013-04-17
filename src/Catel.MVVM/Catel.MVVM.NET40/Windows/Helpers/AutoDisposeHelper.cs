// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoDisposeHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Collections;
    using System.Windows;
    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
	/// Auto diposes the properties that have this attribute when the property is declared.
	/// </summary>
	/// <remarks>
	/// The attribute itself only defines itself, but has no real implementation. The supported object that allows
	/// to use this object should implement the actual disposing.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property)]
	public class AutoDisposeAttribute : Attribute
	{
	}

	/// <summary>
	/// Auto dispose helper class.
	/// </summary>
	public static class AutoDisposeHelper
	{
		/// <summary>
		/// Registers the AutoDisposeProperties call to the Unloaded event of the given element.
		/// </summary>
		/// <param name="element">The element.</param>
		public static void EnableAutoDisposeProperties(this FrameworkElement element)
		{
			if (element != null)
			{
				element.Unloaded -= UnloadedWrapper;
				element.Unloaded += UnloadedWrapper;
			}
		}

		/// <summary>
		/// Handles the AutoDisposeProperties call.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private static void UnloadedWrapper(object sender, RoutedEventArgs e)
		{
			AutoDisposeProperties(sender);

			var element = sender as FrameworkElement;
			if (element != null)
			{
				element.Unloaded -= UnloadedWrapper;
			}
		}

		/// <summary>
		/// Automatically disposes all properties of the object decorated with the <see cref="AutoDisposeAttribute"/>.
		/// </summary>
		/// <param name="obj">The object to dispose the properties of.</param>
		public static void AutoDisposeProperties(object obj)
		{
			if (obj == null)
			{
			    return;
			}

			var properties = obj.GetType().GetPropertiesEx();
			foreach (var propertyInfo in properties)
			{
				if (propertyInfo.GetCustomAttributeEx(typeof(AutoDisposeAttribute), true) != null)
				{
					object value = propertyInfo.GetValue(obj, null);
					AutoDisposeObject(value);
				}
			}
		}

		/// <summary>
		/// Automatically disposes an object. If the object is an enumerable, all childs are disposed.
		/// </summary>
		/// <param name="obj">The object to dispose.</param>
		private static void AutoDisposeObject(object obj)
		{
			if (obj == null)
			{
			    return;
			}

		    var objAsIEnumerable = obj as IEnumerable;
            if (objAsIEnumerable != null)
			{
                foreach (object childObj in objAsIEnumerable)
				{
					AutoDisposeObject(childObj);
				}
			}

		    var objAsIDiposable = obj as IDisposable;
            if (objAsIDiposable!= null)
			{
                objAsIDiposable.Dispose();
			}
		}
	}
}
