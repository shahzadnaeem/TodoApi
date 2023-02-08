using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace TodoApi;

public static class DatabaseExtensions
{
    public const string DB_PASSWORD_SECRET_KEY = "DbPassword";
    public const string SQLITE = "Sqlite";
    public const string POSTGRES = "Postgres";

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var dbProvider = config.GetValue("DbProvider", SQLITE);

        if (dbProvider == SQLITE)
        {
            var connectionString = builder.Configuration.GetConnectionString(dbProvider);
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
            var assembly = typeof(Migrations.Postgres.Identifier).Assembly.GetName().Name;

            if (builder.Environment.IsDevelopment())
            {
                // Use password from user-secrets
                var dbPassword = config[DB_PASSWORD_SECRET_KEY];

                // Update using the Postgres specific builder
                var cb = new NpgsqlConnectionStringBuilder(connectionString);
                cb.Password = dbPassword;

                connectionString = cb.ConnectionString;
            }

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
