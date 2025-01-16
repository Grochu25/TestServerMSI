using TestServerMSI.Application.Interfaces;
using System.Diagnostics;

namespace TestServerMSI.Application.Services
{
    public class StateReader : IStateReader
    {
        public StateReader() { }
        public void LoadFromFileStateOfAlgorithm(string path)
        {
            Debug.WriteLine("READING FILE");
            StreamReader sr = new StreamReader(path);
            // Num of Iterations
            String line = sr.ReadLine();
            CalculationProcessor.Instance.CurrentAlgorithm.CurrentIteration = int.Parse(line);
            // Num of Fitness Exec
            line = sr.ReadLine();
            CalculationProcessor.Instance.CurrentAlgorithm.NumberOfEvaluationFitnessFunction = int.Parse(line);
            // Pop and values
            line = sr.ReadLine();
            int k = 0;
            while (line != null)
            {
                string[] val = line.Split(' ');
                double[] agent = new double[val.Length-1];
                for (int i = 0; i < val.Length - 1; i++)
                {
                    agent[i] = double.Parse(val[i]);
                }
                CalculationProcessor.Instance.CurrentAlgorithm.Population[k] = agent;
                CalculationProcessor.Instance.CurrentAlgorithm.PopulationValues[k] = double.Parse(val[val.Length-1]);
                k++;
                line = sr.ReadLine();
            }
            sr.Close();
        }
    }
}
