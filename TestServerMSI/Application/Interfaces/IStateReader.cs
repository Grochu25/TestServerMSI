namespace TestServerMSI.Application.Interfaces
{
    public interface IStateReader
    {
        // Metoda wczytująca z pliku stan algorytmu (w odpowiednim formacie ).
        // Stan algorytmu : numer iteracji , liczba wywołań funkcji celu ,
        // populacja wraz z wartością funkcji dopasowania
        void LoadFromFileStateOfAlgorithm(string path);
    }
}
