using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Application.Alogrithms;
using TestServerMSI.Application.DTO;
using TestServerMSI.Application;
using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.TestFunctions;
using TestServerMSI.Application.DLLs;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class testdllControler : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            Assembly assembly = Assembly.LoadFrom("Application/DLLs/ClassLibraryTest.dll");
            
            
            return Ok();
        }

        [HttpPost("oneAlgorithmManyFunctions")]
        public IActionResult Post(OneAlgorithmManyFunctionsDTO oamf)
        {
            Assembly assembly = Assembly.LoadFrom("Application/DLLs/ClassLibraryTest.dll");

            List<Type> types = new List<Type>(from t in assembly.GetTypes() where t.Name == "fitnessFunction" select t);
            Type? fitness = types[0];

            IOptimizationAlgorithm? algDll = null;
            foreach (var x in from t in assembly.GetTypes() where t.IsClass && t.GetInterface("IOptimizationAlgorithm", true) != null select t)
                algDll = new AlgorithmCapsule(Activator.CreateInstance(x), fitness);

            if (CalculationProcessor.Instance.CalculationsInProgress == false)
            {
                new DirectoryInfo("savedAlgorithms").GetFiles().ToList().ForEach(f => f.Delete());
                QueueSavers.saveOAMFdtoToFile(oamf);

                List<ITestFunction?> tests = new List<ITestFunction?>();
                foreach (var func in oamf.TestFunctionNames)
                    if (TestFunctions.getTestFunction(func) != null)
                        tests.Add(TestFunctions.getTestFunction(func));

                Debug.WriteLine("TESTS " + tests.Count);
                if (algDll != null && !tests.Contains(null))
                {
                    CalculationProcessor.Instance.oneAlgorithmManyFunctions(algDll,
                        oamf.domainAsMulti(), oamf.Parameters, tests.ToArray());
                    return Ok();
                }
                else
                    return StatusCode(500);
            }
            else
                return Ok("In progress");

            return Ok();
        }
    }
}
