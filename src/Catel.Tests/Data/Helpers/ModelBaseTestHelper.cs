// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseTestHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    /// <summary>
    ///   <see cref = "ModelBase" /> test helper class.
    /// </summary>
    public static class ModelBaseTestHelper
    {
        /// <summary>
        /// Saves the object to memory stream so the <see cref="IModel.IsDirty" /> property is set to false.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        internal static void SaveObjectToDummyMemoryStream(this ISavableModel obj, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                obj.Save(memoryStream, serializer, configuration);
            }
        }

        /// <summary>
        ///   Creates the ini entry object.
        /// </summary>
        /// <param name = "group">The group.</param>
        /// <param name = "key">The key.</param>
        /// <param name = "value">The value.</param>
        /// <returns>New <see cref = "IniEntry" />.</returns>
        public static IniEntry CreateIniEntryObject(string group, string key, string value)
        {
            return new IniEntry
                       {
                           Group = group,
                           Key = key,
                           Value = value
                       };
        }

        /// <summary>
        ///   Creates the ini entry object.
        /// </summary>
        /// <returns>New <see cref = "IniEntry" />.</returns>
        public static IniEntry CreateIniEntryObject()
        {
            var iniEntryObject = CreateIniEntryObject("MyGroup", "MyKey", "MyValue");
            iniEntryObject.IniEntryType = IniEntryType.Private;

            return iniEntryObject;
        }

        /// <summary>
        ///   Creates the ini file object.
        /// </summary>
        /// <param name = "fileName">Name of the file.</param>
        /// <param name = "iniEntries">The ini entries.</param>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static IniFile CreateIniFileObject(string fileName, IEnumerable<IniEntry> iniEntries)
        {
            // Ini file
            IniFile iniFile = new IniFile
                                  {
                                      FileName = fileName
                                  };

            // Add entries
            foreach (IniEntry iniEntry in iniEntries)
            {
                iniFile.IniEntryCollection.Add(iniEntry);
            }

            // Return result
            return iniFile;
        }

        /// <summary>
        ///   Creates the ini file object with some predefined values.
        /// </summary>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static IniFile CreateIniFileObject()
        {
            // Declare variables
            var iniEntries = new List<IniEntry>();

            // Create collection
            for (int i = 0; i < 3; i++)
            {
                var iniEntry = CreateIniEntryObject(string.Format("Group {0}", i),
                                                    string.Format("Key {0}", i),
                                                    string.Format("Value {0}", i));

                iniEntry.IniEntryType = (i % 2 == 0) ? IniEntryType.Public : IniEntryType.Private;

                iniEntries.Add(iniEntry);
            }

            // Create object
            return CreateIniFileObject("MyIniFile", iniEntries);
        }

        /// <summary>
        ///   Creates the computer settings object.
        /// </summary>
        /// <param name = "computerName">Name of the computer.</param>
        /// <param name = "iniFiles">The ini files.</param>
        /// <returns>New <see cref = "ComputerSettings" />.</returns>
        public static ComputerSettings CreateComputerSettingsObject(string computerName, IEnumerable<IniFile> iniFiles)
        {
            // Computer settings
            var computerSettings = new ComputerSettings
            {
                ComputerName = computerName
            };

            // Add entries
            foreach (var iniFile in iniFiles)
            {
                computerSettings.IniFileCollection.Add(iniFile);
            }

            computerSettings.SetValue("IsDirty", false);
            return computerSettings;
        }

        /// <summary>
        ///   Creates the computer settings object with some predefined values.
        /// </summary>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static ComputerSettings CreateComputerSettingsObject()
        {
            // Declare variables
            var iniFiles = new List<IniFile>();

            // Create collection
            for (int i = 0; i < 3; i++)
            {
                // Create object
                IniFile iniFile = CreateIniFileObject();
                iniFile.FileName = string.Format("MyFile {0}", i);
                iniFiles.Add(iniFile);
            }

            // Create object
            return CreateComputerSettingsObject("MyComputer", iniFiles);
        }

        /// <summary>
        ///   Creates the computer settings object with xml mappings.
        /// </summary>
        /// <param name = "computerName">Name of the computer.</param>
        /// <param name = "iniFiles">The ini files.</param>
        /// <returns>New <see cref = "ComputerSettings" />.</returns>
        public static ComputerSettingsWithXmlMappings CreateComputerSettingsWithXmlMappingsObject(string computerName, IEnumerable<IniFile> iniFiles)
        {
            // Copy and return
            return CreateComputerSettingsCopy(CreateComputerSettingsObject(computerName, iniFiles));
        }

        /// <summary>
        ///   Creates the computer settings with xml mappings object with some predefined values.
        /// </summary>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static ComputerSettingsWithXmlMappings CreateComputerSettingsWithXmlMappingsObject()
        {
            // Copy and return
            return CreateComputerSettingsCopy(CreateComputerSettingsObject());
        }

        /// <summary>
        ///   Creates the computer settings copy.
        /// </summary>
        /// <param name = "computerSettings">The computer settings.</param>
        /// <returns></returns>
        public static ComputerSettingsWithXmlMappings CreateComputerSettingsCopy(ComputerSettings computerSettings)
        {
            // Copy the properties
            ComputerSettingsWithXmlMappings computerSettingsWithXmlMappings = new ComputerSettingsWithXmlMappings();
            computerSettingsWithXmlMappings.ComputerName = computerSettings.ComputerName;
            computerSettingsWithXmlMappings.IniFileCollection = computerSettings.IniFileCollection;

            // Return result
            return computerSettingsWithXmlMappings;
        }

        /// <summary>
        /// Creates the hierarchical graph with inheritance.
        /// </summary>
        /// <returns></returns>
        public static ModelC CreateHierarchicalGraphWithInheritance()
        {
            var modelC = new ModelC
            {
                D = "D",
                E = new ModelA
                {
                    A = "A",
                    B = "B"
                }
            };

            return modelC;
        }
    }
}