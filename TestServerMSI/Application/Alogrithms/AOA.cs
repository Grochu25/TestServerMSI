using System.Diagnostics;
using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.Services;

namespace TestServerMSI.Application.Alogrithms
{
    public class AOA : IOptimizationAlgorithm
    {
        public AOA() {
            Name = "Archimedes Optimization Algorithm";
            ParamsInfo = [
                new ParamInfo("population", "wielość populacji obiektów",10000,0),
                new ParamInfo("iterations", "ilość iteracji",10000,0),
                new ParamInfo("C1", "parametr specyficzny dla algorytmu. Zalecane wartości {1, 2}",2,1),
                new ParamInfo("C2", "parametr specyficzny dla algorytmu. Zalecane wartości {2, 4, 6}",6,2),
                new ParamInfo("C3", "parametr specyficzny dla algorytmu. Zalecane wartości {1, 2}",2,1),
                new ParamInfo("C4", "parametr specyficzny dla algorytmu. Zalecane wartości {0.5, 1}",1,0.5)
            ];
            XBest = new double[1];
            FBest = 1;
            NumberOfEvaluationFitnessFunction = 0;

            stringReportGenerator = new GenerateTextReport();
            pdfReportGenerator = new GeneratePDFReport();
            writer = new StateWriter();
            reader = new StateReader();
        }

        public string Name { get; set; }
        public ParamInfo[] ParamsInfo { get; set; }
        public IStateWriter writer { get; set; }
        public IStateReader reader { get; set; }
        public IGenerateTextReport stringReportGenerator { get; set; }
        public IGeneratePDFReport pdfReportGenerator { get; set; }
        public double[] XBest { get; set; }
        public double FBest { get; set; }
        public int NumberOfEvaluationFitnessFunction { get; set; }

        private int dimensions = 1;
        private double population = 0;
        private double iterations = 0.0;
        private double C1 = 2.0;
        private double C2 = 4.0;
        private double C3 = 1.0;
        private double C4 = 0.5;

        private fitnessFunction f;
        private double[,] domain;
        private Random rand = new Random();
        private Floating[] objects;
        private Floating bestObject;
        private Floating.MinComparer minComparer = new Floating.MinComparer();
        private Floating.MaxComparer maxComparer = new Floating.MaxComparer();

        public void Solve(fitnessFunction f, double[,] domain, params double[] parameters)
        {
            this.f = f;
            this.domain = domain;
            dimensions = domain.GetLength(1);
            population = parameters[0];
            iterations = parameters[1];
            C1 = parameters[2];
            C2 = parameters[3];
            C3 = parameters[4];
            C4 = parameters[5];

            XBest = new double[dimensions];
            FBest = 1;
            NumberOfEvaluationFitnessFunction = 0;
            runAlgorithm();
        }

        private double[] randomPointInDiemnsions()
        {
            double[] coords = new double[dimensions];
            for (int i = 0; i < dimensions; i++)
                coords[i] = domain[0,i] + rand.NextDouble() * (domain[1, i] - domain[0, i]);
            return coords;
        }

        private double[] randomVector()
        {
            double[] vector = new double[dimensions];
            for (int i = 0; i < dimensions; i++)
                vector[i] = rand.NextDouble();
            return vector;
        }

        private Floating[] initialize()
        {
            objects = new Floating[(int)population];
            for (int i = 0; i < population; i++)
            {
                objects[i] = new Floating(
                    randomPointInDiemnsions(),
                    randomVector(),
                    randomVector(),
                    randomPointInDiemnsions()
                );
            }
            return objects;
        }

        private void evaluate()
        {
            bestObject = objects[0];
            for (int i = 1; i < objects.Length; i++)
            {
                if (f(objects[i].X) < f(bestObject.X))
                    bestObject = objects[i];
            }
        }

        private void actualizeParams()
        {
            double random = rand.NextDouble();
            for (int i = 0; i < objects.Length; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    objects[i].Den[j] = objects[i].Den[j] + random * (bestObject.Den[j] - objects[i].Den[j]);
                    objects[i].Vol[j] = objects[i].Vol[j] + random * (bestObject.Vol[j] - objects[i].Vol[j]);
                }
            }
        }

