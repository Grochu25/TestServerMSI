
namespace TestServerMSI.Application.Alogrithms
{
    public class Butterfly
    {
        public double[] X { get; set; }

        public Butterfly(double[] X)
        {
            if (X == null)
            {
                throw new InvalidDataException();
            }
            this.X = X;
        }

        public double GetFragrance(fitnessFunction f, double a, double c)
        {
            return c * Math.Pow(f(this.X), a);
        }

        public void Clip(double[,] domain)
        {
            for (int i = 0; i < this.X.Length; i++)
            {
                if (this.X[i] > domain[1, i])
                    this.X[i] = domain[1, i];
                if (this.X[i] < domain[0, i])
                    this.X[i] = domain[0, i];
            }
        }
    }
}