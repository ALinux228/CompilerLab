using System;
using System.Collections.Generic;
using System.IO;

namespace CompilerLab
{
    class InputOutput
    {
        private static string _line;                // текущая строка
        private static char _ch;                    // текущий символ
        private static byte _lastInLine;            // индекс последнего символа в строке   
        private static TextPosition _positionNow;   // позиция (строка, символ)
        private static StreamReader _file;          // поток для чтения файла
        private static bool _isEndOfFile;           // флаг конца файла

        static InputOutput()
        {
            _line = string.Empty;
            _lastInLine = 0;
            _file = null;
            _ch = '\0';
            _positionNow = new TextPosition();
            _isEndOfFile = false;
        }

        public static char Ch
        {
            get
            {
                return _ch;
            }
            set
            {
                _ch = value;
            }
        }

        public static bool IsEndOfFile
        {
            get
            {
                return _isEndOfFile;
            }
        }

        public static TextPosition PositionNow
        {
            get
            {
                return _positionNow;
            }
            set
            {
                _positionNow = value;
            }
        }

        // Метод для чтения файла и инициализации процесса компиляции
        public static void ReadFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }

            _file = new StreamReader(fileName);
            _isEndOfFile = false;

            if (!_file.EndOfStream)
            {
                _line = _file.ReadLine();
                if (_line == null)
                {
                    _line = string.Empty;
                }

                _positionNow = new TextPosition(1, 0);
                _lastInLine = (byte)(_line.Length - 1);

                if (_line.Length > 0)
                {
                    Ch = _line[0];
                }
                else
                {
                    Ch = ' ';
                }

            }
            else
            {
                Console.WriteLine("Пустой файл");
                End();
            }
        }

        // Метод для получения следующего символа 
        public static void NextCh()
        {
            if (_isEndOfFile)
            {
                Ch = '\0';
                return;
            }

            if (_positionNow.CharNumber == _lastInLine)
            {
                Console.WriteLine("      " + _line);
                // ErrorHandler.GenerateRandomErrors(_positionNow, _line);
                ErrorHandler.PrintErrorsForLine();

                ReadNextLine();

                var nextPosition = _positionNow;
                nextPosition.LineNumber += 1;
                nextPosition.CharNumber = 0;
                _positionNow = nextPosition;
            }
            else
            {
                var nextPosition = _positionNow;
                nextPosition.CharNumber++;
                _positionNow = nextPosition;
            }

            if (_isEndOfFile)
            {
                Ch = '\0';
            }
            else if (_line.Length == 0)
            {
                Ch = ' ';
            }
            else
            {
                Ch = _line[_positionNow.CharNumber];
            }
        }

        // Метод для чтения следующей строки из файла
        private static void ReadNextLine()
        {
            if (!_file.EndOfStream)
            {
                _line = _file.ReadLine();
                _lastInLine = (byte)(_line.Length - 1);
            }
            else
            {
                _isEndOfFile = true;
                End();
            }
        }

        // Метод для завершения компиляции и освобождения ресурсов
        private static void End()
        {
            Console.WriteLine($"\nКомпиляция завершена: ошибок — {ErrorHandler.TotalErrCount}!");

            if (_file != null)
            {
                _file.Dispose();
                _file = null;
            }
        }

        public static bool IsEndOfLine()
        {
            if (_isEndOfFile)
                return true;

            return _positionNow.CharNumber >= _lastInLine;
        }
    }
}
