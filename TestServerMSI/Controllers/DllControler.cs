using Microsoft.AspNetCore.Mvc;
using TestServerMSI.Application.DLLs;
using TestServerMSI.Application.DTO;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DllControler : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        { 
            return Ok(DLLContents.GetFileContentPairList());
        }

        [HttpGet("{fileName}")]
        public IActionResult Get(string fileName)
        {
            FileContentPair? pair = DLLContents.GetFileContentPair(fileName);
            if (pair == null) 
                return NotFound();
            return Ok(pair);
        }

        [HttpPost]
        public IActionResult PostFileUpload(IFormFile file)
        {
            string result = checkFile(file);
            if (result.Equals(""))
            {
                using (FileStream stream = new FileStream("./uploadedDLLs/" + file.FileName, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Ok();
            }
            return BadRequest(result);
        }

        private string checkFile(IFormFile uploadFile)
        {
            if(Path.GetExtension(uploadFile.FileName).ToLower() != ".dll")
                return "Bad file extention. DLL files required.";
            if(uploadFile.Length > 5 * 1024 * 1024)
                return "File too large. Max size 5MB.";
            DirectoryInfo di = new DirectoryInfo("uploadedDLLs");
            foreach (var file in di.GetFiles())
                if (file.Name == uploadFile.FileName)
                    return "File with this name already exists.";
            return "";
        }

        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            if (fileName.Substring(fileName.Length - 4).ToLower() != ".dll")
                fileName += ".dll";

            DirectoryInfo di = new DirectoryInfo("uploadedDLLs");
            foreach (var file in di.GetFiles())
                if(file.Name == fileName)
                {
                    file.Delete();
                    return NoContent();
                }

            return NotFound();
        }

    }
}
