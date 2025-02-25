# VersionNext

![VersionNext](https://img.shields.io/badge/VersionNext-Database%20Upgrader-blue)

**VersionNext** is a powerful and flexible database upgrade manager for SQL Server. Created by **Nick Hadder**, this NuGet package simplifies database deployment, version management, and upgrading to the latest development version for seamless development and production workflows.

## Features

- **Automated Database Deployment**: Easily deploy new databases with structured version control.
- **Version-Based Upgrades**: Upgrade databases incrementally with defined schema and data migrations.
- **Development Mode Support**: Upgrade to the latest development version for testing new changes.
- **Customizable Settings**: Configure upgrade behaviors like rollback, SQL timeout, and version tracking.
- **SQL Server Support**: Works with Microsoft SQL Server, leveraging Microsoft.Data.Sqlclient for database interactions.

## Installation

Install **VersionNext** via NuGet:

```sh
PM> Install-Package VersionNext
```

## Quick Start

### 1. Define Your Database Versions

Create the initial start of your database `VersionStart`.

```csharp
public class InitialDatabaseCreation : Models.VersionStart
{
    public InitialDatabaseCreation() : base()
    {
        AddSqlUpdate("CREATE TABLE [dbo].[InitialTable] (Id INT Identity(1,1) PRIMARY KEY);");
    }
}
```

Create a release version `DatabaseVersion`.

```csharp
public class v20250225 : DatabaseVersion
{
    public v20250225() : base(2025, 2, 25)
    {
        AddSqlUpdate("CREATE TABLE [dbo].[TestTable] ([Id] INT PRIMARY KEY);");
        AddSqlUpdate("INSERT INTO [dbo].[TestTable] ([Id]) VALUES (1);");
    }
}
```

Create the latest development version of your database `VersionNext`.

```csharp
public class InitialDatabaseCreation : Models.VersionNext
{
    public class vDevelop : Models.VersionNext
    {
        public vDevelop() : base()
        {
            AddSqlUpdate("CREATE TABLE [dbo].[LatestTestTable] ([Id] INT PRIMARY KEY);");
        }
    }
}
```

### 2. Configure Database and Upgrade Settings

```csharp
var connectionString = new SqlConnectionStringBuilder()
{
    DataSource = "localhost,1433",
    InitialCatalog = "MyDatabase",
    UserID = "sa",
    Password = "YourStrong!Passw0rd",
    TrustServerCertificate = true
};

var versions = new List<DatabaseVersion>()
{
    new InitialDatabaseCreation(), //initial database creation version (must be first)
    new v20250225(), //some released version where database upgrade was needed
    new vDevelop() //latest development version, yet to be released (must be last)
};

var database = new Database(connectionString, versions);

var settings = new DatabaseUpgraderSettings()
{
    ThrowOnUpgradeFailure = true,
    UpgradeVersionStart = true,
    UpgradeVersionNext = false,
    VersionTableSchema = "dbo",
    VersionTableName = "DatabaseVersion",
    SqlTimeoutSeconds = 60
};
```

### 3. Run Database Upgrades

```csharp
var upgrader = new DatabaseUpgrader(database, settings);
await upgrader.UpgradeAsync();
var currentVersion = await upgrader.CurrentVersionAsync();
Console.WriteLine($"Database upgraded to version: {currentVersion.FullVersion}");
```

## License

VersionNext is open-source and licensed under the MIT License.

## Author

**Nick Hadder** - Creator of VersionNext

## Contributing

Contributions are welcome! Feel free to submit issues or pull requests to improve VersionNext.

## Contact

For questions or feedback, reach out via GitHub Issues.

---

Happy upgrading! 🚀

