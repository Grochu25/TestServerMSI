using System.Reflection;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.DLLs
{
    public class DLLTestFunctions
    {
        public static List<ITestFunction> getDLLTestFunctionList()
        {
            Assembly assembly;
            List<ITestFunction> testFunctions = new List<ITestFunction>();

            DirectoryInfo di = new DirectoryInfo("uploadedDLLs");
            foreach (var file in di.GetFiles())
            {
                if (file.Extension != ".dll")
                    continue;
                assembly = Assembly.LoadFrom(file.FullName);
                foreach (var x in from t in assembly.GetTypes() where t.IsClass && t.GetInterface("ITestFunction", true) != null select t)
                {
                    TestFunctionCapsule capsule = new TestFunctionCapsule(Activator.CreateInstance(x));
                    capsule.Name = file.Name.Substring(0, file.Name.IndexOf('.')) + ":" + capsule.Name;
                    testFunctions.Add(capsule);
                }
            }

            return testFunctions;
        }

        public static ITestFunction? getDLLTestFunction(string testFunctionName)
        {
            var functions = getDLLTestFunctionList();
            foreach (var func in functions)
            {
                if (func.Name == testFunctionName)
                    return func;
            }
            return null;
        }
    }
}
