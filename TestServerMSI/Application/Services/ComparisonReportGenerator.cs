using System.Diagnostics;
using System.Runtime.InteropServices;
using TestServerMSI.Application.Alogrithms;

namespace TestServerMSI.Application.Services
{
    public class ComparisonReportGenerator
    {
        private DirectoryInfo reportDirecotry;

        private string[] algorithms;
        private double[][] bestEntities;
        private double[] fitnessValue;
        private int[] numberOfEval;
        private string[] functionName;
        public double[][] parameters { get; set; } //Dwa pierwsze to zawsze population i iterations

        public ComparisonReportGenerator(string reportDirecotryPath)
        {
            if (!Directory.Exists($"records/{reportDirecotryPath}"))
                Directory.CreateDirectory($"records/{reportDirecotryPath}");

            reportDirecotry = new DirectoryInfo($"records/{reportDirecotryPath}");
            int numberOfReports = reportDirecotry.GetFiles().ToList().Where(x => x.Extension == ".txt").Count();

            algorithms = new string[numberOfReports];
            bestEntities = new double[numberOfReports][];
            fitnessValue = new double[numberOfReports];
            numberOfEval = new int[numberOfReports];
            functionName = new string[numberOfReports];
            parameters = new double[numberOfReports][];
        }

        public void generateComparisonReport()
        {
            pullDataFromAllRaports();
            //TODO: tutaj już są pobrane wszystkie dane z raportów (wystarczy iterować te prywatne pola) index i to i-ty raport
            //dalej trzeba by zrobić PDFa z tabelką porównawczą (z parametrów najważniejsze będą population i iterations, (patrz linia 16)
        }


        private void pullDataFromAllRaports()
        {
            int index = 0;

            foreach (FileInfo file in reportDirecotry.GetFiles())
            {
                if (file.Extension == ".txt")
                {
                    using (StreamReader sr = new StreamReader(file.FullName))
                    {
                        readForIndex(file, index);
                    }
                    index++;
                }
            }
        }

        private void readForIndex(FileInfo file, int index)
        {
            using (StreamReader sr = new StreamReader(file.FullName))
            {
                string name = sr.ReadLine();
                algorithms[index] = name.Substring(name.IndexOf(":") + 1).Trim();
                string bestEntStr = sr.ReadLine();
                bestEntities[index] = pullBestEnt(bestEntStr.Substring(bestEntStr.IndexOf(":") + 1).Trim());
                string fitnessVal = sr.ReadLine();
                fitnessValue[index] = double.Parse(fitnessVal.Substring(fitnessVal.IndexOf(":") + 1).Trim());
                string numOfEval = sr.ReadLine();
                numberOfEval[index] = int.Parse(numOfEval.Substring(numOfEval.IndexOf(":") + 1).Trim());
                sr.ReadLine();
                string testFunName = sr.ReadLine();
                functionName[index] = testFunName.Substring(testFunName.IndexOf(":") + 1).Trim();
                sr.ReadLine();
                parameters[index] = readParametersFor(algorithms[index], sr);
            }
        }

        private double[] pullBestEnt(string endStr)
        {
            endStr = endStr.Substring(1, endStr.Length - 2);
            double[] result = new double[endStr.Count(x => x == ';') + 1];

            int i = 0;
            foreach (string oneDim in endStr.Split(';'))
            {
                result[i] = double.Parse(oneDim);
                i++;
            }

            return result;
        }

        private double[] readParametersFor(string algorithm, StreamReader sr)
        {
            int numberOfParams = Algorithms.getAlgorithm(algorithm).ParamsInfo.Length;
            double[] result = new double[numberOfParams];

            for (int i = 0; i < numberOfParams; i++)
            {
                sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
                string parameter = sr.ReadLine();
                result[i] = double.Parse(parameter.Substring(parameter.IndexOf(":") + 1).Trim());
            }

            return result;
        }
    }
}
