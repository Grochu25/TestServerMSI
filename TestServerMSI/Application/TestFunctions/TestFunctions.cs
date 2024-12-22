using System.Diagnostics;
using System.Reflection;
using TestServerMSI.Appliaction.Interfaces;

namespace TestServerMSI.Application.TestFunctions
{
    public class TestFunctions
    {
        public static List<ITestFunction> getTestFunctionList()
        {
            List<Type> types = new List<Type>(from t in Assembly.GetExecutingAssembly().GetTypes()
                                          where t.IsClass && t.Namespace == "TestServerMSI.Appliaction.TestFunctions"
                                          && t.GetInterface("ITestFunction", true) != null
                                          select t);
            List<ITestFunction> testFunctions = new List<ITestFunction>();
            foreach (var t in types)
            {
                var instance = Activator.CreateInstance(t);
                if (instance is ITestFunction)
                    testFunctions.Add((ITestFunction)instance);
            }
            return testFunctions;
        }

        public static ITestFunction? getTestFunction(string testFunctionName)
        {
            var functions = getTestFunctionList();
            foreach (var func in functions)
            {
                if (func.Name == testFunctionName)
                    return func;
            }
            return null;
        }
    }
}
