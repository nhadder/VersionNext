using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using VersionNext.Models;

namespace VersionNext
{
    public class DatabaseUpgrader
    {
        private readonly DatabaseUpgraderSettings _settings;
        private readonly Database _database;

        public DatabaseUpgrader(Database database)
        {
            _database = database;
            _settings = new DatabaseUpgraderSettings();
        }

        public DatabaseUpgrader(Database database, DatabaseUpgraderSettings settings)
        {
            _database = database;
            _settings = settings;
        }

        /// <summary>
        /// Upgrades the database if there is an update available. 
        /// Creates the database if it does not exist if UpgradeVersionStart setting is true.
        /// Set UpgradeVersionNext to true to upgrade to the next unreleased version.
        /// </summary>
        /// <returns></returns>
        public async Task UpgradeAsync()
        {
            var databaseVersions = _database.Versions;
            var currentVersion = await CurrentVersionAsync();

            if (_settings.UpgradeVersionStart && databaseVersions.First().GetType() == typeof(VersionStart) && currentVersion.GetType() == typeof(VersionStart))
                await SqlWrapper(string.Join("\n", databaseVersions.First().DatabaseUpdates.Select(u => u.GetCommandText())));

            if (!_settings.UpgradeVersionNext && databaseVersions.Last().GetType() == typeof(Models.VersionNext))
                databaseVersions.ToList().Remove(databaseVersions.Last());

            if (UpgradeAvailable(currentVersion, databaseVersions))
            {
                var versionSteps = _database.Versions.Skip(databaseVersions.ToList().IndexOf(currentVersion) + 1);
                foreach(var version in versionSteps)
                    await UpgradeDatabaseAsync(version);
            }
        }

        /// <summary>
        /// Creates the version table in the database if it doesn't exist and assumes starting version.
        /// Then returns the current DatabaseVersion from that table in the database.
        /// </summary>
        /// <returns>Current DatabaseVersion</returns>
        public async Task<DatabaseVersion> CurrentVersionAsync()
        {
            await EnsureVersionTableAsync();

            using (var connection = new SqlConnection(_database.ConnectionString.ToString()))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT [FullVersion] FROM {_settings.VersionTableSchema}.{_settings.VersionTableName}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var dbVersion = reader.GetString(reader.GetOrdinal("FullVersion"));
                        return _database.Versions.First(v => v.FullVersion.Equals(dbVersion));
                    }
                }
            }
        }

        #region Private Methods
        private async Task EnsureVersionTableAsync()
        {
            await SqlWrapper($@"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{_database.ConnectionString.InitialCatalog}')
            BEGIN
                USE MASTER;
                CREATE DATABASE {_database.ConnectionString.InitialCatalog};
            END    

            USE {_database.ConnectionString.InitialCatalog};
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = '{_settings.VersionTableName}' AND schema = '{_settings.VersionTableSchema}' xtype = 'U')
            BEGIN
                CREATE TABLE [{_settings.VersionTableSchema}].[{_settings.VersionTableName}] (
                    Major INT,
                    Minor INT,
                    Patch INT,
                    FullVersion NVARCHAR(20)
                );

                INSERT INTO {_settings.VersionTableSchema}.{_settings.VersionTableName} (Major, Minor, Patch, FullVersion)
                VALUES (0, 0, 0, '0.0.0');
            END");
        }

        private bool UpgradeAvailable(DatabaseVersion currentVersion, IOrderedEnumerable<DatabaseVersion> versions)
        {
            return currentVersion.IsNewer(versions.Last());
        }

        private async Task UpgradeDatabaseAsync(DatabaseVersion version)
        {
            await SqlWrapper(string.Join("\n", version.DatabaseUpdates.Select(u => u.GetCommandText())));
            await SqlWrapper($"UPDATE [{_settings.VersionTableSchema}].[{_settings.VersionTableName}] SET [Major] = {version.Major}, [Minor] = {version.Minor}, [Patch] = {version.Patch}, [FullVersion] = '{version.FullVersion}';");
        }

        private async Task SqlWrapper(string commandText)
        {
            using (var connection = new SqlConnection(_database.ConnectionString.ToString()))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = commandText;
                            command.Transaction = transaction;
                            command.CommandTimeout = _settings.SqlTimeoutSeconds;
                            await command.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        if (_settings.ThrowOnUpgradeFailure)
                            throw;
                    }
                }
            }
        }
        #endregion
    }
}
