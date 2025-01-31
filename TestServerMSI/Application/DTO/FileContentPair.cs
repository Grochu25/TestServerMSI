using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.DTO
{
    public class FileContentPair
    {
        public string FileName {  get; set; }
        public List<IOptimizationAlgorithm> AlgorithmList { get; set; }
        public List<ITestFunction> FunctionList { get; set; }

        public FileContentPair(string fileName) {
            FileName = fileName;
            AlgorithmList = new List<IOptimizationAlgorithm>();
            FunctionList = new List<ITestFunction>();
        }

        public FileContentPair()
        {
            FileName = "";
            AlgorithmList = new List<IOptimizationAlgorithm>();
            FunctionList = new List<ITestFunction>();
        }
    }
}
