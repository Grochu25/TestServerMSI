using System.Data;
using System.Diagnostics;
using TestServerMSI.Application.Interfaces;

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
        public int iterationsMade = 0;
        public int functionsChecked = 0;
        Thread thread;

        private CalculationProcessor() {
            reports = new List<string>();
            parametersList = new List<double[]>();
        }

        public void oneAlgorithmManyFunctions(IOptimizationAlgorithm algorithm,
            double[,] domain, 
            double[][] parameters,
            ITestFunction[] functions,
            int iterationsMade = 0,
            int functionsChecked = 0)
        {
            clearLists();
            createParametersArray(parameters);
            parametersList.Sort(parametersCompare);

            thread = new Thread(() =>
            {
                CalculationsInProgress = true;
                for (int i = 0; i < functions.Length; i++)
                {
                    for (int j = iterationsMade; j < parametersList.Count; j++)
                    {
                        algorithm.Solve(functions[i].invoke, domain, parametersList[j]);
                        reports.Add(algorithm.stringReportGenerator.ReportString);
                        iterationsMade++;
                        Debug.WriteLine("BIMBO " + iterationsMade);
                        //TODO jakaś forma zapisu do pliku z CalculationProcessorController(44) raportów w razie przerwania i iterationsMade, dla wznowienia w ostatnim miejscu
                    }
                    functionsChecked++;
                    //TODO jakaś forma zapisu do pliku z CalculationProcessorController(44) functionsChecked, dla wznowienia w ostatnim miejscu
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
            IOptimizationAlgorithm[] algorithms, 
            int iterationsMade = 0,
            int functionsChecked = 0)
        {
            clearLists();
            thread = new Thread(() =>
            {
                CalculationsInProgress = true;
                for(int i= iterationsMade; i<algorithms.Length; i++)
                {
                    algorithms[i].Solve(function.invoke, domain, parameters[i]);
                    reports.Add(algorithms[i].stringReportGenerator.ReportString);
                    iterationsMade++;
                    //TODO jakaś forma zapisu do pliku z CalculationProcessorController(67) raportów w razie przerwania i iterationsMade, dla wznowienia w ostatnim miejscu
                }
                CalculationsInProgress = false;
                iterationsMade = 0;
                functionsChecked = 0;
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
