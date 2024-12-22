using TestServerMSI.Appliaction.Interfaces;
using TestServerMSI.Application.Alogrithms;
using TestServerMSI.Application.TestFunctions;

namespace TestServerMSI.Application.DTO
{
    public class OneFunctionManyAlgorithmsDTO
    {
        public string TestFunctionName { get; set; }
        public double[][] Domain { get; set; }
        public double[][] Parameters { get; set; }
        public string[] AlgorithmNames { get; set; }

        public OneFunctionManyAlgorithmsDTO() {
            TestFunctionName = "";
            AlgorithmNames = new string[0];
            Domain = new double[0][];
            Parameters = new double[0][];
        }

        public OneFunctionManyAlgorithmsDTO(string testFunction, 
            double[,] domain, double[][] parameters, string[] algorithms)
        {
            TestFunctionName = testFunction;
            Domain = fromMultiToJagged(domain);
            Parameters = parameters;
            AlgorithmNames = algorithms;
        }
        public OneFunctionManyAlgorithmsDTO(string testFunction,
            double[][] domain, double[][] parameters, string[] algorithms)
        {
            TestFunctionName = testFunction;
            Domain = domain;
            Parameters = parameters;
            AlgorithmNames = algorithms;
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
