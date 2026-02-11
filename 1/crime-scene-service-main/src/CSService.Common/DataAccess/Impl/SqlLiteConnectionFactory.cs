using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace CSService.Common.DataAccess.Impl;

internal sealed class SqlLiteConnectionFactory(IConnectionSettings connectionSettings) : IConnectionFactory
{
    private readonly string _connectionString = connectionSettings.ConnectionString;

    public Task<IDbConnection> Create() => Create(_connectionString);

    public async Task<IDbConnection> Create(string connectionString) {
        var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        return connection;
    }
}
