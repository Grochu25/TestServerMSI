namespace TestServerMSI.Appliaction.Interfaces
{
    public interface IGenerateTextReport
    {
        // Tworzy raport w postaci łańcucha znaków
        // w raporcie powinny znaleźć się informacje o:
        // najlepszym osobniku wraz z warto ścią funkcji celu ,
        // liczbie wywo łań funkcji celu ,
        // parametrach algorytmu
        string ReportString { get; set; }
    }
}
