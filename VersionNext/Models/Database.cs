using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using VersionNext.Exceptions;

namespace VersionNext.Models
{
    public class Database
    {
        public SqlConnectionStringBuilder ConnectionString;
        public readonly IOrderedEnumerable<DatabaseVersion> Versions;
        
        public Database(SqlConnectionStringBuilder connectionString, IEnumerable<DatabaseVersion> versions)
        {
            ConnectionString = connectionString;
            ValidateStartAndEndVersion(versions);
            Versions = versions.OrderBy(v => v.FullVersion);
        }

        public Database(string connectionString, IEnumerable<DatabaseVersion> versions)
        {
            ConnectionString = new SqlConnectionStringBuilder(connectionString);
            ValidateStartAndEndVersion(versions);
            Versions = versions.OrderBy(v => v.FullVersion);
        }

        private void ValidateStartAndEndVersion(IEnumerable<DatabaseVersion> versions)
        {
            if (!(versions.First() is VersionStart && versions.Last() is VersionNext))
                throw new MissingVersionException();
        }
    }
}
