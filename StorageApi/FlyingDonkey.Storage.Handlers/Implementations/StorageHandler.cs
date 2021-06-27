using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FlyingDonkey.Api.Models;
using FlyingDonkey.Storage.DataLayer;
using FlyingDonkey.Storage.Handlers.Interfaces;
using FlyingDonkey.Storage.Shared;
using Microsoft.Extensions.Options;
using FileInfo = FlyingDonkey.Api.Models.FileInfo;

namespace FlyingDonkey.Storage.Handlers.Implementations
{

    public class StorageHandler : IStorageHandler
    {
        private readonly S3Settings _storageSettings;
        private readonly IAmazonS3 _client;
        private readonly IFilesInfoRepository _attachmentsRepository;

        public StorageHandler(IOptions<Settings> settings, IAmazonS3 client, IFilesInfoRepository attachmentsRepository)
        {
            _storageSettings = settings.Value.S3Settings;
            _client = client;
            _attachmentsRepository = attachmentsRepository;
        }

        public async Task<string> UploadFileToS3(Stream file, string key, string fileName)
        {
            try
            {
                await using var newMemoryStream = new MemoryStream();
                await file.CopyToAsync(newMemoryStream);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = string.IsNullOrEmpty(_storageSettings.Folder) ? key : $"{_storageSettings.Folder}/{key}",
                    BucketName = _storageSettings.BucketName,
                    CannedACL = S3CannedACL.Private
                };

                var fileTransferUtility = new TransferUtility(_client);
                await fileTransferUtility.UploadAsync(uploadRequest);
                var size = file.Length;
                var path = await _attachmentsRepository.AddAttachment(uploadRequest.BucketName, uploadRequest.Key, fileName,size);
                return $"{path}";
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<FileInfo> DownloadFile(Guid id)
        {
            try
            {
                var location = await _attachmentsRepository.GetAttachment(id);
                if (location == null)
                    return null;
                var downloadRequest = new GetObjectRequest()
                {
                    Key = location.Key,
                    BucketName = location.Bucket,
                };
                var getFileResponse = await _client.GetObjectAsync(downloadRequest);
                var file = getFileResponse.ResponseStream;

                return new FileInfo()
                {
                    Content = file,
                    Name = location.Key
                };
            }
            catch (Exception e)
            {
                if (e is AmazonS3Exception)
                {
                    if ((e as AmazonS3Exception).StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }

                if (e is AggregateException && (e as AggregateException).InnerExceptions.FirstOrDefault(x => x is AmazonS3Exception) != null)
                {
                    var amazonException =
                        (e as AggregateException).Flatten().InnerExceptions.FirstOrDefault(x => x is AmazonS3Exception) as
                        AmazonS3Exception;
                    if (amazonException.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }
                throw;
            }
        }

        public async Task<FilesPage> GetListOfFiles(uint page, uint size)
        {
            var filesRecords = await _attachmentsRepository.GetPage(page, size);
            var order = (int)((page*size)-size);
            var filesDetails = new List<FileDetails>();
            foreach (var item in filesRecords)
            {
                order++;
                var details = new FileDetails()
                {
                    Name = item.FileName,
                    Date = item.UploadDate,
                    Url = await GetSignedLink(item.Id),
                    Order = order
                };
                filesDetails.Add(details);
            }

            return new FilesPage()
            {
                Items = filesDetails,
                TotalCount = await _attachmentsRepository.GetTotalFilesCount()
            };
        }


        public async Task<string> GetSignedLink(Guid id)
        {
            var location = await _attachmentsRepository.GetAttachment(id);
            if (location == null)
                return null;
            var downloadRequest = new GetPreSignedUrlRequest()
            {
                Key = location.Key,
                BucketName = location.Bucket,
                Expires = DateTime.UtcNow.AddMinutes(1)
            };
            var url = _client.GetPreSignedURL(downloadRequest);
            return url;
        }
    }

}
