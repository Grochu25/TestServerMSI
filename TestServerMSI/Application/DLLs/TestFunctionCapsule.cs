using System.Reflection;
using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.DLLs
{
    public class TestFunctionCapsule : ITestFunction
    {
        private object _testFunctionFromDLL;
        private Type _testFunctionType;

        public TestFunctionCapsule(object testFunctionFromDLL)
        {
            _testFunctionFromDLL = testFunctionFromDLL;
            _testFunctionType = _testFunctionFromDLL.GetType();
        }

        public string Name
        {
            get
            {
                return (string)_testFunctionType.GetProperty("Name").GetValue(_testFunctionFromDLL);
            }
            set
            {
                _testFunctionType.GetProperty("Name").SetValue(_testFunctionFromDLL, value);
            }
        }

        public double invoke(params double[] arg)
        {
            MethodInfo invokeMethod = _testFunctionType.GetMethod("invoke");
            return (double)invokeMethod.Invoke(_testFunctionFromDLL, [arg]);
        }
    }
}
