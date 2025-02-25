namespace VersionNext.Models
{
    public class DatabaseUpgraderSettings
    {
        /// <summary>
        /// Name of the version table in the database (Default: 'DatabaseVersion')
        /// </summary>
        public string VersionTableName { get; set; }
        /// <summary>
        /// Schema of the version table in the database (Default: 'dbo')
        /// </summary>
        public string VersionTableSchema { get; set; }
        /// <summary>
        /// True if you want to create the database and deploy the first version.
        /// False if the database already exists and you want to upgrade to the latest version.
        /// </summary>
        public bool UpgradeVersionStart { get; set; }
        /// <summary>
        /// True if you want to upgrade to the next unreleased version.
        /// False if you want to upgrade to the last released version.
        /// </summary>
        public bool UpgradeVersionNext { get; set; }
        /// <summary>
        /// Sql Timeout configuration for running updates. (Default: 60 seconds)
        /// </summary>
        public int SqlTimeoutSeconds { get; set; }
        /// <summary>
        /// If an exception occurs for a database upgrade step, should be throw or skip to the next step. (Default: true)
        /// </summary>
        public bool ThrowOnUpgradeFailure { get; set; }

        public DatabaseUpgraderSettings()
        {
            VersionTableName = "DatabaseVersion";
            VersionTableSchema = "dbo";
            UpgradeVersionStart = true;
            UpgradeVersionNext = false;
            SqlTimeoutSeconds = 60;
            ThrowOnUpgradeFailure = true;
        }
    }
}
