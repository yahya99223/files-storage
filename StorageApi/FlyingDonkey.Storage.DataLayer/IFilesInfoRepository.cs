using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyingDonkey.Storage.DataLayer
{
    public interface IFilesInfoRepository
    {
        Task<FileInfoRecord> GetAttachment(Guid id);
        Task<Guid> AddAttachment(string bucket, string key,string fileName, double size);
        Task<IList<FileInfoRecord>> GetPage(uint page, uint size);
        Task<int> GetTotalFilesCount();
    }
}
