namespace VersionNext.Tests.Versions
{
    public class vDevelop : Models.VersionNext
    {
        public vDevelop() : base()
        {
            AddSqlUpdate("CREATE TABLE [dbo].[LatestTestTable] ([Id] INT PRIMARY KEY);");
        }
    }
}
