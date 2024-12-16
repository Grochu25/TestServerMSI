using TestServerMSI.Appliaction.Interfaces;

namespace TestServerMSI.Appliaction.TestFunctions
{
    public class SphereTestFunction : ITestFunction
    {
        public string Name => "Sphere";

        public double invoke(params double[] arg)
        {
            double sum = 0.0;
            for (int i = 0; i < arg.Length; i++)
                sum += Math.Pow((arg[i] - Math.Sqrt(2)), 2.0);
            return sum;
        }
    }
}
