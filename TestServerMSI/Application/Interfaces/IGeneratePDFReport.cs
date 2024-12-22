﻿namespace TestServerMSI.Appliaction.Interfaces
{
    public interface IGeneratePDFReport
    {
        // Tworzy raport w określonym stylu w formacie PDF
        // w raporcie powinny znaleźć się informacje o:
        // najlepszym osobniku wraz z wartością funkcji celu ,
        // liczbie wywołań funkcji celu ,
        // parametrach algorytmu
        void GenerateReport(string path);

    }
}
