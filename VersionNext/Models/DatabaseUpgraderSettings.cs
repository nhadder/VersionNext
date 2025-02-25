namespace VersionNext.Models
{
    public class DatabaseUpgraderSettings
    {
        public string VersionTableName { get; set; }
        public string VersionTableSchema { get; set; }
        public bool UpgradeVersionStart { get; set; }
        public bool UpgradeVersionNext { get; set; }
        public int SqlTimeoutSeconds { get; set; }
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
