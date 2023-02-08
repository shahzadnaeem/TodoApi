# .NET Core Multiple Database Provdiers Support

## Introduction

This document outlines allowing multiple database providers for an application.

Here, PostgreSQL is added to an application using Sqlite.

## Organisation

The project directory structure has been updated as follows

- TodoApi.Data directory with three new projects from the main `TodoApi` project
  - Models
    - This contains the EF Core models and a DbContext
  - Migrations.Sqlite
    - This contains the Sqlite specific migrations for Models
  - Migrations.Postgres
    - This contains the Postgres specific migrations for Models
- TodoApi
  - The main `TodoApi` project which implements the API

The main `TodoApi` project now references all three of the TodoApi.Data projects above.

## Some Notes

### TodoApi Changes

- Project references added to all of the above `Models`, `Migrations.Sqlite` and `Migrations.Postgres` projects
- Add a `DbProvider` entry to `appsettings.Development.json` to select which database to use.
  - A connection string is added for each named `DbProvider`
  - NOTE: Do not add a password to the connection string!
    - Add a secret named `DbPassword` using the `dotnet user-secrets set` command as shown below:

      ```sh
      # Run the command from the TodoApi project
      $ cd TodoApi

      $ dotnet user-secrets set "DbPassword" "password123"
      # You should not need to 'init' as that was done earlier
      ```

- `DatabaseExtensions.cs` added to support selecting the configured `DbProvider` at startup.
  - Use the correct connection string and `Assembly` for the configured `DbProvider`
    - Using the `DbPassword` secret where the connection string needs a password

### Managing Migrations

New migrations need to be added to each migration project to ensure that they all remain in sync.

NOTE: On adding a new database, in this example, Postgres, The new database will need to be initialised. This is done by adding an `Initial` migration to sync it with the existing database configuration. This may result in different initial migration histories for newer databases.

```sh
# Run the commands from the TodoApi project
$ cd TodoApi

# Specify the migrations project we want to update and also its `DbProvider` config
$ dotnet ef migrations add InitialBundle --project ../TodoApi.Data/Migrations.Postgres -- --DbProvider=Postgres

# Now apply the created 'InitialBundle' migration for Postgres
$ dotnet ef database update --project ../TodoApi.Data/Migrations.Postgres -- --DbProvider=Postgres
```

Once the above is run both database options are availble for the `TodoApi` application.
