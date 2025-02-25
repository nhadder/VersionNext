using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace VersionNext.Models
{
    public class Database
    {
        public SqlConnectionStringBuilder ConnectionString;
        public readonly IOrderedEnumerable<DatabaseVersion> Versions;
        
        public Database(SqlConnectionStringBuilder connectionString, IEnumerable<DatabaseVersion> versions)
        {
            ConnectionString = connectionString;
            Versions = versions.OrderBy(v => v.FullVersion);
        }

        public Database(string connectionString, IEnumerable<DatabaseVersion> versions)
        {
            ConnectionString = new SqlConnectionStringBuilder(connectionString);
            Versions = versions.OrderBy(v => v.FullVersion);
        }
    }
}
