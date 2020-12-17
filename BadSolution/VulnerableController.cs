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

        private string[] forbiddenExtensions = { ".exe", ".rar", ".jsp", ".gif", ".html" };
        private readonly int _fileNameLimit = 255;

        /// <summary>
        /// Загрузка файлов на сервер
        /// </summary>
        /// <param name="files">Список файлов для загрузки</param>
        /// <returns>Число файлов и размер</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {                
                var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || forbiddenExtensions.Contains(ext))
                {
                    return "Your file is not allowed to be uploaded";
                }
                if (formFile.FileName.Length > _fileNameLimit)
                {
                    return "Name of your file is too long";
                }
                foreach (var formfile in files)
                {
                    string path = "/Files/" + formfile.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await formfile.CopyToAsync(fileStream);
                    }
                }
            }

            return Ok(new { count = files.Count, size });
        }

        ...
    }
}
