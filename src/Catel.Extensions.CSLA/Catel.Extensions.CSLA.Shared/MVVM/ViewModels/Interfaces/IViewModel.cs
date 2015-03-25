// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.CSLA
{
	using System;

	/// <summary>
	/// View model for CSLA view models.
	/// </summary>
	[CLSCompliant(false)]
	public interface IViewModel : Catel.MVVM.IViewModel, Csla.Xaml.IViewModel
	{
	}
}