using TestServerMSI.Appliaction.Interfaces;

namespace TestServerMSI.Appliaction.Services
{
    public class GenerateTextReport : IGenerateTextReport
    {
        public GenerateTextReport() { }
        public string ReportString { get => "zaimplementować"; set => ReportString = value;}
    }
}
