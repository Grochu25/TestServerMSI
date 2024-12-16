using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Appliaction.Interfaces;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestFunctionsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            List<Type> q = new List<Type>(from t in Assembly.GetExecutingAssembly().GetTypes()
                                              where t.IsClass && t.Namespace == "TestServerMSI.Appliaction.TestFunctions"
                                              && t.GetInterface("ITestFunction", true) != null
                                              select t);
            List<ITestFunction> tests = new List<ITestFunction>();
            foreach (var t in q)
            {
                var instance = Activator.CreateInstance(t);
                if(instance is ITestFunction)
                    tests.Add((ITestFunction)instance);
            }
            List<string> testNames = new List<string>(tests.Select(x => x.Name));

            Debug.WriteLine(q.GetType());
            return Ok(testNames);
        }
    }
}
