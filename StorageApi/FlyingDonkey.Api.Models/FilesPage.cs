using System.Collections.Generic;

namespace FlyingDonkey.Api.Models
{
    public class FilesPage
    {
        public List<FileDetails> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
