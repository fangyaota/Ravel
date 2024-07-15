using Ravel.Values;

internal class Program
{
    private static void Main(string[] args)
    {
        RavelGlobal global = new();
        Interactor interactor = new(global);

        interactor.Run();
    }
}