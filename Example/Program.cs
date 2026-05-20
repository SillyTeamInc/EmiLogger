using EmiLogger;
using Spectre.Console;

namespace Example;

class Program
{
    static void Main(string[] args)
    {
        Emi.ForceSetCapabilities();
        Emi.Info("Hello, World!");
    }
}