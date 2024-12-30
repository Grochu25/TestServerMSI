using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.DTO
{
    public class OneAlgorithmManyFunctionsDTO
    {
        public string AlgorithmName { get; set; }
        public double[][] Domain { get; set; }
        public double[][] Parameters { get; set; }
        public string[] TestFunctionNames { get; set; }

        public OneAlgorithmManyFunctionsDTO() 
        {
            AlgorithmName = "";
            TestFunctionNames = new string[1];
            Domain = new double[2][];
            Parameters = new double[1][];
        }

        public OneAlgorithmManyFunctionsDTO(string algorithm, 
            double[,] domain, double[][] parameters, string[] testFunctions)
        {
            AlgorithmName = algorithm;
            Domain = fromMultiToJagged(domain);
            Parameters = parameters;
            TestFunctionNames = testFunctions;
        }
        public OneAlgorithmManyFunctionsDTO(string algorithm,
            double[][] domain, double[][] parameters, string[] testFunctions)
        {
            AlgorithmName = algorithm;
            Domain = domain;
            Parameters = parameters;
            TestFunctionNames = testFunctions;
        }

        public double[,] domainAsMulti()
        {
            return fromJaggedToMulti(Domain);
        }

        private double[][] fromMultiToJagged(double[,] multi)
        {
            double[][] jagged = new double[multi.GetLength(0)][];
            for (int i = 0; i < multi.GetLength(0); i++)
            {
                jagged[i] = new double[multi.GetLength(1)];
                for (int j = 0; j < multi.GetLength(1); j++)
                    jagged[i][j] = multi[i, j];
            }
            return jagged;
        }

        private double[,] fromJaggedToMulti(double[][] jagged)
        {
            double[,] multi = new double[jagged.Length,jagged[0].Length];
            for (int i = 0; i < multi.GetLength(0); i++)
                for (int j = 0; j < multi.GetLength(1); j++)
                    multi[i,j] = jagged[i][j];
            return multi;
        }
    }
}