        public void runAlgorithm()
        {
            initialize();
            evaluate();
            double TF = 0.0;
            double d = 0.0;
            for (double t = NumberOfEvaluationFitnessFunction + 1.0; t <= iterations; t += 1.0)
            {
                TF = Math.Exp((t - iterations) / iterations);
                TF = (TF > 1) ? 1 : TF;
                d = Math.Exp((iterations - t) / iterations) - (t / iterations);
                actualizeParams();
                if (TF <= 0.5)
                {
                    countAccForExploration();
                    actualizePositionsForExploration(d);
                }
                else
                {
                    countAccForExploitation();
                    actualizePositionsForExploitation(d, TF);
                }

                evaluate();
                XBest = bestObject.X;
                FBest = f(XBest);
                NumberOfEvaluationFitnessFunction++;
                writer.SaveToFileStateOfAlghoritm("savedAlgorithms/AOA" + population + iterations + C1 + C2 + C3 + C4 + ".alg");
                Debug.WriteLine("test run: "+NumberOfEvaluationFitnessFunction);
            }
            //Tutaj plik z zapisanym algorytmem jest usuwany, bo już nie będzie potrzebny
            if(File.Exists("savedAlgorithms/AOA" + population + iterations + C1 + C2 + C3 + C4 + ".alg"))
                File.Delete("savedAlgorithms/AOA" + population + iterations + C1 + C2 + C3 + C4 + ".alg");
        }

        private void countAccForExploration()
        {
            for (int i = 0; i < population; i++)
            {
                var mr = rand.NextInt64((int)population);
                objects[i].evaluateNewAccelerationFrom(objects[mr]);
            }
        }

        private void actualizePositionsForExploration(double d)
        {


            double min = objects.Min(minComparer).Acc.Min();
            double max = objects.Max(maxComparer).Acc.Max();
            double[] oldX = new double[dimensions];

            Int64 mr = 0;

            for (int i = 0; i < population; i++)
            {
                mr = rand.NextInt64((int)population);

                objects[i].normalizeAccFor(min, max);

                for (int j = 0; j < dimensions; j++)
                {
                    oldX[j] = objects[i].X[j];
                    objects[i].X[j] = objects[i].X[j] + C1 * rand.NextDouble() * objects[i].Acc[j] * d * (objects[mr].X[j] - objects[i].X[j]);
                }

                if (f(objects[i].X) > f(oldX))
                    oldX.CopyTo(objects[i].X, 0);
                objects[i].checkBoundry(domain);
            }
        }

        private void countAccForExploitation()
        {
            for (int i = 0; i < population; i++)
                objects[i].evaluateNewAccelerationFrom(bestObject);
        }

        private void actualizePositionsForExploitation(double d, double TF)
        {

            double min = objects.Min(minComparer).Acc.Min();
            double max = objects.Max(maxComparer).Acc.Max();

            double F = 0.0;
            double T = (C3 * TF > 1) ? 1 : C3 * TF;

            double[] oldX = new double[dimensions];

            for (int i = 0; i < population; i++)
            {
                objects[i].normalizeAccFor(min, max);

                for (int j = 0; j < dimensions; j++)
                {
                    F = (2.0 * rand.NextDouble() - C4 <= 0.5) ? 1.0 : -1.0;
                    oldX[j] = objects[i].X[j];
                    objects[i].X[j] = bestObject.X[j] + F * C2 * rand.NextDouble() * objects[i].Acc[j] * d * (T * bestObject.X[j] - objects[i].X[j]);
                }

                if (f(objects[i].X) > f(oldX))
                    oldX.CopyTo(objects[i].X, 0);
                objects[i].checkBoundry(domain);
            }
        }

        private class Floating
        {
            public double[] X { get; set; }
            public double[] Den { get; set; }
            public double[] Vol { get; set; }
            public double[] Acc { get; set; } //Zrobić z tego jednowymiarową?

            public Floating(double[] X, double[] Den, double[] Vol, double[] Acc)
            {
                this.X = X;
                this.Den = Den;
                this.Vol = Vol;
                this.Acc = Acc;
            }

            public void evaluateNewAccelerationFrom(Floating other)
            {
                for (int i = 0; i < Acc.Length; i++)
                    Acc[i] = (other.Den[i] + (other.Vol[i] * other.Acc[i])) / (Den[i] * Vol[i]);
            }

            public void normalizeAccFor(double min, double max)
            {
                for (int i = 0; i < Acc.Length; i++)
                    Acc[i] = 0.9 * ((Acc[i] - min) / (max - min)) + 0.1;
            }

            public void checkBoundry(double[,] domain)
            {
                for (int i = 0; i < X.Length; i++)
                {
                    if (X[i] > domain[1, i])
                        X[i] = domain[1, i];
                    if (X[i] < domain[0, i])
                        X[i] = domain[0, i];
                }
            }

            public class MinComparer : IComparer<Floating>
            {
                public int Compare(Floating? x, Floating? y)
                {
                    if (x.Acc.Min() < y.Acc.Min())
                        return -1;
                    else if (x.Acc.Min() > y.Acc.Min())
                        return 1;
                    else
                        return 0;
                }
            }

            public class MaxComparer : IComparer<Floating>
            {
                public int Compare(Floating? x, Floating? y)
                {
                    if (x.Acc.Max() < y.Acc.Max())
                        return -1;
                    else if (x.Acc.Max() > y.Acc.Max())
                        return 1;
                    else
                        return 0;
                }
            }
        }
    }
}
