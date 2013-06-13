// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExecute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    /// <summary>
    /// Interface defining a method to execute the object. This allows several classes to be executed
    /// without the know-how of the types itself, as long as they implement this interface.
    /// </summary>
    public interface IExecute
    {
        /// <summary>
        /// Executes the object without any parameters.
        /// </summary>
        /// <returns><c>true</c> if the action is executed successfully; otherwise <c>false</c>.</returns>
        bool Execute();
    }
}