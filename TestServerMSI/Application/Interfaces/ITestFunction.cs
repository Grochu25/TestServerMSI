namespace TestServerMSI.Application.Interfaces
{
    public interface ITestFunction
    {
        string Name { get; set; }
        double invoke(params double[] arg);
    }
}
