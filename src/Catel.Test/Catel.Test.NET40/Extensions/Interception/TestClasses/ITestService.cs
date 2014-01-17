// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Interception
{
    using Catel.Interception;

    public interface ITestService
    {
        #region Properties
        string Name { get; set; }
        string Description { get; set; }
        bool WasExecuted { get; set; }
        #endregion

        #region Methods
        void Perform();
        void Perform(string value);
        void Perform(int value);
        T Perform<T>(T instance);
        [DoNotIntercept]
        void TaggedPerform();
        void Fail();
        int Return();
        #endregion
    }
}