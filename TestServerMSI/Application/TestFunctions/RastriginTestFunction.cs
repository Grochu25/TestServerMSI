using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.TestFunctions
{
    public class RastriginTestFunction : ITestFunction
    {
        public string Name { get; set; } = "Rastrigin";

        public double invoke(params double[] arg)
        {
            double sum = 10.0 * arg.Length;
            for (int i = 0; i < arg.Length; i++)
                sum += (Math.Pow(arg[i], 2.0) - 10*Math.Cos(2*arg[i]*Math.PI));
            return sum;
        }
    }
}
