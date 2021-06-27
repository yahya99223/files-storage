using System.Collections.Generic;

namespace FlyingDonkey.Storage.Shared
{
    public class SupportedFilesSettings
    {
        public int MaxFileSizeInMb { get; set; }
        public List<string> FileTypes { get; set; }
    }
}
