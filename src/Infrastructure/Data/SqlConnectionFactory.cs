using System.Data;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Data;

/// <summary>Cria conexões SQL Server a partir da connection string configurada.</summary>
public sealed class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqlConnection(connectionString);
}
