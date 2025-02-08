
using TestServerMSI.Application.Services;

namespace TestServerMSI.Application.Interfaces
{
    public interface IGeneratePDFReport
    {
        public uint Precision { get; set; }
        public string ReportString { get; set; }
        public IOptimizationAlgorithm Alg { get; set; }
        public ITestFunction TF { get; set; }

        public string FloatFormat { get; set; }

        // Tworzy raport w określonym stylu w formacie PDF
        // w raporcie powinny znaleźć się informacje o:
        // najlepszym osobniku wraz z wartością funkcji celu ,
        // liczbie wywołań funkcji celu ,
        // parametrach algorytmu
        public abstract void GenerateReport(string path, ComparisonReportGenerator comparisonReportGenerator);
    }
}


