using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.Services
{
    public class GenerateTextReport : IGenerateTextReport
    {
        public GenerateTextReport() { }
        public string ReportString { get => "zaimplementować"; set => ReportString = value;}
    }
}
