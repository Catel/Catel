// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyCell.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
    /// Control to fill up a row in the <see cref="StackGrid"/> control. This control will use an entire row to fill up.
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
    ///     <ColumnDefinition Width="Auto" />
    ///     <ColumnDefinition Width="*" />
    ///   </StackGrid.ColumnDefinitions>
    /// 
    ///   <Label Content="Title" />
    ///   <EmptyCell />
    /// 
    ///   <!-- Name, will be set to row 1, column 1 and 2 -->
    ///   <Label Content="Name" />
    ///   <TextBox Text="{Bindng Name}" />
    ///
    ///   <!-- Wrappanel, will span 2 columns -->
    ///   <WrapPanel StackGrid.ColumnSpan="2">
    ///     <Button Command="{Binding OK}" />
    ///   </WrapPanel>
    /// </StackGrid>
    /// ]]>
    /// </code>
    /// </example>
    public class EmptyCell : Control
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyCell"/> class.
        /// </summary>
        public EmptyCell()
        {
#if NET
            Focusable = false;
#endif
        }
        #endregion
    }
}