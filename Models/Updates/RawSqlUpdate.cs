
namespace VersionNext.Models.Updates
{
    public class RawSqlUpdate : IDatabaseUpdate
    {
        private readonly string _query;
        public RawSqlUpdate(string commandText)
        {
            _query = commandText;
        }

        public string GetCommandText()
        {
            return _query;
        }
    }
}
