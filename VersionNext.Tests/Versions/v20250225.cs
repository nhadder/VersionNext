using VersionNext.Models;

namespace VersionNext.Tests.Versions
{
    public class v20250225 : DatabaseVersion
    {
        public v20250225() : base(2025, 2, 25)
        {
            AddSqlUpdate("CREATE TABLE [dbo].[TestTable] ([Id] INT PRIMARY KEY);");
            AddSqlUpdate("INSERT INTO [dbo].[TestTable] ([Id]) VALUES (1);");
        }
    }
}
