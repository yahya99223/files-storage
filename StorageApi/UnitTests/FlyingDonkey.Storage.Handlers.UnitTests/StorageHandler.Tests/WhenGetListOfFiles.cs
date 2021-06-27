using System;
using System.Collections.Generic;
using Amazon.S3;
using FlyingDonkey.Storage.DataLayer;
using FlyingDonkey.Storage.Shared;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FlyingDonkey.Storage.Handlers.UnitTests.StorageHandler.Tests
{
    [TestFixture]
    public class WhenGetListOfFiles
    {
        private Implementations.StorageHandler storageHandler;
        private Mock<IOptions<Settings>> settings;
        private Mock<IAmazonS3> s3Client;
        private Mock<IFilesInfoRepository> repository;
        private uint page = 5, size = 3;
        [SetUp]
        public void SetUp()
        {
            settings = new Mock<IOptions<Settings>>();
            s3Client = new Mock<IAmazonS3>();
            repository = new Mock<IFilesInfoRepository>();
            settings.Setup(x => x.Value).Returns(new Settings()
            {
                S3Settings = new S3Settings()
            });
            repository.Setup(x => x.GetPage(It.IsAny<uint>(), It.IsAny<uint>()))
                .ReturnsAsync(new List<FileInfoRecord>());
            repository.Setup(x => x.GetTotalFilesCount()).ReturnsAsync(100);
            storageHandler = new Implementations.StorageHandler(settings.Object, s3Client.Object, repository.Object);
        }
        [TearDown]
        public void Teardown()
        {

        }
        [Test]
        public void Then_FilesRepository_GetPage_And_GetTotal_Should_Be_Called_Once_Each()
        {
            var result = storageHandler.GetListOfFiles(page, size).Result;
            repository.Verify(x => x.GetPage(page, size), Times.Once);
            repository.Verify(x => x.GetTotalFilesCount(), Times.Once);
        }
        [Test]
        public void Then_Exception_Should_Be_Thrown_If_Repository_Throws_Exception()
        {
            repository.Setup(x => x.GetTotalFilesCount()).ThrowsAsync(new Exception());
            storageHandler = new Implementations.StorageHandler(settings.Object, s3Client.Object, repository.Object);
            var exceptionThrown = false;
            try
            {
                var result = storageHandler.GetListOfFiles(page, size).Result;
            }
            catch (Exception e)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }
        [Test]
        public void Then_Result_TotalCount_Should_Match_With_Repository_TotalCount()
        {
            var count = new Random().Next(1, 1000);
            repository.Setup(x => x.GetTotalFilesCount()).ReturnsAsync(count);
            storageHandler = new Implementations.StorageHandler(settings.Object, s3Client.Object, repository.Object);
            var result = storageHandler.GetListOfFiles(page, size).Result;
            Assert.AreEqual(count, result.TotalCount);
        }
    }
}
