using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildRiftWebAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace WildRiftWebAPI
{
    [Route("file")]
    [Authorize(Roles = "Admin")]
    public class FileController : ControllerBase
    {
        [HttpGet]
        [ResponseCache(Duration = 1200, VaryByQueryKeys = new[] { "fileName" })]
        public ActionResult GetFile([FromQuery] string fileName)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = $"{rootPath}/PrivateFiles/{fileName}";

            //pełna scieżka aby nie mylił z kontrolerem
            var fileExists = System.IO.File.Exists(filePath);

            if (!fileExists) return NotFound();

            var fileContents = System.IO.File.ReadAllBytes(filePath);

            //musimy dynamicznie zczytać rodzaj pliku
            var contentProvider = new FileExtensionContentTypeProvider();

            //pod zmienną "contentType" będzie krył się typ pliku
            contentProvider.TryGetContentType(fileName, out string contentType);

            return File(fileContents, contentType, fileName); 
        }

        [HttpPost]
        public ActionResult Upload([FromForm] IFormFile file)
        {
            if (file is not null && file.Length > 0)
            {
                var rootPath = Directory.GetCurrentDirectory();
                var fileName = file.FileName;
                var fullPath = $"{rootPath}/PrivateFiles/{fileName}";

                using var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);
                return Ok();
            }
            return BadRequest();
        }
    }
}
