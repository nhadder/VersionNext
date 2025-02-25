using Microsoft.Data.SqlClient;
using VersionNext.Models;
using VersionNext.Tests.Versions;

namespace VersionNext.Tests
{
    [TestClass]
    public sealed class VersionNextTests
    {
        private static SqlConnectionStringBuilder _connectionString = new SqlConnectionStringBuilder()
        {
            DataSource = "localhost,1433",
            InitialCatalog = "TEST_DB",
            UserID = "sa",
            Password = "YourStrong!Passw0rd",
            IntegratedSecurity = false,
            TrustServerCertificate = true
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
            var connectionString = new SqlConnectionStringBuilder(_testDatabase.ConnectionString.ToString()) { InitialCatalog = "master" };
            using (var connection = new SqlConnection(connectionString.ToString()))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"
ALTER DATABASE [{_testDatabase.ConnectionString.InitialCatalog}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [{_testDatabase.ConnectionString.InitialCatalog}]";
                    command.CommandTimeout = _settings.SqlTimeoutSeconds;
                    await command.ExecuteNonQueryAsync();
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
            Assert.AreEqual(new v20250225().FullVersion, currentVersion.FullVersion);

            //Upgrade to next unreleased/developer version
            _settings.UpgradeVersionNext = true;
            _settings.UpgradeVersionStart = false;
            await upgrader.UpgradeAsync();
            currentVersion = await upgrader.CurrentVersionAsync();
            Assert.AreEqual(new vDevelop().FullVersion, currentVersion.FullVersion);
        }
    }
}
