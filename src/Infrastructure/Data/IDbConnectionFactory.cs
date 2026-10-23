using System.Data;

namespace Infrastructure.Data;

/// <summary>Fábrica de conexões com o banco. Cada operação cria e descarta sua própria conexão.</summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
