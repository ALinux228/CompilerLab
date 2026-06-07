using CompilerLab;
using System;
using System.IO;

namespace CompilerLab
{
    class Program
    {
        static void Main(string[] args)
        {
            InputOutput.ReadFile(@"C:\TASM\test.pas");

            LexicalAnalyzer analyzer = new LexicalAnalyzer();
            using (StreamWriter writer = new StreamWriter(@"C:\TASM\codes.txt"))
            while (!InputOutput.IsEndOfFile)
            {
                byte sym = analyzer.NextSym();
                writer.Write(sym + " ");
            }
            Console.WriteLine("Процесс завершен успешно: коды символов записаны в файл.");
        }
    }
}
