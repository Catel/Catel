// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignerProperties.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace System.ComponentModel
{
    using System.Reflection;

    using Catel.Logging;

    /// <summary>
    /// Designer properties for portable class library.
    /// </summary>
    public static class DesignerProperties
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a value indicating whether this instance is currently hosted in a design tool.
        /// </summary>
        /// <value><c>true</c> if this instance is currently hosted in a design tool; otherwise, <c>false</c>.</value>
        public static bool IsInDesignTool
        {
            get
            {
                try
                {
                    // SL
                    var slDesignerProperties = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows");
                    if (slDesignerProperties != null)
                    {
                        var isInDesignToolProperty = slDesignerProperties.GetProperty("IsInDesignTool", BindingFlags.Static | BindingFlags.Public);
                        return (bool)isInDesignToolProperty.GetValue(null, null);
                    }

                    // NET
                    var netDesignerProperties = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework");
                    if (netDesignerProperties != null)
                    {
                        var isInDesignToolProperty = netDesignerProperties.GetProperty("IsInDesignTool", BindingFlags.Static | BindingFlags.Public);
                        return (bool)isInDesignToolProperty.GetValue(null, null);
                    }

                    // WINDOWS
                    var windowsDesignMode = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
                    if (windowsDesignMode != null)
                    {
                        var designModeEnabledProperty = windowsDesignMode.GetProperty("DesignModeEnabled", BindingFlags.Static | BindingFlags.Public);
                        return (bool)designModeEnabledProperty.GetValue(null, null);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to check if in design mode");
                }

                return false;
            }
        }
    }
}