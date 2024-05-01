using Npgsql;
using System.Data;

namespace WebApi.Context;

public class DapperDbContext(IConfiguration _configuration)
{
    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_configuration.GetConnectionString("DbCon"));
}
