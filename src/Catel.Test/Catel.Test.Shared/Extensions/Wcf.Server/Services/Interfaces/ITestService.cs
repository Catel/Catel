// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.Wcf.Server.Services
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ITestService
    {
        #region Methods
        [OperationContract]
        void DoSomething();
        #endregion
    }
}

#endif