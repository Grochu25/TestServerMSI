namespace TestServerMSI.Appliaction.Interfaces
{
    public interface ITestFunction
    {
        string Name { get; }
        double invoke(params double[] arg);
    }
}
