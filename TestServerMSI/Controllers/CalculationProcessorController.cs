using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestServerMSI.Application.Alogrithms;
using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.TestFunctions;
using TestServerMSI.Application;
using TestServerMSI.Application.DTO;
using System.IO;

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
                    CalculationProcessor.Instance.stopCalculations();
                    return Ok();
                case "resume":
                    return resumeSavedState();
                case "last":
                    return Ok("pokazać ostatnią próbę");
                case "downloadSaved":
                    return Ok("pobieranie zapisanego stanu");
                default:
                    return NotFound();
            }
            
        }

        private IActionResult resumeSavedState()
        {
            if (!Directory.Exists("savedAlgorithms")) Directory.CreateDirectory("savedAlgorithms");
            List<string> savedFiles = new DirectoryInfo("savedAlgorithms").GetFiles().Select(f => f.Name).ToList();
            if (savedFiles.Contains("OAMF.dto"))
            {
                OneAlgorithmManyFunctionsDTOExtended? oamf = QueueSavers.readOAMFdtoFromFile();
                if (oamf != null)
                {
                    runOAMF(oamf.OneAlgorithmManyFunctions, oamf.IterationsMade, oamf.FunctionsChecked);
                    return Ok("Resumed One Algorithm Many Functions");
                }
                return Ok("Failed to resume");
            }
            else if (savedFiles.Contains("OFMA.dto"))
            {
                OneFunctionManyAlgorithmsDTOExtended? ofma = QueueSavers.readOFMAdtoFromFile();
                if (ofma != null)
                {
                    runOFMA(ofma.OneFunctionManyAlgorithms, ofma.IterationsMade, ofma.AlgorithmsChecked);
                    return Ok("Resumed One Function Many Algorithms");
                }
                return Ok("Failed to resume");
            }
            return Ok("No saved states");
        }

        [HttpPost("oneAlgorithmManyFunctions")]
        public IActionResult Post(OneAlgorithmManyFunctionsDTO oamf)
        {
            if (CalculationProcessor.Instance.CalculationsInProgress == false)
            {
                if (!Directory.Exists("savedAlgorithms")) Directory.CreateDirectory("savedAlgorithms");
                new DirectoryInfo("savedAlgorithms").GetFiles().ToList().ForEach(f => f.Delete());
                QueueSavers.saveOAMFdtoToFile(oamf);
                if(!runOAMF(oamf))
                    return StatusCode(500);
            }
            else
                return Ok("In progress");

            return Ok();
        }

        [HttpPost("oneFunctionManyAlgorithms")]
        public IActionResult Post(OneFunctionManyAlgorithmsDTO ofma)
        {
            if (CalculationProcessor.Instance.CalculationsInProgress == false)
            {
                if (!Directory.Exists("savedAlgorithms")) Directory.CreateDirectory("savedAlgorithms");
                new DirectoryInfo("savedAlgorithms").GetFiles().ToList().ForEach(f => f.Delete());
                QueueSavers.saveOFMAdtoToFile(ofma);
                if(!runOFMA(ofma))
                    return StatusCode(500);
            }
            else
                return Ok("In progress");
            
            return Ok();
        }

        private bool runOAMF(OneAlgorithmManyFunctionsDTO oamf, int iterationsMade = 0, int functionsChecked = 0)
        {
            IOptimizationAlgorithm? algorithm = Algorithms.getAlgorithm(oamf.AlgorithmName);
            List<ITestFunction?> tests = new List<ITestFunction?>();
            foreach (var func in oamf.TestFunctionNames)
                if (TestFunctions.getTestFunction(func) != null)
                    tests.Add(TestFunctions.getTestFunction(func));

            Debug.WriteLine("TESTS " + tests.Count);
            if (algorithm != null && !tests.Contains(null))
            {
                CalculationProcessor.Instance.oneAlgorithmManyFunctions(algorithm,
                    oamf.domainAsMulti(), oamf.Parameters, tests.ToArray(), iterationsMade, functionsChecked);
                return true;
            }
            else
                return false;
        }

        private bool runOFMA(OneFunctionManyAlgorithmsDTO ofma, int iterationsMade = 0, int algorithmsChecked = 0)
        {
            ITestFunction? testFunction = TestFunctions.getTestFunction(ofma.TestFunctionName);
            List<IOptimizationAlgorithm?> algorithms = new List<IOptimizationAlgorithm?>();
            foreach (var alg in ofma.AlgorithmNames)
                if (Algorithms.getAlgorithm(alg) != null)
                    algorithms.Add(Algorithms.getAlgorithm(alg));


            if (testFunction != null && !algorithms.Contains(null))
            {
                CalculationProcessor.Instance.oneFunctionManyAlgoritms(testFunction,
                    ofma.domainAsMulti(), ofma.Parameters, algorithms.ToArray(), iterationsMade, algorithmsChecked);
                return true;
            }
            else
                return false;
        }
    }
}
