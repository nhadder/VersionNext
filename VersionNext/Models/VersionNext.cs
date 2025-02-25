namespace VersionNext.Models
{
    public abstract class VersionNext : DatabaseVersion
    {
        public VersionNext() : base(9999, 9999, 9999) { }
    }
}
