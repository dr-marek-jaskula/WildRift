using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
namespace WildRiftWebAPI;

[Route("file")]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
public class FileController : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 1200, VaryByQueryKeys = new[] { "fileName" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Get([FromQuery] string fileName)
    {
        var rootPath = Directory.GetCurrentDirectory();
        var filePath = $"{rootPath}/PrivateFiles/{fileName}";
        var fileExists = System.IO.File.Exists(filePath);

        if (!fileExists)
            return NotFound();

        var fileContents = System.IO.File.ReadAllBytes(filePath);
        var contentProvider = new FileExtensionContentTypeProvider();
        contentProvider.TryGetContentType(fileName, out string contentType);

        return File(fileContents, contentType, fileName);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
