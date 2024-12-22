using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Appliaction.Interfaces;
using TestServerMSI.Application.TestFunctions;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestFunctionsController : ControllerBase
    {
        List<ITestFunction> testFunctions;
        List<string> testNames;

        public TestFunctionsController()
        { 
            testFunctions = TestFunctions.getTestFunctionList();
            testNames = new List<string>(testFunctions.Select(x => x.Name));
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(testFunctions);
        }

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            var testFunction = TestFunctions.getTestFunction(name);
            if (testFunction == null)
                return NotFound();
            return Ok(testFunction);
        }
    }
}
