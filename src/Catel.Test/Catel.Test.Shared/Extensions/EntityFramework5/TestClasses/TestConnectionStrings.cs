// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestConnectionStrings.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.EntityFramework5
{
    public static class TestConnectionStrings
    {
        #region Constants
        public const string DbContextDefault = "data source=.\\SQLExpress;initial catalog=CatelUnitTestDbContext;Integrated Security=true";
        public const string DbContextModified = "data source=myServerAddress;initial catalog=myCustomizedDataBase;User Id=myUsername;Password=myPassword";

        public const string ObjectContextDefault = "data source=.\\SQLExpress;initial catalog=CatelUnitTestObjectContext;Integrated Security=true";
        public const string ObjectContextModified = "data source=myServerAddress;initial catalog=myCustomizedDataBase;User Id=myUsername;Password=myPassword";
        #endregion
    }
}

#endif