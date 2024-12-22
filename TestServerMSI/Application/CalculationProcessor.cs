using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using TestServerMSI.Appliaction.Interfaces;
using TestServerMSI.Application.Alogrithms;
using TestServerMSI.Application.TestFunctions;

namespace TestServerMSI.Application
{
    public class CalculationProcessor
    {
        private static CalculationProcessor? _instance = null;
        public static CalculationProcessor Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CalculationProcessor();
                return _instance;
            }
        }

        public bool CalculationsInProgress = false;
        public List<string> reports { get; private set;}
        public List<IOptimizationAlgorithm> algoritms { get; private set; }
        public List<ITestFunction> testFunctions { get; private set; }
        public double[][] parameters { get; private set; }
        Thread thread;

        private CalculationProcessor() {
            reports = new List<string>();
            algoritms = new List<IOptimizationAlgorithm>();
            testFunctions = new List<ITestFunction>();
        }

        public void oneAlgorithmManyFunctions(IOptimizationAlgorithm algorithm,
            double[,] domain, 
            double[][] parameters,
            params ITestFunction[] functions)
        {
            clearLists();
            algoritms.Add(algorithm);
            testFunctions.AddRange(functions.ToList());
            this.parameters = parameters;

            thread = new Thread(() =>
            {
                CalculationsInProgress = true;
                for (int i = 0; i < functions.Length; i++)
                {
                    algorithm.Solve(functions[i].invoke, domain, parameters[i]);
                    reports.Add(algorithm.stringReportGenerator.ReportString);
                }
                CalculationsInProgress = false;
            });
            thread.Start();
        }

        public void oneFunctionManyAlgoritms(ITestFunction function,
            double[,] domain,
            double[][] parameters,
            params IOptimizationAlgorithm[] algorithms)
        {
            clearLists();
            testFunctions.Add(function);
            algoritms.AddRange(algorithms.ToList());
            this.parameters = parameters;

            thread = new Thread(() =>
            {
                CalculationsInProgress = true;
                for(int i=0; i<algorithms.Length; i++)
                {
                    algorithms[i].Solve(function.invoke, domain, parameters[i]);
                    reports.Add(algorithms[i].stringReportGenerator.ReportString);
                }
                CalculationsInProgress = false;
            });
            thread.Start();
        }

        private void clearLists()
        {
            reports.Clear();
            testFunctions.Clear();
            algoritms.Clear();
        }
    }
}
