using TestServerMSI.Application.DTO;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.DLLs
{
    public class DLLContents
    {
        public static List<FileContentPair> GetFileContentPairList()
        {
            List<FileContentPair> fileContentPairs = new List<FileContentPair>();
            List<IOptimizationAlgorithm> optimizationAlgorithms = DLLAlgorithms.getDLLAlgorithmList();
            List<ITestFunction> testFunctions = DLLTestFunctions.getDLLTestFunctionList();
            List<string> names = new List<string>();

            while (optimizationAlgorithms.Count > 0)
            {
                string fileName = optimizationAlgorithms.ElementAt(0).Name;
                fileName = fileName.Substring(0, fileName.IndexOf(':'));
                if (!names.Contains(fileName))
                {
                    names.Add(fileName);
                    fileContentPairs.Add(new FileContentPair(fileName));
                }
                fileContentPairs.ElementAt(names.IndexOf(fileName)).AlgorithmList.Add(optimizationAlgorithms.ElementAt(0));
                optimizationAlgorithms.RemoveAt(0);
            }
            while (testFunctions.Count > 0)
            {
                string fileName = testFunctions.ElementAt(0).Name;
                fileName = fileName.Substring(0, fileName.IndexOf(':'));
                if (!names.Contains(fileName))
                {
                    names.Add(fileName);
                    fileContentPairs.Add(new FileContentPair(fileName));
                }
                fileContentPairs.ElementAt(names.IndexOf(fileName)).FunctionList.Add(testFunctions.ElementAt(0));
                testFunctions.RemoveAt(0);
            }

            return fileContentPairs;
        }

        public static FileContentPair? GetFileContentPair(string fileName)
        {
            List<FileContentPair> fileContentPairs = GetFileContentPairList();
            foreach (FileContentPair pair in fileContentPairs)
                if(pair.FileName == fileName)
                    return pair;
            return null;
        }
    }
}
