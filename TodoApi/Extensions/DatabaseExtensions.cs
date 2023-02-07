using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public static class DatabaseExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        const string SQLITE = "Sqlite";
        const string POSTGRES = "Postgres";

        var config = builder.Configuration;

        var dbProvider = config.GetValue("DbProvider", SQLITE);
        Console.WriteLine($"Got Database Provider: '{dbProvider}'");

        // NOTE: Need to keep separate Migrations projects for each DB type
        //       Postgres does not like Sqlite limitations on bool columns stored as INTEGER

        if (dbProvider == SQLITE)
        {
            var connectionString = builder.Configuration.GetConnectionString(dbProvider);
            System.Console.WriteLine($"# Got Sqlite connection string: '{connectionString}'");

            builder.Services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });
        }
        else if (dbProvider == POSTGRES)
        {
            var connectionString = builder.Configuration.GetConnectionString(dbProvider);
            System.Console.WriteLine($"# Got Postgres connection string: '{connectionString}'");

            throw new Exception($"FATAL: Postgres can't use Sqlite Migrations!");

            builder.Services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }
        else
        {
            throw new Exception($"FATAL: Unknown Database Provider: '{dbProvider}'");
        }

        return builder;
    }
}
