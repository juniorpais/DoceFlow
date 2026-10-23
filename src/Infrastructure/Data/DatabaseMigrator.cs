using System.Reflection;
using DbUp;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public static class DatabaseMigrator
{
    /// <summary>
    /// Cria o banco (se não existir) e executa os scripts embutidos ainda não aplicados.
    /// Lança <see cref="InvalidOperationException"/> se a migração falhar.
    /// </summary>
    public static void Migrate(string connectionString, ILogger logger)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .LogToNowhere()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "Falha ao migrar o banco de dados.");
            throw new InvalidOperationException("Falha ao migrar o banco de dados.", result.Error);
        }

        logger.LogInformation(
            "Migração concluída. Scripts aplicados: {Quantidade}.",
            result.Scripts.Count());
    }
}
