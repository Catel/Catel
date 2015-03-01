// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Interception
{
    using System;
    using Catel.Interception;

    public class TestService : ITestService
    {
        #region Fields
        private string _name;
        #endregion

        #region Constructors
        public TestService()
        {
            _name = "testValue";
        }
        #endregion

        #region ITestService Members
        public bool WasExecuted { get; set; }

        public virtual string Name
        {
            get
            {
                WasExecuted = true;
                return _name;
            }

            set
            {
                _name = value;
                WasExecuted = true;
            }
        }

        public string Description { get; set; }

        public void Fail()
        {
            WasExecuted = true;
            throw new InvalidOperationException();
        }

        public int Return()
        {
            WasExecuted = true;
            return 1;
        }

        public void TaggedPerform()
        {
            WasExecuted = true;
        }

        public virtual void Perform()
        {
            WasExecuted = true;
        }

        public void Perform(string value)
        {
            WasExecuted = true;
        }

        public void Perform(int a)
        {
            WasExecuted = true;
        }

        public T Perform<T>(T instance)
        {
            WasExecuted = true;
            return instance;
        }
        #endregion
    }
}

#endif