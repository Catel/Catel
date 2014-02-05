// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyColumn.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
#if NETFX_CORE
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Control to fill up a column in the <see cref="StackGrid"/> control. This control is just a placeholder for a grid cell.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <StackGrid>
    ///   <StackGrid.RowDefinitions>
    ///     <RowDefinition Height="Auto" />
    ///     <RowDefinition Height="*" />
    ///     <RowDefinition Height="Auto" />
    ///   </StackGrid.RowDefinitions>
    /// 
    ///   <StackGrid.ColumnDefinitions>
    ///	    <ColumnDefinition Width="Auto" />
    ///	    <ColumnDefinition Width="*" />
    ///   </StackGrid.ColumnDefinitions>
    /// 
    ///   <!-- Name, will be set to row 0, column 1 and 2 -->
    ///   <Label Content="Name" />
    ///   <TextBox Text="{Bindng Name}" />
    /// 
    ///   <!-- Empty row, will in this case use 2 columns -->
    ///   <EmptyRow />
    /// 
    ///   <!-- Wrappanel, will span 2 columns -->
    ///   <WrapPanel StackGrid.ColumnSpan="2">
    ///     <Button Command="{Binding OK}" />
    ///   </WrapPanel>
    /// </StackGrid>
    /// ]]>
    /// </code>
    /// </example>
    public class EmptyColumn : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyColumn"/> class.
        /// </summary>
        public EmptyColumn()
        {
#if NET
            Focusable = false;
#endif
        }
    }
}
