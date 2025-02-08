using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using TestServerMSI.Application.Services;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetList()
        {
            DirectoryInfo di = new DirectoryInfo("records");
            return Ok(di.GetDirectories().Select(dir=>dir.Name).ToArray());
        }

        [HttpGet("last")]
        public IActionResult GetLatestReport()
        {
            DirectoryInfo[] list = new DirectoryInfo("records").GetDirectories();
            if (list.Length == 0)
                return NotFound();

            DirectoryInfo theLatest = list[0];
            for(int i=1; i<list.Length; i++)
            {
                if (theLatest.CreationTime < list[i].CreationTime)
                    theLatest = list[i];
            }

            MemoryStream memoryStream = new MemoryStream();
            ZipFile.CreateFromDirectory(theLatest.FullName, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            

            return File(memoryStream, "application/zip", theLatest.Name);
        }

        [HttpGet("{name}")]
        public IActionResult GetReportByName(string name)
        {
            DirectoryInfo dir = new DirectoryInfo($"records/{name}");
            if (!dir.Exists)
                return NotFound();

            MemoryStream memoryStream = new MemoryStream();
            ZipFile.CreateFromDirectory(dir.FullName, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);


            return File(memoryStream, "application/zip", dir.Name);
        }

        [HttpDelete("{name}")]
        public IActionResult DeleteReportByName(string name)
        {
            DirectoryInfo dir = new DirectoryInfo($"records/{name}");
            if (!dir.Exists)
                return NotFound();

            dir.Delete(true);


            return NoContent();
        }
    }
}
