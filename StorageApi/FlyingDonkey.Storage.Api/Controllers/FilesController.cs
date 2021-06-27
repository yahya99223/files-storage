using System;
using System.Linq;
using System.Threading.Tasks;
using FlyingDonkey.Storage.Api.Pipeline;
using FlyingDonkey.Storage.DataLayer;
using FlyingDonkey.Storage.Handlers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlyingDonkey.Storage.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IStorageHandler _storageHandler;

        public FilesController(ILogger<FilesController> logger, IStorageHandler storageHandler, IFilesInfoRepository attachmentsRepository)
        {
            _logger = logger;
            _storageHandler = storageHandler;
        }
        [HttpGet("")]
        public async Task<IActionResult> List(uint? page)
        {
            page ??= 1;
            var list = await _storageHandler.GetListOfFiles((uint)page, 10);
            return Ok(list);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            if (id == null)
                return NotFound();
            var fileInfo = await _storageHandler.DownloadFile((Guid)id);
            if (fileInfo == null)
                return NotFound();
            var contentType = "APPLICATION/octet-stream";
            return File(fileInfo.Content, contentType, $"{fileInfo.Name.Split(new char[] { '/', '\\' }).Last()}");
        }
        [HttpPost("")]
        [ServiceFilter(typeof(UploadFileValidator))]
        public async Task<IActionResult> Add(IFormFile file)
        {
            if (file == null)
                return BadRequest("File is required");
            var id = $"{Guid.NewGuid().ToString().Substring(0, 8)}.{file.FileName.Split('.').Last()}";
            var path = await _storageHandler.UploadFileToS3(file.OpenReadStream(), id, file.FileName);
            return new CreatedResult(Url.Action("Get", new { id = $"{path}" }), new { path });
        }
    }
}
