// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StyleHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Logging;
    
#if NET
    using System.Windows.Markup;
    using System.Windows.Resources;
    using System.Xml;
    using Ricciolo.StylesExplorer.MarkupReflection;
#endif

    #region Enums
    /// <summary>
    /// Sets the available pixel shader modes of Catel.
    /// </summary>
    public enum PixelShaderMode
    {
        /// <summary>
        /// Disable all pixel shaders.
        /// </summary>
        Off,

        /// <summary>
        /// Automatically determine the best option.
        /// </summary>
        Auto,

        /// <summary>
        /// Use hardware for the pixel shaders.
        /// </summary>
        Hardware,

        /// <summary>
        /// Use software for the pixel shaders.
        /// </summary>
        Software
    }
    #endregion

    /// <summary>
    /// Helper class for WPF styles and themes.
    /// </summary>
    public static class StyleHelper
    {
        #region Constants
        /// <summary>
        /// Prefix of a default style key.
        /// </summary>
        private const string DefaultKeyPrefix = "Default";

        /// <summary>
        /// Postfix of a default style key.
        /// </summary>
        private const string DefaultKeyPostfix = "Style";
        #endregion

        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if !SILVERLIGHT
        /// <summary>
        /// Cached decompiled XAML resource dictionaries.
        /// </summary>
        private static readonly Dictionary<Uri, XmlDocument> _resourceDictionaryCache = new Dictionary<Uri, XmlDocument>();

        /// <summary>
        /// Cached types of <see cref="FrameworkElement"/> belonging to the string representation of the type.
        /// </summary>
        private static readonly Dictionary<string, Type> _styleToFrameworkElementTypeCache = new Dictionary<string, Type>();
#endif
        #endregion

        #region Properties
        /// <summary>
        /// This property allows you to disable all pixel shaders in Catel.
        /// <para />
        /// By default, all pixel shaders are enabled.
        /// </summary>
        public static PixelShaderMode PixelShaderMode = PixelShaderMode.Auto;

        /// <summary>
        /// Gets or sets a value indicating whether style forwarding is enabled. Style forwarding can be
        /// enabled by calling one of the <see cref="CreateStyleForwardersForDefaultStyles()"/> overloads.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is style forwarding enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsStyleForwardingEnabled { get; private set; }
        #endregion

#if !SILVERLIGHT
        /// <summary>
        /// Ensures that an application instance exists and the styles are applied to the application. This method is extremely useful
        /// to apply when WPF is hosted (for example, when loaded as plugin of a non-WPF application).
        /// </summary>
        /// <param name="applicationResourceDictionary">The application resource dictionary.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="applicationResourceDictionary"/> is <c>null</c>.</exception>
        public static void EnsureApplicationResourcesAndCreateStyleForwarders(Uri applicationResourceDictionary)
        {
            EnsureApplicationResourcesAndCreateStyleForwarders(applicationResourceDictionary, DefaultKeyPrefix);
        }

        /// <summary>
        /// Ensures that an application instance exists and the styles are applied to the application. This method is extremely useful
        /// to apply when WPF is hosted (for example, when loaded as plugin of a non-WPF application).
        /// </summary>
        /// <param name="applicationResourceDictionary">The application resource dictionary.</param>
        /// <param name="defaultPrefix">The default prefix, uses to determine the styles as base for other styles.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="applicationResourceDictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="defaultPrefix"/> is <c>null</c> or whitespace.</exception>
        public static void EnsureApplicationResourcesAndCreateStyleForwarders(Uri applicationResourceDictionary, string defaultPrefix)
        {
            Argument.IsNotNull("applicationResourceDictionary", applicationResourceDictionary);
            Argument.IsNotNullOrWhitespace("defaultPrefix", defaultPrefix);

            if (Application.Current == null)
            {
                try
                {
                    new Application();
                    Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(applicationResourceDictionary) as ResourceDictionary);

                    CreateStyleForwardersForDefaultStyles(Application.Current.Resources, defaultPrefix);

                    // Create an invisible dummy window to make sure that this is the main window
                    Window dummyMainWindow = new Window();
                    dummyMainWindow.Visibility = Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to ensure application resources");
                }
            }
        }
