// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyObjectExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Media;

    using IoC;

	using Microsoft.Practices.Prism.Regions;

	/// <summary>
	/// The dependency object extensions.
	/// </summary>
	public static class DependencyObjectExtensions
	{
		/// <summary>
		/// The region info.
		/// </summary>
		private class RegionInfo : IRegionInfo
		{
			#region Fields

			/// <summary>
			/// The dependency object.
			/// </summary>
			private readonly DependencyObject _dependencyObject;
			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="RegionInfo"/> class.
			/// </summary>
			/// <param name="dependencyObject">
			/// The dependency object.
			/// </param>
			/// <exception cref="System.ArgumentNullException">
			/// The <paramref name="dependencyObject"/> is <c>null</c>.
			/// </exception>
			public RegionInfo(DependencyObject dependencyObject)
			{
				Argument.IsNotNull("dependencyObject", dependencyObject);

				_dependencyObject = dependencyObject;
			}

			#endregion

			#region IRegionInfo Members

			/// <summary>
			/// Gets RegionName.
			/// </summary>
			string IRegionInfo.RegionName
			{
				get
				{
					return _dependencyObject.GetRegionName();
				}
			}

			/// <summary>
			/// Gets RegionManager.
			/// </summary>
			IRegionManager IRegionInfo.RegionManager
			{
				get
				{
					return _dependencyObject.GetRegionManager();
				}
			}
			#endregion
		}

		#region Methods

		/// <summary>
		/// Gets the region name.
		/// </summary>
		/// <param name="this">
		/// The <see cref="DependencyObject"/> instance.
		/// </param>
		/// <returns>
		/// The region name.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="this"/> is <c>null</c>.
		/// </exception>
		public static string GetRegionName(this DependencyObject @this)
		{
			Argument.IsNotNull("@this", @this);

			return RegionManager.GetRegionName(@this);
		}

		/// <summary>
		/// Gets the region manager.
		/// </summary>
		/// <param name="this">
		/// The <see cref="DependencyObject"/> instance.
		/// </param>
		/// <returns>
		/// The region manager.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="this"/> is <c>null</c>.
		/// </exception>
		public static IRegionManager GetRegionManager(this DependencyObject @this)
		{
			Argument.IsNotNull("@this", @this);

			return RegionManager.GetRegionManager(@this);
		}

		/// <summary>
		/// Sets the region manager.
		/// </summary>
		/// <param name="this">
		/// The <see cref="DependencyObject"/> instance.
		/// </param>
		/// <param name="regionManager">
		/// The region manager.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="this"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="regionManager"/> is <c>null</c>.
		/// </exception>
		public static void SetRegionManager(this DependencyObject @this, IRegionManager regionManager)
		{
			Argument.IsNotNull("@this", @this);
			Argument.IsNotNull("regionManager", regionManager);
			
			if (@this.GetRegionManager() != regionManager)
			{
				RegionManager.SetRegionManager(@this, regionManager);
			}
		}
		
		/// <summary>
		/// Find first parent region manager attached to a parent dependency object.
		/// </summary>
		/// <param name="this">
		/// The dependency object.
		/// </param>
		/// <returns>
		/// The <see cref="IRegionManager"/> of parent dependency object whether exists, otherwise <c>null</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">The <paramref name="this"/> is <c>null</c>.</exception>
		/// <remarks>
		/// This method walk up through the visual tree in order to find a parent dependency object with a <see cref="IRegionManager"/> attached.
		/// </remarks>
		public static IRegionManager FindFirstParentRegionManager(this DependencyObject @this)
		{
			Argument.IsNotNull("@this", @this);

			IRegionManager regionManager = null;
			DependencyObject current = VisualTreeHelper.GetParent(@this);
			while (current != null && (regionManager = current.GetRegionManager()) == null)
			{
				current = VisualTreeHelper.GetParent(current);
			}

			return regionManager;
		}

	    /// <summary>
	    /// Checks if the dependency object contains a <see cref="IRegion"/> with the name received as parameter. 
	    /// </summary>
	    /// <param name="this">
	    /// The dependency object.
	    /// </param>
	    /// <param name="regionName">
	    /// The region name.
	    /// </param>
	    /// <param name="serviceLocator">
	    /// The service locator.
	    /// </param>
	    /// <param name="defaultRegionManager">
	    /// The default region manager.
	    /// </param>
	    /// <returns>
	    /// The <see cref="IRegionInfo"/> if the view or nested visual nodes contains a region with the given name, otherwise <c>null</c>.
	    /// </returns>
	    /// <exception cref="System.ArgumentNullException">The <paramref name="this"/> is <c>null</c>.</exception>
	    /// <exception cref="System.ArgumentException">The <paramref name="regionName"/> is <c>null</c> or whitespace.</exception>
	    /// <remarks>
	    /// This method also ensures the setup of the same <see cref="IRegionManager"/> through the visual tree.<br/>
	    /// - If the dependency object has no a region manager the <paramref name="defaultRegionManager"/> will be set. <br/>
	    /// - If a parent dependency object of the give <paramref name="this"/> instance have a established <see cref="IRegionManager"/> then the <paramref name="defaultRegionManager" /> will be ignored.<br/>
	    /// </remarks>
	    public static IRegionInfo GetRegionInfo(this DependencyObject @this, string regionName, IServiceLocator serviceLocator = null, IRegionManager defaultRegionManager = null)
		{
			Argument.IsNotNull("@this", @this);
			Argument.IsNotNullOrWhitespace("regionName", regionName);

		    serviceLocator = serviceLocator ?? ServiceLocator.Default;

			IRegionInfo regionInfo = null;

			var queue = new Queue<DependencyObject>();
			queue.Enqueue(@this);

			defaultRegionManager = @this.GetRegionManager() ?? @this.FindFirstParentRegionManager() ?? defaultRegionManager;

			IRegionManager regionManager = null;
			do 
			{
				var current = queue.Dequeue();

				string currentRegionName = current.GetRegionName();
				if (regionManager == null && !string.IsNullOrEmpty(currentRegionName) && (regionManager = current.GetRegionManager()) == null)
				{
                    current.SetRegionManager(regionManager = defaultRegionManager ?? serviceLocator.ResolveType<IRegionManager>().CreateRegionManager());
				}

				if (currentRegionName == regionName)
				{
					if (regionManager != null)
					{
						current.SetRegionManager(regionManager);
					}

					regionInfo = new RegionInfo(current);
				}
				else
				{
					int childrenCount = VisualTreeHelper.GetChildrenCount(current);
					for (int i = 0; i < childrenCount; i++)
					{
						queue.Enqueue(VisualTreeHelper.GetChild(current, i));
					}
				}
			}
			while (regionInfo == null && queue.Count > 0);

			return regionInfo;
		}

		#endregion
	}
}