using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.Alogrithms
{
    public class Algorithms
    {
        public static List<IOptimizationAlgorithm> getAlgorithmList()
        {
            List<Type> types = new List<Type>(from t in Assembly.GetExecutingAssembly().GetTypes()
                                          where t.IsClass && t.Namespace == "TestServerMSI.Appliaction.Alogrithms"
                                          && t.GetInterface("IOptimizationAlgorithm", true) != null
                                          select t);
            List<IOptimizationAlgorithm> algorithms = new List<IOptimizationAlgorithm>();
            foreach (var t in types)
            {
                Debug.WriteLine(t.Name);
                var instance = Activator.CreateInstance(t);
                if (instance is IOptimizationAlgorithm)
                    algorithms.Add((IOptimizationAlgorithm)instance);
            }
            return algorithms;
        }

        public static IOptimizationAlgorithm? getAlgorithm(string algorithmName)
        {
            var algorithms = getAlgorithmList();
            foreach (var alg in algorithms)
            {
                if(alg.Name == algorithmName)
                    return alg;
            }
            return null;
        }
    }
}
