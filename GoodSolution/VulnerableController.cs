using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Security_issue
{
    class VulnerableController : ControllerBase
    {
        ...

        private string[] allowedExtensions = { ".pdf", ".doc" };
        private readonly long _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        private readonly int _fileNameLimit = 255;
        /// <summary>
        /// Загрузка файлов на сервер
        /// </summary>
        /// <param name="files">Список файлов для загрузки</param>
        /// <returns>Число файлов и размер</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {                
                var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();

                if (formFile.FileName.Length > _fileNameLimit)
                {
                    return "Name of your file is too long";
                }

                if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
                {
                    return "Your file is not allowed to be uploaded";
                }

                if (formFile.Length > _fileSizeLimit)
                {
                    return "Size of your file exceeds the limit";
                }
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetRandomFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { count = files.Count, size });
        }

        ...
    }
}
