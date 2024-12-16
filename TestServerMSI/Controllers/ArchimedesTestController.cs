using Microsoft.AspNetCore.Mvc;
using TestServerMSI.Appliaction.Alogrithms;
using TestServerMSI.Appliaction.TestFunctions;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArchimedesTestController : ControllerBase
    {
        private readonly ILogger<ArchimedesTestController> _logger;
        public ArchimedesTestController(ILogger<ArchimedesTestController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetArchimedesResult")]
        public IActionResult Get()
        {
            AOA archimedes = new AOA();
            double[,] domain = { { -10, -10, -10 }, { 10, 10, 10 } };
            archimedes.Solve(new SphereTestFunction().invoke, domain, 30, 200, 2.0, 4.0, 1.0, 0.5);
            return Ok(archimedes);
        }
    }
}
