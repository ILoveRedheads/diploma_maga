using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CSService.Common.DataAccess;

public interface ISqlRepository
{
    Task<TEntity> Query<TEntity>(
        string sql,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null);

    Task<IEnumerable<TEntity>> QueryList<TEntity>(
        string sql,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null);

    Task<IEnumerable<TReturn>> QueryList<TFirst, TSecond, TReturn>(
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null);

    Task<IEnumerable<TReturn>> QueryList<TFirst, TSecond, TThird, TReturn>(
        string sql,
        Func<TFirst, TSecond, TThird, TReturn> map,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null);

    Task<int> Execute(
        string sql,
        object @params = null,
        IDbConnection connection = null,
        IDbTransaction transaction = null);
}
