﻿namespace TestServerMSI.Application.Interfaces
{
    public interface IStateWriter
    {
        // Metoda zapisująca do pliku tekstowego stan algorytmu (w odpowiednim formacie).
        // Stan algorytmu : numer iteracji , liczba wywołań funkcji celu ,
        // populacja wraz z wartością funkcji dopasowania
        void SaveToFileStateOfAlghoritm(string path);
    }
}
