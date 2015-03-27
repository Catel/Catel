// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexSerializationHierarchy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Catel.Data;

    public enum SortDirection
    {
        Ascending,

        Descending
    }

    public static class ComplexSerializationHierarchy
    {
        public static ScheduleAssistantSettings CreateComplexHierarchy()
        {
            var settings = new ScheduleAssistantSettings();

            settings.SelectedResource = "Resource 1";

            // main grid
            settings.GridSettings.ColumnSettings = new List<ColumnSettings>()
                {
                    new ColumnSettings {ColumnName = "Col1", IsHidden = false, Width = 100},
                    new ColumnSettings {ColumnName = "Col2", IsHidden = true, Width = 100},
                    new ColumnSettings {ColumnName = "Col3", IsHidden = false, Width = 80},
                    new ColumnSettings {ColumnName = "Col4", IsHidden = false, Width = 60}
                };

            settings.GridSettings.SortSettings = new List<SortSetting>()
                {
                    new SortSetting {ColumnName = "Col1", SortDirection = SortDirection.Ascending},
                    new SortSetting {ColumnName = "Col4", SortDirection = SortDirection.Descending},
                };

            settings.GridSettings.GroupingOrder = new List<string>()
                {
                    "Col1"
                };

            // pending grid
            settings.PendingGridSettings.ColumnSettings = new List<ColumnSettings>()
                {
                    new ColumnSettings {ColumnName = "Col1", IsHidden = false, Width = 100},
                    new ColumnSettings {ColumnName = "Col2", IsHidden = true, Width = 100},
                    new ColumnSettings {ColumnName = "Col3", IsHidden = false, Width = 80},
                    new ColumnSettings {ColumnName = "Col4", IsHidden = false, Width = 60}
                };

            settings.PendingGridSettings.SortSettings = new List<SortSetting>()
                {
                    new SortSetting {ColumnName = "Col3", SortDirection = SortDirection.Ascending},
                    new SortSetting {ColumnName = "Col2", SortDirection = SortDirection.Descending},
                };

            settings.PendingGridSettings.GroupingOrder = new List<string>()
                {
                    "Col2"
                };

            // scheduled grid
            settings.ScheduledGridSettings.ColumnSettings = new List<ColumnSettings>()
                {
                    new ColumnSettings {ColumnName = "Col1", IsHidden = false, Width = 100},
                    new ColumnSettings {ColumnName = "Col2", IsHidden = true, Width = 100},
                    new ColumnSettings {ColumnName = "Col3", IsHidden = false, Width = 80},
                    new ColumnSettings {ColumnName = "Col4", IsHidden = false, Width = 60}
                };

            settings.ScheduledGridSettings.SortSettings = new List<SortSetting>()
                {
                    new SortSetting {ColumnName = "Col4", SortDirection = SortDirection.Ascending},
                };

            settings.ScheduledGridSettings.GroupingOrder = new List<string>()
                {
                    "Col1"
                };

            return settings;
        }
    }

#if NET
    [Serializable]
