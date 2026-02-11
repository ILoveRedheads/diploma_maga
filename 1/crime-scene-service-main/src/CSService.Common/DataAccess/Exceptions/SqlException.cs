using System;

namespace CSService.Common.DataAccess.Exceptions;

public sealed class SqlException : Exception
{
    public SqlException(string sql, Exception innerException)
        : base($"An error ocured while executing SQL: {sql}", innerException) { }
}
