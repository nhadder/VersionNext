namespace VersionNext.Models
{
    public abstract class VersionStart : DatabaseVersion
    {
        public VersionStart() : base(0, 0, 0) { }
    }
}
