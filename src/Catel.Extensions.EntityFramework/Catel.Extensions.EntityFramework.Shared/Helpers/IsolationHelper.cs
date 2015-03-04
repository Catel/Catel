// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsolationHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Data;

    /// <summary>
    /// Helper class for database isolation.
    /// </summary>
    public static class IsolationHelper
    {
        /// <summary>
        /// Translates the transaction level to a SQL command which can be used to change the transaction level.
        /// </summary>
        /// <remarks>
        /// For additional information, see http://msdn.microsoft.com/en-us/library/ms189542(v=sql.105).aspx.
        /// </remarks>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>The SQL command.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <see cref="IsolationLevel"/> value is not supported.</exception>
        public static string TranslateTransactionLevelToSql(IsolationLevel isolationLevel)
        {
            var sqlCommand = "SET TRANSACTION LEVEL ISOLATION LEVEL ";

            switch (isolationLevel)
            {
                case IsolationLevel.Chaos:
                    throw new ArgumentOutOfRangeException("isolationLevel", "Transaction isolation level 'Chaos' is not supported");

                case IsolationLevel.ReadUncommitted:
                    sqlCommand += "READ UNCOMMITTED";
                    break;

                case IsolationLevel.ReadCommitted:
                    sqlCommand += "READ COMMITTED";
                    break;

                case IsolationLevel.RepeatableRead:
                    sqlCommand += "REPEATABLE READ";
                    break;

                case IsolationLevel.Serializable:
                    sqlCommand += "SERIALIZABLE";
                    break;

                case IsolationLevel.Snapshot:
                    sqlCommand += "SNAPSHOT";
                    break;

                case IsolationLevel.Unspecified:
                    throw new ArgumentOutOfRangeException("isolationLevel", "Transaction isolation level 'Unspecified' is not supported");

                default:
                    throw new ArgumentOutOfRangeException("isolationLevel");
            }

            sqlCommand += ";";

            return sqlCommand;
        }
    }
}