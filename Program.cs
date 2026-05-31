using System;
using CompilerLab;

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = @"C:\TASM\test.pas";
        InputOutput.ReadFile(filePath);

        while (InputOutput.Ch != '\0')
        {
            InputOutput.NextCh();
        }

        Console.WriteLine("\nНажмите Enter для выхода из программы...");
        Console.ReadLine();
    }
}