using System.Diagnostics;
using TestServerMSI.Application.Interfaces;
using TestServerMSI.Application.Services;

namespace TestServerMSI.Application.Alogrithms
{
    public class ButterflyOptimizationAlgorithm : IOptimizationAlgorithm
    {
        public ButterflyOptimizationAlgorithm()
        {
            this.Name = "Butterfy Optimization Algorithm";
            this.ParamsInfo = [
                new ParamInfo("population", "wielość populacji obiektów", 1000, 0),
                new ParamInfo("iterations", "ilość iteracji", 10000, 0),
                new ParamInfo("A", "parametr 'a' jest specyficzny dla algorytmu.", 1, 0),
                new ParamInfo("C", "parametr 'c' jest specyficzny dla algorytmu.", 1, 0),
                new ParamInfo("P", "parametr 'p' jest specyficzny dla algorytmu.", 1, 0),
            ];
            this.XBest = new double[1];
            this.FBest = 10000;
            this.NumberOfEvaluationFitnessFunction = 0;
            this.CurrentIteration = 0;
            this.Population = [[0]];
            this.PopulationValues = [0];
            this.ParametersUsedValues = new Dictionary<string, double>();

            this.stringReportGenerator = new GenerateTextReport();
            this.pdfReportGenerator = new GeneratePDFReport();
            this.writer = new StateWriter();
            this.reader = new StateReader();
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

        // Dodane przez nas
        public int CurrentIteration { get; set; }
        public double[][] Population { get; set; }
        public double[] PopulationValues { get; set; }
        public bool Stop { get; set; } = false;
        public Dictionary<string, double> ParametersUsedValues { get; set; }
        // Koniec

        private int dimensions = 1;
        private double population = 0;
        private double iterations = 0.0;
        private double a = 0.5;
        private double c = 0.5;
        private double p = 0.5;

        private fitnessFunction fitness;
        private double[,] domain;
        private Random rand = new Random();
        private Butterfly[] butterflies;
        private Butterfly bestButterfly;
        private uint bestIdx;

        public void Solve(fitnessFunction f, double[,] domain, params double[] parameters)
        {
            this.fitness = f;
            this.domain = domain;
            this.dimensions = domain.GetLength(1);
            this.population = parameters[0];
            this.ParametersUsedValues["population"] = this.population;
            this.iterations = parameters[1];
            this.ParametersUsedValues["iterations"] = this.iterations;
            if (parameters.Length >= 3) this.a = parameters[2];
            if (parameters.Length >= 4) this.c = parameters[3];
            if (parameters.Length >= 5) this.p = parameters[4];
            this.ParametersUsedValues["A"] = this.a;
            this.ParametersUsedValues["C"] = this.c;
            this.ParametersUsedValues["P"] = this.p;
            this.XBest = new double[this.dimensions];
            this.FBest = 1;
            this.NumberOfEvaluationFitnessFunction = 0;
            this.CurrentIteration = 0;
            Stop = false;
            Population = new double[(int)population][];
            for (int i = CurrentIteration; i < (int)population; i++)
            {
                Population[i] = new double[dimensions];
            }
            PopulationValues = new double[(int)population];
            if (File.Exists("savedAlgorithms/BOA.alg"))
            {
                reader.LoadFromFileStateOfAlgorithm("savedAlgorithms/BOA.alg");
                this.butterflies = new Butterfly[(int)this.population];
                for (uint i = 0; i < this.population; ++i)
                {
                    this.butterflies[i] = new Butterfly(Population[i]);
                    this.butterflies[i].FitnessValue = PopulationValues[i];
                }
                this.butterflies.ToList();
            }
            else
            {
                this.InitButterflies();
            }
            this.runAlgorithm();
        }

        public void runAlgorithm()
        {
            for (int i = CurrentIteration; i < this.iterations; ++i)
            {
                this.Eval();
                this.MoveButterflies();
                this.ClipButterflies();
                for (int k = 0; k < population; k++)
                {
                    Population[k] = butterflies[k].X;
                    PopulationValues[k] = butterflies[k].FitnessValue;
                }
                CurrentIteration++;
                writer.SaveToFileStateOfAlgorithm("savedAlgorithms/BOA.alg");
                if (Stop) return;
                Debug.WriteLine(i); //Dla debugu
            }
            this.Eval();
            this.XBest = this.bestButterfly.X;
            this.FBest = this.bestButterfly.FitnessValue;
            Debug.WriteLine("BOA finished: " + XBest.ToString() + " : " + FBest);
            if (File.Exists("savedAlgorithms/BOA.alg"))
                File.Delete("savedAlgorithms/BOA.alg");
        }

        public void MoveButterflies()
        {
            for (int i = 0; i < this.population; ++i)
            {
                double r = this.rand.NextDouble();
                double r2 = r * r;
                double currentFragrance = this.butterflies[i].GetFragrance(this.fitness, this.a, this.c);
                if (r < this.p && i != this.bestIdx)                                 // Move to best excluding best bf
                {
                    for (int j = 0; j < this.dimensions; ++j)
                    {
                        double moveFactor = r2 * this.bestButterfly.X[j] - this.butterflies[i].X[j];
                        this.butterflies[i].X[j] += moveFactor * currentFragrance;
                    }
                }
                else if (i != this.bestIdx)                                          // Move locally (excluded movig best cuz works better xpp)
                {
                    int randJ = this.rand.Next(0, (int)this.population);
                    int randK = this.rand.Next(0, (int)this.population);
                    for (int j = 0; j < this.dimensions; ++j)
                    {
                        double moveFactor = r2 * this.butterflies[randJ].X[j] - this.butterflies[randK].X[j];
                        this.butterflies[i].X[j] += moveFactor * currentFragrance;
                    }
                }
            }
        }

        private void InitButterflies()
        {
            this.butterflies = new Butterfly[(int)this.population];
            for (uint i = 0; i < this.population; ++i)
            {
                this.butterflies[i] = new Butterfly(this.RandomPointInDiemnsions());
                Debug.WriteLine(butterflies[i].X[0]);
            }
            this.butterflies.ToList();
        }

        private double ButterflyF(double x)
        {
            return this.c * Math.Pow(x, this.a);
        }

        private void ClipButterflies()
        {
            for (int i = 0; i < this.population; ++i)
            {
                this.butterflies[i].Clip(this.domain);
            }
        }

        private double[] RandomPointInDiemnsions()
        {
            double[] coords = new double[dimensions];
            for (uint i = 0; i < this.dimensions; i++)
            {
                double range = this.domain[1, i] - this.domain[0, i];
                coords[i] = this.domain[0, i] + rand.NextDouble() * range;
            }
            return coords;
        }

        private void Eval()
        {
            for (int i = 0; i < butterflies.Length; i++)
            {
                butterflies[i].FitnessValue = fitness(butterflies[i].X);
                NumberOfEvaluationFitnessFunction++;
            }
            uint minIdx = 0;
            for (uint i = 1; i < this.population; i++)
            {
                if (butterflies[i].FitnessValue < butterflies[minIdx].FitnessValue)
                    minIdx = i;
            }
            this.bestButterfly = this.butterflies[minIdx];
            this.bestIdx = minIdx;
        }
    }
}
