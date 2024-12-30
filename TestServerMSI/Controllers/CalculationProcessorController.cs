using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestServerMSI.Application.Alogrithms;
using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.TestFunctions;
using TestServerMSI.Application;
using TestServerMSI.Application.Alogrithms;
using TestServerMSI.Application.DTO;
using TestServerMSI.Application.TestFunctions;

namespace TestServerMSI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculationProcessorController : ControllerBase
    {
        [HttpGet("{command}")]
        public IActionResult Get(string command)
        {
            switch(command)
            {
                case "result":
                    if (CalculationProcessor.Instance.CalculationsInProgress)
                        return Ok("In progress");
                    else
                    {
                        return Ok(CalculationProcessor.Instance.reports);
                    }
                case "stop":
                    return Ok("Zaimplementować przerwanie");
                case "resume":
                    return Ok("Zaimplementować wznowienie");
                case "last":
                    return Ok("pokazać ostatnią próbę");
                default:
                    return NotFound();
            }
            
        }

        [HttpPost("oneAlgorithmManyFunctions")]
        public IActionResult Post(OneAlgorithmManyFunctionsDTO oamf)
        {
            if (CalculationProcessor.Instance.CalculationsInProgress == false)
            {
                IOptimizationAlgorithm? algorithm = Algorithms.getAlgorithm(oamf.AlgorithmName);
                List<ITestFunction> tests = new List<ITestFunction>();
                foreach(var func in oamf.TestFunctionNames)
                    if(TestFunctions.getTestFunction(func) != null)
                        tests.Add(TestFunctions.getTestFunction(func));

                Debug.WriteLine("TESTS " + tests.Count);
                CalculationProcessor.Instance.oneAlgorithmManyFunctions(algorithm,
                    oamf.domainAsMulti(), oamf.Parameters, tests.ToArray());
            }
            else
                return Ok("In progress");

            return Ok();
        }

        [HttpPost("oneFunctionManyAlgorithms")]
        public IActionResult Post(OneFunctionManyAlgorithmsDTO oamf)
        {
            if (CalculationProcessor.Instance.CalculationsInProgress == false)
            {
                ITestFunction? testFunction = TestFunctions.getTestFunction(oamf.TestFunctionName);
                List<IOptimizationAlgorithm> algorithms = new List<IOptimizationAlgorithm>();
                foreach (var alg in oamf.AlgorithmNames)
                    if (Algorithms.getAlgorithm(alg) != null)
                        algorithms.Add(Algorithms.getAlgorithm(alg));


                CalculationProcessor.Instance.oneFunctionManyAlgoritms(testFunction,
                    oamf.domainAsMulti(), oamf.Parameters, algorithms.ToArray());
            }
            else
                return Ok("In progress");

            return Ok();
        }
    }
}
