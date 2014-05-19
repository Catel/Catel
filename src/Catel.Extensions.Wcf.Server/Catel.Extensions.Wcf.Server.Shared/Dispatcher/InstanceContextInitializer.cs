// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceContextInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// 
    /// </summary>
    public class InstanceContextInitializer : IInstanceContextInitializer
    {
        #region IInstanceContextInitializer Members
        /// <summary>
        ///     Provides the ability to modify the newly created <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <param name="instanceContext">The system-supplied instance context.</param>
        /// <param name="message">The message that triggered the creation of the instance context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceContext"/> is <c>null</c>.</exception>
        public void Initialize(InstanceContext instanceContext, Message message)
        {
            Argument.IsNotNull("instanceContext", instanceContext);

            instanceContext.Extensions.Add(new InstanceContextExtension());
        }
        #endregion
    }
}