using System;

namespace FlyingDonkey.Api.Models
{
    public class FileDetails
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
    }
}