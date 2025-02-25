namespace VersionNext.Tests.Versions
{
    public class InitialDatabaseCreation : Models.VersionStart
    {
        public InitialDatabaseCreation() : base()
        {
            AddSqlUpdate("CREATE TABLE [dbo].[InitialTable] (Id INT Identity(1,1) PRIMARY KEY);");
        }
    }
}