#endif

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// This method will use the current application (<see cref="System.Windows.Application.Current"/> to retrieve
        /// the resources. The forwarders will be written to the same dictionary.
        /// </summary>
        public static void CreateStyleForwardersForDefaultStyles()
        {
            CreateStyleForwardersForDefaultStyles(DefaultKeyPrefix);
        }

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// This method will use the current application (<see cref="System.Windows.Application.Current"/> to retrieve
        /// the resources. The forwarders will be written to the same dictionary.
        /// </summary>
        /// <param name="defaultPrefix">The default prefix, uses to determine the styles as base for other styles.</param>
        /// <exception cref="ArgumentException">The <paramref name="defaultPrefix"/> is <c>null</c> or whitespace.</exception>
        public static void CreateStyleForwardersForDefaultStyles(string defaultPrefix)
        {
            CreateStyleForwardersForDefaultStyles(Application.Current.Resources, defaultPrefix);
        }

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// This method will use the passed resources, but the forwarders will be written to the same dictionary as
        /// the source dictionary.
        /// </summary>
        /// <param name="sourceResources">Resource dictionary to read the keys from (thus that contains the default styles).</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceResources"/> is <c>null</c>.</exception>
        public static void CreateStyleForwardersForDefaultStyles(ResourceDictionary sourceResources)
        {
            CreateStyleForwardersForDefaultStyles(sourceResources, DefaultKeyPrefix);
        }

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// This method will use the passed resources, but the forwarders will be written to the same dictionary as
        /// the source dictionary.
        /// </summary>
        /// <param name="sourceResources">Resource dictionary to read the keys from (thus that contains the default styles).</param>
        /// <param name="defaultPrefix">The default prefix, uses to determine the styles as base for other styles.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="defaultPrefix"/> is <c>null</c> or whitespace.</exception>
        public static void CreateStyleForwardersForDefaultStyles(ResourceDictionary sourceResources, string defaultPrefix)
        {
            CreateStyleForwardersForDefaultStyles(sourceResources, sourceResources, defaultPrefix);
        }

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are 
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// <para />
        /// This method will use the passed resources.
        /// </summary>
        /// <param name="sourceResources">Resource dictionary to read the keys from (thus that contains the default styles).</param>
        /// <param name="targetResources">Resource dictionary where the forwarders will be written to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetResources"/> is <c>null</c>.</exception>
        public static void CreateStyleForwardersForDefaultStyles(ResourceDictionary sourceResources, ResourceDictionary targetResources)
        {
            CreateStyleForwardersForDefaultStyles(sourceResources, targetResources, DefaultKeyPrefix);
        }

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// <para/>
        /// This method will use the passed resources.
        /// </summary>
        /// <param name="sourceResources">Resource dictionary to read the keys from (thus that contains the default styles).</param>
        /// <param name="targetResources">Resource dictionary where the forwarders will be written to.</param>
        /// <param name="defaultPrefix">The default prefix, uses to determine the styles as base for other styles.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="defaultPrefix"/> is <c>null</c> or whitespace.</exception>
        public static void CreateStyleForwardersForDefaultStyles(ResourceDictionary sourceResources, ResourceDictionary targetResources,
            string defaultPrefix)
        {
            CreateStyleForwardersForDefaultStyles(sourceResources, sourceResources, targetResources, false, defaultPrefix);
        }

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// This method will use the passed resources.
        /// </summary>
        /// <param name="rootResourceDictionary">The root resource dictionary.</param>
        /// <param name="sourceResources">Resource dictionary to read the keys from (thus that contains the default styles).</param>
        /// <param name="targetResources">Resource dictionary where the forwarders will be written to.</param>
        /// <param name="forceForwarders">if set to <c>true</c>, styles will not be completed but only forwarders are created.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="rootResourceDictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetResources"/> is <c>null</c>.</exception>
        public static void CreateStyleForwardersForDefaultStyles(ResourceDictionary rootResourceDictionary, ResourceDictionary sourceResources,
            ResourceDictionary targetResources, bool forceForwarders)
        {
            CreateStyleForwardersForDefaultStyles(rootResourceDictionary, sourceResources, targetResources, forceForwarders, DefaultKeyPrefix);
        }

        /// <summary>
        /// Creates style forwarders for default styles. This means that all styles found in the theme that are
        /// name like Default[CONTROLNAME]Style (i.e. "DefaultButtonStyle") will be used as default style for the
        /// control.
        /// This method will use the passed resources.
        /// </summary>
        /// <param name="rootResourceDictionary">The root resource dictionary.</param>
        /// <param name="sourceResources">Resource dictionary to read the keys from (thus that contains the default styles).</param>
        /// <param name="targetResources">Resource dictionary where the forwarders will be written to.</param>
        /// <param name="forceForwarders">if set to <c>true</c>, styles will not be completed but only forwarders are created.</param>
        /// <param name="defaultPrefix">The default prefix, uses to determine the styles as base for other styles.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="rootResourceDictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="defaultPrefix"/> is <c>null</c> or whitespace.</exception>
        public static void CreateStyleForwardersForDefaultStyles(ResourceDictionary rootResourceDictionary, ResourceDictionary sourceResources,
            ResourceDictionary targetResources, bool forceForwarders, string defaultPrefix)
        {
            Argument.IsNotNull("rootResourceDictionary", rootResourceDictionary);
            Argument.IsNotNull("sourceResources", sourceResources);
            Argument.IsNotNull("targetResources", targetResources);
            Argument.IsNotNullOrWhitespace("defaultPrefix", defaultPrefix);

            #region If forced, use old mechanism
            if (forceForwarders)
            {
                // Get all keys from this resource dictionary
                var keys = (from key in sourceResources.Keys as ICollection<object>
                            where key is string &&
                                  ((string)key).StartsWith(defaultPrefix, StringComparison.InvariantCulture) &&
                                  ((string)key).EndsWith(DefaultKeyPostfix, StringComparison.InvariantCulture)
                            select key).ToList();

                foreach (string key in keys)
                {
                    Style style = sourceResources[key] as Style;
                    if (style != null)
                    {
                        Type targetType = style.TargetType;
                        if (targetType != null)
                        {
                            try
                            {
#if SILVERLIGHT
                                Style styleForwarder = new Style(targetType);
                                styleForwarder.BasedOn = style;
#else
                                Style styleForwarder = new Style(targetType, style);
#endif
                                targetResources.Add(targetType, styleForwarder);
                            }
                            catch (Exception ex)
                            {
                                Log.Warning(ex, "Failed to create style forwarder for '{0}'", key);
                            }
                        }
                    }
                }

                foreach (ResourceDictionary resourceDictionary in sourceResources.MergedDictionaries)
                {
                    CreateStyleForwardersForDefaultStyles(rootResourceDictionary, resourceDictionary, targetResources, forceForwarders, defaultPrefix);
                }
                
                return;
            }
            #endregion

            IEnumerable<Style> defaultStyles = FindDefaultStyles(sourceResources, defaultPrefix);

            foreach (Style defaultStyle in defaultStyles)
            {
                try
                {
                    Type targetType = defaultStyle.TargetType;
                    if (targetType != null)
                    {
                        ResourceDictionary resourceDictionaryDefiningStyle = FindResourceDictionaryDeclaringType(targetResources, targetType);
                        if (resourceDictionaryDefiningStyle != null)
                        {
                            Log.Debug("Completing the style info for '{0}' with the additional info from the default style definition", targetType);

                            resourceDictionaryDefiningStyle[targetType] = CompleteStyleWithAdditionalInfo(resourceDictionaryDefiningStyle[targetType] as Style, defaultStyle);
                        }
                        else
                        {
                            Log.Debug("Couln't find style definition for '{0}', creating style forwarder", targetType);

#if SILVERLIGHT
                            var targetStyle = new Style(targetType);
                            targetStyle.BasedOn = defaultStyle;
                            targetResources.Add(targetType, targetStyle);
#else
                            targetResources.Add(targetType, new Style(targetType, defaultStyle));
#endif
                        }
                    }
                }
                catch (Exception)
                {
                    Log.Warning("Failed to complete the style for '{0}'", defaultStyle);
                }
            }

#if !SILVERLIGHT
            RecreateDefaultStylesBasedOnTheme(rootResourceDictionary, targetResources, defaultPrefix);
#endif

            IsStyleForwardingEnabled = true;
        }

        /// <summary>
        /// Finds the <see cref="ResourceDictionary"/> declaring the real style for the target type.
        /// </summary>
        /// <param name="rootResourceDictionary">The root resource dictionary.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><see cref="ResourceDictionary"/> in which the style is defined, or <c>null</c> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="rootResourceDictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> is <c>null</c>.</exception>
        private static ResourceDictionary FindResourceDictionaryDeclaringType(ResourceDictionary rootResourceDictionary, Type targetType)
        {
            Argument.IsNotNull("rootResourceDictionary", rootResourceDictionary);
            Argument.IsNotNull("targetType", targetType);

            var styleKey = (from key in rootResourceDictionary.Keys as ICollection<object>
                            where key is Type && (Type)key == targetType
                            select key).FirstOrDefault();
            if (styleKey != null)
            {
                return rootResourceDictionary;
            }

            foreach (ResourceDictionary mergedResourceDictionary in rootResourceDictionary.MergedDictionaries)
            {
                var foundResourceDictionary = FindResourceDictionaryDeclaringType(mergedResourceDictionary, targetType);
                if (foundResourceDictionary != null)
                {
                    return foundResourceDictionary;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all the the default styles definitions
        /// </summary>
        /// <param name="sourceResources">The source resources.</param>
        /// <param name="defaultPrefix">The default prefix.</param>
        /// <returns>An enumerable of default styles.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceResources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="defaultPrefix"/> is <c>null</c> or whitespace.</exception>
        private static IEnumerable<Style> FindDefaultStyles(ResourceDictionary sourceResources, string defaultPrefix)
        {
            Argument.IsNotNull("sourceResources", sourceResources);
            Argument.IsNotNullOrWhitespace("defaultPrefix", defaultPrefix);

            var styles = new List<Style>();

            var keys = from key in sourceResources.Keys as ICollection<object>
                       where key is string &&
                             ((string)key).StartsWith(defaultPrefix, StringComparison.InvariantCulture) &&
                             ((string)key).EndsWith(DefaultKeyPostfix, StringComparison.InvariantCulture)
                       select key;

            foreach (string key in keys)
            {
                try
                {
                    var style = sourceResources[key] as Style;
                    if (style != null)
                    {
                        styles.Add(style);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to add a default style ('{0}') definition to the list of styles", key);
                }
            }

            foreach (var resourceDictionary in sourceResources.MergedDictionaries)
            {
                styles.AddRange(FindDefaultStyles(resourceDictionary, defaultPrefix));
            }

            return styles;
        }

        /// <summary>
        /// Completes a style with additional info.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="styleWithAdditionalInfo">The style with additional info.</param>
        /// <returns>New completed style.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="style"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="styleWithAdditionalInfo"/> is <c>null</c>.</exception>
        private static Style CompleteStyleWithAdditionalInfo(Style style, Style styleWithAdditionalInfo)
        {
            Argument.IsNotNull("style", style);
            Argument.IsNotNull("styleWithAdditionalInfo", styleWithAdditionalInfo);

            Style newStyle = new Style(style.TargetType);

            #region Copy style with additional info
            foreach (Setter setter in styleWithAdditionalInfo.Setters)
            {
                newStyle.Setters.Add(setter);
            }

#if !SILVERLIGHT
            foreach (Trigger trigger in styleWithAdditionalInfo.Triggers)
            {
                newStyle.Triggers.Add(trigger);
            }
#endif
            #endregion

            #region Copy original style
            foreach (Setter setter in style.Setters)
            {
                bool exists = (from styleSetter in newStyle.Setters
                               where ((Setter)styleSetter).Property == setter.Property
                               select styleSetter).Any();
                if (!exists)
                {
                    newStyle.Setters.Add(setter);
                }
            }

#if !SILVERLIGHT
            foreach (Trigger trigger in style.Triggers)
            {
                newStyle.Triggers.Add(trigger);
            }
#endif
            #endregion

            return newStyle;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Recreates the default styles based on theme.
        /// </summary>
        /// <param name="rootResourceDictionary">The root resource dictionary.</param>
        /// <param name="resources">The resources to fix.</param>
        /// <param name="defaultPrefix">The default prefix.</param>
        /// <remarks>
        /// This method is introduced due to the lack of the ability to use DynamicResource for the BasedOn property when
        /// defining styles inside a derived theme.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="rootResourceDictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resources"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="defaultPrefix"/> is <c>null</c> or whitespace.</exception>
        private static void RecreateDefaultStylesBasedOnTheme(ResourceDictionary rootResourceDictionary, ResourceDictionary resources, string defaultPrefix)
        {
            Argument.IsNotNull("rootResourceDictionary", rootResourceDictionary);
            Argument.IsNotNull("resources", resources);
            Argument.IsNotNull("defaultPrefix", defaultPrefix);

            var keys = (from key in resources.Keys as ICollection<object>
                        where key is string &&
                              ((string)key).StartsWith(defaultPrefix, StringComparison.InvariantCulture) &&
                              ((string)key).EndsWith(DefaultKeyPostfix, StringComparison.InvariantCulture)
                        select key).ToList();

            foreach (string key in keys)
            {
                var style = resources[key] as Style;
                if (style == null)
                {
                    continue;
                }

                var basedOnType = FindFrameworkElementStyleIsBasedOn(resources.Source, key);
                if (basedOnType == null)
                {
                    continue;
                }

                resources[key] = CloneStyleIfBasedOnControl(rootResourceDictionary, style, basedOnType);
            }

            foreach (var resourceDictionary in resources.MergedDictionaries)
            {
                RecreateDefaultStylesBasedOnTheme(rootResourceDictionary, resourceDictionary, defaultPrefix);
            }
        }
#endif

        /// <summary>
        /// Clones a style when the style is based on a control.
        /// </summary>
        /// <param name="rootResourceDictionary">The root resource dictionary.</param>
        /// <param name="style">The style.</param>
        /// <param name="basedOnType">Type which the style is based on.</param>
        /// <returns><see cref="Style"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="rootResourceDictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="style"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="basedOnType"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is introduced due to the lack of the ability to use DynamicResource for the BasedOn property when
        /// defining styles inside a derived theme.
        /// <para />
        /// Should be used in combination with the <c>RecreateDefaultStylesBasedOnTheme</c> method.
        /// </remarks>
        private static Style CloneStyleIfBasedOnControl(ResourceDictionary rootResourceDictionary, Style style, Type basedOnType)
        {
            Argument.IsNotNull("rootResourceDictionary", rootResourceDictionary);
            Argument.IsNotNull("style", style);
            Argument.IsNotNull("basedOnType", basedOnType);

#if SILVERLIGHT
            Style newStyle = new Style(style.TargetType);
            newStyle.BasedOn = rootResourceDictionary[basedOnType] as Style;
#else
            Style newStyle = new Style(style.TargetType, rootResourceDictionary[basedOnType] as Style);
#endif

            foreach (SetterBase setter in style.Setters)
            {
                newStyle.Setters.Add(setter);
            }

#if !SILVERLIGHT
            foreach (TriggerBase trigger in style.Triggers)
            {
                newStyle.Triggers.Add(trigger);
            }
#endif

            return newStyle;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Finds the <see cref="FrameworkElement"/> a specific style is based on.
        /// </summary>
        /// <param name="resourceDictionaryUri">The resource dictionary URI.</param>
        /// <param name="styleKey">The style key.</param>
        /// <returns>
        /// 	<see cref="Type"/> or <c>null</c> if the style is not based on a <see cref="FrameworkElement"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="resourceDictionaryUri"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="styleKey"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is introduced due to the lack of the ability to use DynamicResource for the BasedOn property when
        /// defining styles inside a derived theme.
        /// Should be used in combination with the <see cref="RecreateDefaultStylesBasedOnTheme"/> method.
        /// </remarks>
        private static Type FindFrameworkElementStyleIsBasedOn(Uri resourceDictionaryUri, string styleKey)
        {
            Argument.IsNotNull("resourceDictionaryUri", resourceDictionaryUri);
            Argument.IsNotNull("styleKey", styleKey);

            if (_styleToFrameworkElementTypeCache.ContainsKey(styleKey))
            {
                return _styleToFrameworkElementTypeCache[styleKey];
            }

            try
            {
                XmlDocument doc;

                if (_resourceDictionaryCache.ContainsKey(resourceDictionaryUri))
                {
                    doc = _resourceDictionaryCache[resourceDictionaryUri];
                }
                else
                {
                    StreamResourceInfo streamResourceInfo = Application.GetResourceStream(resourceDictionaryUri);
                    XmlBamlReader reader = new XmlBamlReader(streamResourceInfo.Stream);

                    doc = new XmlDocument();
                    doc.Load(reader);

                    _resourceDictionaryCache.Add(resourceDictionaryUri, doc);
                }

                #region Create xml namespace manager
                // Create namespace manager (all namespaces are required)
                XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);
                foreach (XmlAttribute namespaceAttribute in doc.DocumentElement.Attributes)
                {
                    // Clean up namespace (remove xmlns prefix)
                    string xmlNamespace = namespaceAttribute.Name.Replace("xmlns", "").TrimStart(new char[] { ':' });
                    xmlNamespaceManager.AddNamespace(xmlNamespace, namespaceAttribute.Value);
                }

                // Add a dummy node
                xmlNamespaceManager.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                xmlNamespaceManager.AddNamespace("ctl", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                #endregion

                string xpath = string.Format("/ctl:ResourceDictionary/ctl:Style[@x:Key='{0}']/@BasedOn", styleKey);
                XmlAttribute xmlAttribute = doc.SelectSingleNode(xpath, xmlNamespaceManager) as XmlAttribute;
                if (xmlAttribute == null)
                {
                    Log.Warning("Style '{0}' does not have the 'BasedOn' attribute defined", styleKey);

                    _styleToFrameworkElementTypeCache.Add(styleKey, null);

                    return null;
                }

                string basedOnValue = xmlAttribute.Value;
                basedOnValue = basedOnValue.Replace("StaticResource", "");
                basedOnValue = basedOnValue.Replace("x:Type", "").Trim(new[] { ' ', '{', '}' });

                #region Create xml type mapper
                var xamlTypeMapper = new XamlTypeMapper(new[] { "PresentationFramework" });
                foreach (XmlAttribute namespaceAttribute in doc.DocumentElement.Attributes)
                {
                    string xmlNamespace = namespaceAttribute.Name.Replace("xmlns", "").TrimStart(new char[] { ':' });

                    string value = namespaceAttribute.Value;
                    string clrNamespace = value;
                    string assemblyName = string.Empty;

                    if (clrNamespace.StartsWith("clr-namespace:"))
                    {
                        // We have a hit (formatting is normally one of the 2 below):
                        // * clr-namespace:[NAMESPACE]
                        // * clr-namespace:[NAMESPACE];assembly=[ASSEMBLY]
                        if (clrNamespace.Contains(";"))
                        {
                            clrNamespace = clrNamespace.Split(new char[] { ';' })[0];
                        }
                        clrNamespace = clrNamespace.Replace("clr-namespace:", "");

                        if (value.Contains(";"))
                        {
                            assemblyName = value.Split(new char[] { ';' })[1].Replace("assembly:", "");
                        }

                        xamlTypeMapper.AddMappingProcessingInstruction(xmlNamespace, clrNamespace, assemblyName);
                    }
                }
                #endregion

                string[] splittedType = basedOnValue.Split(new[] { ':' });
                string typeNamespace = (splittedType.Length == 2) ? splittedType[0] : "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                string typeName = (splittedType.Length == 2) ? splittedType[1] : splittedType[0];
                Type type = xamlTypeMapper.GetType(typeNamespace, typeName);
                if (type == null)
                {
                    _styleToFrameworkElementTypeCache.Add(styleKey, null);
                    return null;
                }

                Log.Debug("Style '{0}' is based on type '{1}'", styleKey, type);

                if ((type == typeof(FrameworkElement)) || type.IsSubclassOf(typeof(FrameworkElement)))
                {
                    _styleToFrameworkElementTypeCache.Add(styleKey, type);
                    return type;
                }


                _styleToFrameworkElementTypeCache.Add(styleKey, null);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to find the framework element where style '{0}' is based on", styleKey);
                return null;
            }
        }
#endif
    }
}
