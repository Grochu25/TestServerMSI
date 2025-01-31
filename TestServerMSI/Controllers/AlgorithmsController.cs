using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.Alogrithms;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlgorithmsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Algorithms.getAlgorithmList());
        }

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            var algorithm = Algorithms.getAlgorithm(name);
            if (name == null)
                return NotFound();
            return Ok(algorithm);
        }
    }
}
