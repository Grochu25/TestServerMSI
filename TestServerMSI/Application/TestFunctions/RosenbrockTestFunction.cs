using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.TestFunctions
{
    public class RosenbrockTestFunction : ITestFunction
    {
        public string Name { get; set; } = "Rosenbrock";

        public double invoke(params double[] arg)
        {
            double sum = 0.0;
            for (int i = 0; i < arg.Length - 1; i++)
                sum += 100 * (arg[i + 1] - Math.Pow(arg[i], 2.0)) + Math.Pow(arg[i] - 1.0, 2.0);
            return sum;
        }
    }
}
