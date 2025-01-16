using TestServerMSI.Application.Interfaces;
using System.Diagnostics;

namespace TestServerMSI.Application.Services
{
    public class StateWriter : IStateWriter
    {
        public StateWriter() { }
        public void SaveToFileStateOfAlgorithm(string path)
        {
            // Iteration number
            var str = CalculationProcessor.Instance.CurrentAlgorithm.CurrentIteration.ToString();
            str += "\n";
            // Fitness Function times
            str += CalculationProcessor.Instance.CurrentAlgorithm.NumberOfEvaluationFitnessFunction;
            str += "\n";
            // Population
            for (int i = 0; i < CalculationProcessor.Instance.CurrentAlgorithm.Population.Length; i++)
            {
                // X
                for (int j = 0; j < CalculationProcessor.Instance.CurrentAlgorithm.Population[i].Length; j++)
                {
                    //Debug.WriteLine("Population read ("+i+","+j+"): "+CalculationProcessor.Instance.CurrentAlgorithm.Population[i][j]);
                    str += CalculationProcessor.Instance.CurrentAlgorithm.Population[i][j];
                    str += " ";
                }
                // Y
                str += CalculationProcessor.Instance.CurrentAlgorithm.PopulationValues[i];
                str += "\n";
            }
            StreamWriter sw = new StreamWriter(path);
            sw.Write(str);
            sw.Close();
        }
    }
}
