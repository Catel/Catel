// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignTimeInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Catel.IoC;

    /// <summary>
    /// Initializes design-time code.
    /// </summary>
    public class DesignTimeInitializer
    {
        private static bool _internallyInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignTimeInitializer"/> class.
        /// </summary>
        public DesignTimeInitializer()
        {
            if (CanInitialize)
            {
                InternalInitialize();

                Initialize();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can be initialized.
        /// </summary>
        /// <value><c>true</c> if this instance can be initialized; otherwise, <c>false</c>.</value>
        public bool CanInitialize
        {
            get { return CatelEnvironment.IsInDesignMode; }
        }

        /// <summary>
        /// Initializes the design time data.
        /// </summary>
        private void InternalInitialize()
        {
            if (_internallyInitialized)
            {
                return;
            }

            _internallyInitialized = true;

            var serviceLocator = ServiceLocator.Default;

            var coreModule = new CoreModule();
            coreModule.Initialize(serviceLocator);

            var mvvmModule = new MVVMModule();
            mvvmModule.Initialize(serviceLocator);
        }

        /// <summary>
        /// Initializes the design time data.
        /// </summary>
        protected virtual void Initialize()
        {
        }
    }
}