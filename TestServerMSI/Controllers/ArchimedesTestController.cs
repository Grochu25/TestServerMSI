using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Appliaction.Alogrithms;
using TestServerMSI.Appliaction.Interfaces;
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

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            DirectoryInfo dllDirectory = new DirectoryInfo("./Application/DLLs");
            List<string> dllFileList = dllDirectory.GetFiles("*.dll").ToList().Select(x => x.Name).ToList();
            List<object> algorithms = new List<object>();

            foreach (string dllFile in dllFileList)
            {
                List<Type> q = new List<Type>(from t in Assembly.LoadFile(dllDirectory.FullName +"\\"+ dllFile).GetTypes()
                                              where t.IsClass
                                              && t.GetInterface("IOptimizationAlgorithm", true) != null
                                              select t);
                foreach (var t in q)
                {
                    var instance = Activator.CreateInstance(t);
                    Debug.WriteLine("BBB ", instance);
                    //if (instance is IOptimizationAlgorithm)
                    algorithms.Add((IOptimizationAlgorithm)instance);
                }
            }

            return Ok(algorithms);
        }
    }
}
