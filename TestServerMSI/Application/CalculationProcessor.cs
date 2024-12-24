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
        public List<double[]> parametersList { get; private set; }
        Thread thread;

        private CalculationProcessor() {
            reports = new List<string>();
            parametersList = new List<double[]>();
        }

        public void oneAlgorithmManyFunctions(IOptimizationAlgorithm algorithm,
            double[,] domain, 
            double[][] parameters,
            params ITestFunction[] functions)
        {
            clearLists();
            createParametersArray(parameters);
            parametersList.Sort(parametersCompare);

            thread = new Thread(() =>
            {
                CalculationsInProgress = true;
                for (int i = 0; i < functions.Length; i++)
                {
                    for (int j = 0; j < parametersList.Count; j++)
                    {
                        algorithm.Solve(functions[i].invoke, domain, parametersList[j]);
                        reports.Add(algorithm.stringReportGenerator.ReportString);
                    }
                }
                CalculationsInProgress = false;
            });
            thread.Start();
        }

        private void createParametersArray(double[][] parameters, int paramNumber = 0)
        {
            double[] row = new double[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                row[i] = parameters[i][0];
            if(!parameters.Contains(row))
                parametersList.Add(row);

            for (int i = paramNumber; i < parameters.Length; i++)
            {
                if (parameters[i][0] + parameters[i][2] <= parameters[i][1] && parameters[i][2]>0) {
                    double[][] newParams = parameters.Select(s => s.ToArray()).ToArray();
                    newParams[i][0] += parameters[i][2];

                    createParametersArray(newParams, i);
                }
            }
        }

        private int parametersCompare(double[] first, double[] second)
        {
            for(int i=0;i<first.Length;i++)
            {
                if (first[i] > second[i])
                    return 1;
                else if (first[i] < second[i])
                    return -1;
            }
            return 0;
        }

        public void oneFunctionManyAlgoritms(ITestFunction function,
            double[,] domain,
            double[][] parameters,
            params IOptimizationAlgorithm[] algorithms)
        {
            clearLists();
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
            parametersList.Clear();
        }
    }
}
