using System.Data;
using System.Threading.Tasks;

namespace CSService.Common.DataAccess;

public interface IConnectionFactory
{
    Task<IDbConnection> Create();

    Task<IDbConnection> Create(string connectionString);

}
