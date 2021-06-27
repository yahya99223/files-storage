using System.IO;

namespace FlyingDonkey.Api.Models
{
    public class FileInfo
    {
        public Stream Content { get; set; }
        public string Name { get; set; }
    }
}