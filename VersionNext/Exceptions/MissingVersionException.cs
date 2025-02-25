using System;

namespace VersionNext.Exceptions
{
    public class MissingVersionException : Exception
    {
        public MissingVersionException() : base($"Missing VersionFirst and/or VersionNext from list of versions.")
        {
        }
    }
}
