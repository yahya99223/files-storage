using System;
using System.IO;
using System.Threading.Tasks;
using FlyingDonkey.Api.Models;
using FileInfo = FlyingDonkey.Api.Models.FileInfo;

namespace FlyingDonkey.Storage.Handlers.Interfaces
{
    public interface IStorageHandler
    {
        Task<string> UploadFileToS3(Stream file, string key,string fileName);
        Task<FileInfo> DownloadFile(Guid id);
        Task<FilesPage> GetListOfFiles(uint page, uint size);
        Task<string> GetSignedLink(Guid id);

    }

   
}