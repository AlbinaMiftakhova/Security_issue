using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Security_issue
{
    class VulnerableController : ControllerBase
    {
        ...

        /// <summary>
        /// Загрузка файлов на сервер
        /// </summary>
        /// <param name="files">Список файлов для загрузки</param>
        /// <returns>Число файлов и размер</returns>
        [HttpPut]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formfile in files)
            {
                string path = "/Files/" + formfile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await formfile.CopyToAsync(fileStream);
                }
            }
            return Ok(new { count = files.Count, size });
        }

        ...
    }
}
