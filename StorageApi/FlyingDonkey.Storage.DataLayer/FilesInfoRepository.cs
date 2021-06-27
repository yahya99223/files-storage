using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FlyingDonkey.Storage.DataLayer
{

    public class FilesInfoRepository : IFilesInfoRepository
    {
        private readonly StorageServiceDbContext _dbContext;

        public FilesInfoRepository(StorageServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddAttachment(string bucket, string key, string fileName, double size)
        {
            var record = new FileInfoRecord()
            {
                Id = Guid.NewGuid(),
                UploadDate = DateTime.UtcNow,
                Bucket = bucket,
                Key = key,
                FileName = fileName,
                Extension = fileName.Split('.').Last(),
                Size = size
            };
            try
            {
                await _dbContext.Files.AddAsync(record);
                await _dbContext.SaveChangesAsync();
                return record.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(JsonConvert.SerializeObject(record));
                throw;
            }
        }

        public async Task<IList<FileInfoRecord>> GetPage(uint page, uint size)
        {
            return await _dbContext.Files.Skip((int)size * ((int)page - 1)).Take((int)size).ToListAsync();
        }

        public async Task<int> GetTotalFilesCount()
        {
            return await _dbContext.Files.CountAsync();
        }

        public async Task<FileInfoRecord> GetAttachment(Guid id)
        {
            return await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