#endif
    public class ScheduleAssistantSettings : SavableModelBase<ScheduleAssistantSettings>
    {
        public ScheduleAssistantSettings()
        {

        }

#if NET
        protected ScheduleAssistantSettings(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif

        #region SelectedResource property

        /// <summary>
        /// Gets or sets the SelectedResource value.
        /// </summary>
        public string SelectedResource
        {
            get { return GetValue<string>(SelectedResourceProperty); }
            set { SetValue(SelectedResourceProperty, value); }
        }

        /// <summary>
        /// SelectedResource property data.
        /// </summary>
        public static readonly PropertyData SelectedResourceProperty = RegisterProperty("SelectedResource", typeof(string));

        #endregion

        #region GridSettings property

        /// <summary>
        /// Gets or sets the GridSettings value.
        /// </summary>
        public GridSettings GridSettings
        {
            get { return GetValue<GridSettings>(GridSettingsProperty); }
            set { SetValue(GridSettingsProperty, value); }
        }

        /// <summary>
        /// GridSettings property data.
        /// </summary>
        public static readonly PropertyData GridSettingsProperty = RegisterProperty("GridSettings", typeof(GridSettings), new GridSettings());

        #endregion

        #region PendingGridSettings property

        /// <summary>
        /// Gets or sets the PendingGridSettings value.
        /// </summary>
        public GridSettings PendingGridSettings
        {
            get { return GetValue<GridSettings>(PendingGridSettingsProperty); }
            set { SetValue(PendingGridSettingsProperty, value); }
        }

        /// <summary>
        /// PendingGridSettings property data.
        /// </summary>
        public static readonly PropertyData PendingGridSettingsProperty = RegisterProperty("PendingGridSettings", typeof(GridSettings), new GridSettings());

        #endregion

        #region ScheduledGridSettings property

        /// <summary>
        /// Gets or sets the ScheduledGridSettings value.
        /// </summary>
        public GridSettings ScheduledGridSettings
        {
            get { return GetValue<GridSettings>(ScheduledGridSettingsProperty); }
            set { SetValue(ScheduledGridSettingsProperty, value); }
        }

        /// <summary>
        /// ScheduledGridSettings property data.
        /// </summary>
        public static readonly PropertyData ScheduledGridSettingsProperty = RegisterProperty("ScheduledGridSettings", typeof(GridSettings), new GridSettings());

        #endregion
    }

#if NET
    [Serializable]
#endif
    public class GridSettings : ModelBase
    {
        public GridSettings()
        {

        }

#if NET
        protected GridSettings(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif

        #region SortSettings property

        /// <summary>
        /// Gets or sets the SortSettings value.
        /// </summary>
        public List<SortSetting> SortSettings
        {
            get { return GetValue<List<SortSetting>>(SortSettingsProperty); }
            set { SetValue(SortSettingsProperty, value); }
        }

        /// <summary>
        /// SortSettings property data.
        /// </summary>
        public static readonly PropertyData SortSettingsProperty = RegisterProperty("SortSettings", typeof(List<SortSetting>), new List<SortSetting>());

        #endregion

        #region GroupingOrder property

        /// <summary>
        /// Gets or sets the GroupingOrder value.
        /// </summary>
        public List<string> GroupingOrder
        {
            get { return GetValue<List<string>>(GroupingOrderProperty); }
            set { SetValue(GroupingOrderProperty, value); }
        }

        /// <summary>
        /// GroupingOrder property data.
        /// </summary>
        public static readonly PropertyData GroupingOrderProperty = RegisterProperty("GroupingOrder", typeof(List<string>), new List<string>());

        #endregion

        #region ColumnSettings property

        /// <summary>
        /// Gets or sets the ColumnSettings value.
        /// </summary>
        public List<ColumnSettings> ColumnSettings
        {
            get { return GetValue<List<ColumnSettings>>(ColumnSettingsProperty); }
            set { SetValue(ColumnSettingsProperty, value); }
        }

        /// <summary>
        /// ColumnPositions property data.
        /// </summary>
        public static readonly PropertyData ColumnSettingsProperty = RegisterProperty("ColumnSettings", typeof(List<ColumnSettings>), new List<ColumnSettings>());

        #endregion
    }

#if NET
    [Serializable]
#endif
    public class SortSetting
    {
        protected bool Equals(SortSetting other)
        {
            return string.Equals(ColumnName, other.ColumnName) && SortDirection == other.SortDirection;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ColumnName != null ? ColumnName.GetHashCode() : 0) * 397) ^ (int)SortDirection;
            }
        }

        public string ColumnName { get; set; }
        public SortDirection SortDirection { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((SortSetting)obj);
        }
    }

#if NET
    [Serializable]
#endif
    public class ColumnSettings
    {
        protected bool Equals(ColumnSettings other)
        {
            return string.Equals(ColumnName, other.ColumnName) && Width.Equals(other.Width) && IsHidden.Equals(other.IsHidden);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (ColumnName != null ? ColumnName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ IsHidden.GetHashCode();
                return hashCode;
            }
        }

        public string ColumnName { get; set; }
        public double Width { get; set; }
        public bool IsHidden { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((ColumnSettings)obj);
        }
    }

}
