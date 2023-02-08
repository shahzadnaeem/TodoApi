# .NET Core Multiple Database Support

## Introduction

This document outlines how run an application on multiple databases.

In this example, we support Sqlite and PostgreSQL

## Organisation

As you can see, the following sub-project directory structure is used.

- Models
  - This contains the EF Core models and a DbContext
- Migrations.Sqlite
  - This contains the Sqlite specific migrations for Models
- Migrations.Postgres
  - This contains the Postgres specific migrations for Models

All of the above projects are referenced by the main `TodoApi` project which can be configured to select either Sqlite or Postgres.

## Some Notes

### TodoApi Changes

- Project references added to all of the above `Models`, `Migrations.Sqlite` and `Migrations.Postgres` projects
- Add a `DbProvider` entry to `appsettings.Development.json` to select which database to use.
  - A connection string is added for each named `DbProvider`
- `DatabaseExtensions.cs` added to support selecting the configured `DbProvider` at startup.
  - Use the correct connection string and `Assembly` for the configured `DbProvider`

### Managing Migrations

When adding migrations, the migration has to be added to each migrations project in turn.

NOTE: Adding a new database (in our case, Postgres was added after Sqlite). The new database will need an `Initial` migration added first to sync it with the existing databases. This will result in a different migration history for new databases.

When support was added for Postgres, the following `Initial` migration was added - a single migration, incorporating the two migration that exist for Sqlite.

```sh
# Run the commands from the main project
$ cd TodoApi

# Specify the migrations project we want to update and also its `DbProvider` config
$ dotnet ef migrations add InitialBundle --project ../TodoApi.Data/Migrations.Postgres -- --DbProvider=Postgres

# Now apply the created 'InitialBundle' migration for Postgres
$ dotnet ef database update --project ../TodoApi.Data/Migrations.Postgres -- --DbProvider=Postgres
```

Once the above is run both database options are availble for the `TodoApi` application.
