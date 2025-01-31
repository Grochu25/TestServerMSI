using System.Diagnostics;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.Services
{
    public class GenerateTextReport : IGenerateTextReport
    {
        public uint Precision { get; set; }
        public string ReportString { get; set; }
        public IOptimizationAlgorithm Alg { get; set; }
        public ITestFunction TF { get; set; }

        public string FloatFormat { get; set; }

        public GenerateTextReport()
        {
            this.Precision = 10;

            this.ReportString = String.Empty;
            this.FloatFormat = String.Empty;
        }

        public void GenerateReport(string path)
        {
            if (this.Alg == null)
            {
                Debug.WriteLine(
                    "No algorithm set. try:\n\t" +
                    "[generator as variable].Alg = [algorithm as IOptimizationAlgorithm]\n\n" +
                    "before using GenerateReport(string path)\n"
                );
                return;
            }
            this.FloatFormat = this.buildFloatFormat();

            string algName = $"Algorithm name:                        {this.Alg.Name}";
            string bestString = $"Best entity:                           {this.stringOfDoubleArray(this.Alg.XBest)}";
            string fitnessString = $"Its fitness value:                     {this.Alg.FBest.ToString(this.FloatFormat)}";
            string NumEvalString = $"Number of evaluation fitness function: {this.Alg.NumberOfEvaluationFitnessFunction.ToString()}\n";
            string testFunction = $"Test function name:                    {this.TF.Name}\n";

            string paramsInfoString = "";
            foreach (ParamInfo paramInfo in this.Alg.ParamsInfo)
            {
                paramsInfoString += this.stringOfParamInfo(paramInfo) + "\n";
            }

            string[] content = new string[] { algName, bestString, fitnessString, NumEvalString, testFunction, paramsInfoString };
            this.saveToMember(content);

            this.saveToFile(path, content);
        }

        public string buildFloatFormat()
        {
            string retVal = "0.";
            for (uint i = 0; i < this.Precision; ++i)
            {
                retVal += "0";
            }
            return retVal;
        }

        public string stringOfDoubleArray(double[] source)
        {
            string retVal = "[";
            for (int i = 0; i < source.Length; ++i)
            {
                retVal += source[i].ToString(this.FloatFormat);
                if (i < source.Length - 1)
                {
                    retVal += "; ";
                }
            }
            retVal += "]";
            return retVal;
        }

        public string stringOfParamInfo(ParamInfo paramInfo)
        {
            return String.Format(
                "Name: {0}:\n\tDescription: {1}\n\tRange:       [{2} - {3}]\n\tUsed:       {4}",
                paramInfo.Name,
                paramInfo.Description,
                paramInfo.LowerBoundary,
                paramInfo.UpperBoundary,
                this.Alg.ParametersUsedValues[paramInfo.Name]
            );
        }

        public void saveToMember(string[] content)
        {
            this.ReportString = String.Empty;
            foreach (string line in content)
            {
                this.ReportString += line + "\n";
            }
        }

        private void saveToFile(string path, string[] content)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (StreamWriter reportFile = new StreamWriter(path))
            {
                foreach (string line in content)
                {
                    reportFile.WriteLine(line);
                }
            }
        }
    }
}
