using Microsoft.Data.SqlClient;
using VersionNext.Models;
using VersionNext.Tests.Releases;

namespace VersionNext.Tests
{
    [TestClass]
    public sealed class VersionNextTests
    {
        private static SqlConnectionStringBuilder _connectionString = new SqlConnectionStringBuilder()
        {
            DataSource = "localhost",
            InitialCatalog = "TEST_DB",
            IntegratedSecurity = true
        };

        private static List<DatabaseVersion> _versions = new List<DatabaseVersion>()
        {
            new InitialDatabaseCreation(),
            new v20250225(),
            new vDevelop()
        };

        private Database _testDatabase = new Database(_connectionString, _versions);

        private DatabaseUpgraderSettings _settings = new DatabaseUpgraderSettings()
        {
            ThrowOnUpgradeFailure = true,
            UpgradeVersionStart = true,
            UpgradeVersionNext = false,
            VersionTableSchema = "dbo",
            VersionTableName = "DatabaseVersion",
            SqlTimeoutSeconds = 60
        };

        [TestCleanup]
        public async Task TestCleanup()
        {
            using (var connection = new SqlConnection(_testDatabase.ConnectionString.ToString()))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = $"DROP {_testDatabase.ConnectionString.InitialCatalog}";
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

        [TestMethod]
        public async Task CreateAndUpgradeDatabase()
        {
            var upgrader = new DatabaseUpgrader(_testDatabase, _settings);

            ///Create database and upgrade to latest release version
            await upgrader.UpgradeAsync();
            var currentVersion = await upgrader.CurrentVersionAsync();
            Assert.Equals(currentVersion.FullVersion, new v20250225().FullVersion);

            //Upgrade to next unreleased/developer version
            _settings.UpgradeVersionNext = true;
            await upgrader.UpgradeAsync();
            currentVersion = await upgrader.CurrentVersionAsync();
            Assert.Equals(currentVersion.FullVersion, new vDevelop().FullVersion);
        }
    }
}
