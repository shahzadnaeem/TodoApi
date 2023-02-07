using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace TodoApi;

public static class OpenApiExtensions
{
    // Adds the JWT security scheme to the Open API description
    public static IEndpointConventionBuilder AddOpenApiSecurityRequirement(this IEndpointConventionBuilder builder)
    {
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new()
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        };

        return builder.WithOpenApi(operation => new(operation)
        {
            Security =
            {
                new()
                {
                    [scheme] = new List<string>()
                }
            }
        });
    }

    public static void AddCustomSwaggerGen(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        builder.Services.AddSwaggerGen(c =>
        {
            var dotNetVer = System.Environment.Version.ToString();
            var dbProvider = config.GetValue("DbProvider", DatabaseExtensions.SQLITE);

            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = $"TodoApi (.NET {dotNetVer} - DB {dbProvider})"
            });
        });
    }
}
