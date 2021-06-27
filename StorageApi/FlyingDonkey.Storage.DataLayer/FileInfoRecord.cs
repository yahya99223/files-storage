using System;

namespace FlyingDonkey.Storage.DataLayer
{
    public class FileInfoRecord
    {
        public Guid Id { get; set; }
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public DateTime UploadDate { get; set; }
        public double Size { get; set; }
    }
}
