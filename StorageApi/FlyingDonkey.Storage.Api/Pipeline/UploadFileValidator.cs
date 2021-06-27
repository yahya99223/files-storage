using System;
using System.Collections.Generic;
using System.Linq;
using FlyingDonkey.Storage.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FlyingDonkey.Storage.Api.Pipeline
{
    public class UploadFileValidator : Attribute, IResourceFilter
    {
        private readonly SupportedFilesSettings _filesSettings;
        public UploadFileValidator(IOptions<Settings> settings)
        {
            _filesSettings = settings.Value.SupportedFilesSettings;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var validationTotalErrors = new List<string>();

            if (context.HttpContext.Request.ContentLength > _filesSettings.MaxFileSizeInMb * 1024 * 1024)
            {
                validationTotalErrors.Add("Size Limit Exceeded");
            }
            else if (context.HttpContext.Request.ContentLength == 0)
            {
                validationTotalErrors.Add("File Should be provided");
            }
            else
            {
                var files = context.HttpContext.Request.Form.Files;
                var file = files.FirstOrDefault(x => x.Name == "file");
                if (file == null)
                {
                    validationTotalErrors.Add("File Should be provided");
                }
                else
                {
                    if (files.Count > 1)
                    {
                        validationTotalErrors.Add("Only one file can be uploaded at a time");
                    }
                    else if (!_filesSettings.FileTypes.Select(x => x.ToLower()).Contains(file.ContentType.ToLower()))
                    {
                        validationTotalErrors.Add($"{file.ContentType} is not supported");
                    }
                }
            }
            if (validationTotalErrors.Any())
            {
                context.Result = new BadRequestObjectResult(validationTotalErrors);
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            ;
        }
    }
}
