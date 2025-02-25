using System;
using System.Collections.Generic;
using VersionNext.Models.Updates;

namespace VersionNext.Models
{
    public abstract class DatabaseVersion
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Patch;
        public readonly string FullVersion;
        public List<IDatabaseUpdate> DatabaseUpdates = new List<IDatabaseUpdate>();

        public DatabaseVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            FullVersion = $"{Major}.{Minor}.{Patch}";
        }

        public DatabaseVersion(string fullVersion)
        {
            var parts = fullVersion.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid version format", nameof(fullVersion));
            }
            Major = int.Parse(parts[0]);
            Minor = int.Parse(parts[1]);
            Patch = int.Parse(parts[2]);
            FullVersion = fullVersion;
        }

        public void AddSqlUpdate(string sql)
        {
            DatabaseUpdates.Add(new RawSqlUpdate(sql));
        }

        public bool HasNewer(DatabaseVersion other)
        {
            if (other.Major > Major)
                return true;
            if (other.Major == Major && other.Minor > Minor)
                return true;
            if(other.Major == Major && other.Minor == Minor && other.Patch > Patch)
                return true;
            return false;
        }
    }
}
