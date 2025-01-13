using System.Data;
using System.Diagnostics;
using TestServerMSI.Application.Alogrithms;
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
        private bool stop = false;
        private Thread thread;

        public IOptimizationAlgorithm CurrentAlgorithm;

        private CalculationProcessor() {
            reports = new List<string>();
            parametersList = new List<double[]>();
        }

        public void stopCalculations()
        {
            stop = true;
            if(CurrentAlgorithm != null)
                CurrentAlgorithm.Stop = true;
            thread.Interrupt();
        }

        public void oneAlgorithmManyFunctions(IOptimizationAlgorithm algorithm,
            double[,] domain, 
            double[][] parameters,
            ITestFunction[] functions,
            int iterationsMade = 0,
            int functionsChecked = 0)
        {
            stop = false;
            clearLists();
            saveIterationsToFile("savedAlgorithms/OAMF.dto", iterationsMade);
            saveFunctionsCheckedToFile("savedAlgorithms/OAMF.dto", functionsChecked);
            createParametersArray(parameters);
            parametersList.Sort(parametersCompare);
            CurrentAlgorithm = algorithm;
            thread = new Thread(() =>
            {
                CalculationsInProgress = true;
                for (int i = functionsChecked; i < functions.Length; i++)
                {
                    for (int j = iterationsMade; j < parametersList.Count; j++)
                    {
                        if (stop) return;
                        algorithm.Solve(functions[i].invoke, domain, parametersList[j]);
                        if (stop) return;
                        reports.Add(algorithm.stringReportGenerator.ReportString);
                        iterationsMade++;
                        saveIterationsToFile("savedAlgorithms/OAMF.dto", iterationsMade);
                    }
                    functionsChecked++;
                    iterationsMade = 0;
                    saveIterationsToFile("savedAlgorithms/OAMF.dto", iterationsMade);
                    saveFunctionsCheckedToFile("savedAlgorithms/OAMF.dto", functionsChecked);
                }
                CalculationsInProgress = false;
                if (File.Exists("savedAlgorithms/OAMF.dto"))
                    File.Delete("savedAlgorithms/OAMF.dto");
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
            int algorithmsChecked = 0)
        {
            stop = false;
            clearLists();
            saveIterationsToFile("savedAlgorithms/OFMA.dto", iterationsMade);
            saveFunctionsCheckedToFile("savedAlgorithms/OFMA.dto", algorithmsChecked);
            thread = new Thread(() =>
            {
                CalculationsInProgress = true;
                for(int i= iterationsMade; i<algorithms.Length; i++)
                {
                    if (stop) return;
                    CurrentAlgorithm = algorithms[i];
                    if (stop) return;
                    Debug.WriteLine("Current Algorithm has been set");
                    algorithms[i].Solve(function.invoke, domain, parameters[i]);
                    reports.Add(algorithms[i].stringReportGenerator.ReportString);
                    iterationsMade++;
                    saveIterationsToFile("savedAlgorithms/OFMA.dto", iterationsMade);
                }
                CalculationsInProgress = false;
                if (File.Exists("savedAlgorithms/OFMA.dto"))
                    File.Delete("savedAlgorithms/OFMA.dto");
            });
            thread.Start();
        }

        private void clearLists()
        {
            reports.Clear();
            parametersList.Clear();
        }

        private void saveIterationsToFile(string path, int iterations)
        {
            List<string> lines = new List<string>();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string? line = sr.ReadLine();
                    while (line != null)
                    {
                        lines.Add(line);
                        line = sr.ReadLine();
                    }
                }
            }
            lines[lines.Count - 2] = iterations.ToString();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach(string line in lines)
                        sw.WriteLine(line);
                    sw.Flush();
                }
            }
        }

        private void saveFunctionsCheckedToFile(string path, int functionsChecked)
        {
            List<string> lines = new List<string>();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string? line = sr.ReadLine();
                    while (line != null)
                    {
                        lines.Add(line);
                        line = sr.ReadLine();
                    }
                }
            }
            lines[lines.Count - 1] = functionsChecked.ToString();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (string line in lines)
                        sw.WriteLine(line);
                    sw.Flush();
                }
            }
        }
    }
}
