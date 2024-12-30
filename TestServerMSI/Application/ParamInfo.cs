namespace TestServerMSI.Application
{
    public delegate double fitnessFunction(params double[] arg);

    // opis pojedynczego parametru algorytmu , warto ść jest zmienn ą typu double
    public class ParamInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double UpperBoundary { get; set; }
        public double LowerBoundary { get; set; }

        public ParamInfo(string name, string description, double upperBoundary, double lowerBoundary)
        {  
            Name = name; 
            Description = description;
            UpperBoundary = upperBoundary; 
            LowerBoundary = lowerBoundary;
        }
    }
}
