using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public static class DatabaseExtensions
{
    public const string SQLITE = "Sqlite";
    public const string POSTGRES = "Postgres";

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var dbProvider = config.GetValue("DbProvider", SQLITE);
        Console.WriteLine($"Got Database Provider: '{dbProvider}'");

        // NOTE: Need to keep separate Migrations projects for each DB type
        //       Postgres does not like Sqlite limitations on bool columns stored as INTEGER

        if (dbProvider == SQLITE)
        {
            var connectionString = builder.Configuration.GetConnectionString(dbProvider);
            System.Console.WriteLine($"# Got Sqlite connection string: '{connectionString}'");

            var assembly = typeof(Migrations.Sqlite.Identifier).Assembly.GetName().Name;

            builder.Services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseSqlite(
                    connectionString,
                    x => x.MigrationsAssembly(assembly));
            });
        }
        else if (dbProvider == POSTGRES)
        {
            var connectionString = builder.Configuration.GetConnectionString(dbProvider);
            System.Console.WriteLine($"# Got Postgres connection string: '{connectionString}'");

            var assembly = typeof(Migrations.Postgres.Identifier).Assembly.GetName().Name;

            builder.Services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    x => x.MigrationsAssembly(assembly));

            });
        }
        else
        {
            throw new Exception($"FATAL: Unknown Database Provider: '{dbProvider}'");
        }
    }
}
