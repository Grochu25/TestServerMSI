using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.DLLs
{
    public class DLLAlgorithms
    {
        public static List<IOptimizationAlgorithm> getDLLAlgorithmList()
        {

            Assembly assembly;
            List<IOptimizationAlgorithm> algorithms = new List<IOptimizationAlgorithm>();

            DirectoryInfo di = new DirectoryInfo("uploadedDLLs");
            foreach(var file in di.GetFiles())
            {
                if(file.Extension != ".dll")
                    continue;
                assembly = Assembly.LoadFrom(file.FullName);
                Type? fitnessType = assembly.GetType("fitnessFunction");
                foreach (var x in from t in assembly.GetTypes() where t.IsClass && t.GetInterface("IOptimizationAlgorithm", true) != null select t)
                {
                    AlgorithmCapsule capsule = new AlgorithmCapsule(Activator.CreateInstance(x), fitnessType);
                    capsule.Name = file.Name.Substring(0, file.Name.IndexOf('.')) + ":" + capsule.Name;
                    algorithms.Add(capsule);
                }
            }

            return algorithms;
        }

        public static IOptimizationAlgorithm? getDLLAlgorithm(string algorithmName)
        {
            var algorithms = getDLLAlgorithmList();
            foreach (var alg in algorithms)
            {
                if (alg.Name == algorithmName)
                    return alg;
            }
            return null;
        }
    }
}
