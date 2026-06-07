using System;
using System.Collections.Generic;

namespace CompilerLab
{
    public static class ErrorHandler
    {
        private const int ErrMax = 40;        // максимум ошибок
        private static List<Err> _errList;    // список текущих ошибок для строки
        private static uint _totalErrCount;   // общее количество ошибок
        private static int _lineErrCnt;       // нумерация строк

        static ErrorHandler()
        {
            _errList = new List<Err>();
            _totalErrCount = 0;
            _lineErrCnt = 0;
        }

        public static int ErrorCount
        {
            get
            {
                return _errList.Count;
            }
        }

        public static uint TotalErrCount
        {
            get
            {
                return _totalErrCount;
            }
        }

        public static List<Err> CurrentErrors
        {
            get
            {
                return _errList;
            }
        }

        // Метод для добавления ошибки в список
        public static void AddError(TextPosition position, byte errorCode)
        {
            if (_errList.Count < ErrMax)
            {
                _errList.Add(new Err(position, errorCode));
                _totalErrCount++;
            }
        }

        // Метод для генерации случайных ошибок для текущей строки
        public static void GenerateRandomErrors(TextPosition positionNow, string line)
        {
            Random rnd = new Random();

            if (rnd.Next(0, 100) > 20)
            {
                return;
            }

            var availableCodes = new List<byte>(ErrorTable.Error.Keys);
            int count = rnd.Next(1, 3);
            for (int i = 0; i < count; i++)
            {
                TextPosition p = new TextPosition();
                p.LineNumber = positionNow.LineNumber;
                if (line.Length > 1)
                {
                    p.CharNumber = (byte)rnd.Next(0, line.Length);
                }
                else
                {
                    p.CharNumber = 0;
                }

                int randomKeyIndex = rnd.Next(availableCodes.Count);
                int errorCode = availableCodes[randomKeyIndex];

                AddError(p, (byte)(errorCode));
            }
        }

        // Метод для печати ошибок текущей строки
        public static void PrintErrorsForLine()
        {
            if (_errList.Count == 0)
            {
                return;
            }

            Random random = new Random();

            foreach (var error in _errList)
            {
                _lineErrCnt++; 
                string errorNumber = _lineErrCnt.ToString("D2");
                int charPosition = error.ErrorPosition.CharNumber + 1;
                string pointerLine = new string(' ', charPosition) + "^";

                byte currentKey = (byte)(error.ErrorCode);
                string errorText = ErrorTable.Error[currentKey];

                Console.WriteLine
                    ($"**{errorNumber}**{pointerLine} ошибка код {error.ErrorCode}: {errorText}");
            }
            _errList.Clear();
        }
    }
}
