using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CSService.Common.DataAccess.Exceptions;
using Dapper;

namespace CSService.Common.DataAccess.Impl;

internal sealed class SqlRepository(IConnectionFactory connectionFactory) : ISqlRepository
{
    private const int _commandTimeout = 300;

    public async Task<int> Execute(
        string sql,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null) {
        if (connection is null) {
            using (connection = await connectionFactory.Create()) {
                try {
                    return await connection.ExecuteAsync(sql, @params, commandTimeout: _commandTimeout);
                } catch (Exception ex) {
                    throw new SqlException(sql, ex);
                }
            }
        }

        try {
            return await connection.ExecuteAsync(sql, @params, transaction, commandTimeout: _commandTimeout);
        } catch (Exception ex) {
            throw new SqlException(sql, ex);
        }
    }

    public async Task<TEntity> Query<TEntity>(
        string sql,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null) {
        if (connection is null) {
            using (connection = await connectionFactory.Create()) {
                try {
                    return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, @params, commandTimeout: _commandTimeout);
                } catch (Exception ex) {
                    throw new SqlException(sql, ex);
                }
            }
        }

        try {
            return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, @params, transaction, commandTimeout: _commandTimeout);
        } catch (Exception ex) {
            throw new SqlException(sql, ex);
        }
    }

    public async Task<IEnumerable<TEntity>> QueryList<TEntity>(
        string sql,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null) {
        if (connection is null) {
            using (connection = await connectionFactory.Create()) {
                try {
                    return await connection.QueryAsync<TEntity>(sql, @params, commandTimeout: _commandTimeout);
                } catch (Exception ex) {
                    throw new SqlException(sql, ex);
                }
            }
        }

        try {
            return await connection.QueryAsync<TEntity>(sql, @params, transaction, commandTimeout: _commandTimeout);
        } catch (Exception ex) {
            throw new SqlException(sql, ex);
        }
    }

    public async Task<IEnumerable<TReturn>> QueryList<TFirst, TSecond, TReturn>(
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null) {
        if (connection is null) {
            using (connection = await connectionFactory.Create()) {
                try {
                    return await connection.QueryAsync(sql, map, @params, commandTimeout: _commandTimeout);
                } catch (Exception ex) {
                    throw new SqlException(sql, ex);
                }
            }
        }

        try {
            return await connection.QueryAsync(sql, map, @params, transaction, commandTimeout: _commandTimeout);
        } catch (Exception ex) {
            throw new SqlException(sql, ex);
        }
    }

    public async Task<IEnumerable<TReturn>> QueryList<TFirst, TSecond, TThird, TReturn>(
        string sql,
        Func<TFirst, TSecond, TThird, TReturn> map,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null) {
        if (connection is null) {
            using (connection = await connectionFactory.Create()) {
                try {
                    return await connection.QueryAsync(sql, map, @params, commandTimeout: _commandTimeout);
                } catch (Exception ex) {
                    throw new SqlException(sql, ex);
                }
            }
        }

        try {
            return await connection.QueryAsync(sql, map, @params, transaction, commandTimeout: _commandTimeout);
        } catch (Exception ex) {
            throw new SqlException(sql, ex);
        }
    }
}
