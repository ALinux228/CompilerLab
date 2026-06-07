using System.Collections.Generic;

namespace CompilerLab
{
    static class ErrorTable
    {
        private static Dictionary<byte, string> _error;

        public static Dictionary<byte, string> Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }

        static ErrorTable()
        {
            _error = new Dictionary<byte, string>()
            {
            { 200, "неизвестный символ" },
            { 202, "незакрытый комментарий" },
            { 203, "целая константа превышает предел" },
            { 204, "не закрытая кавычка" },
            { 205, "ошибка вещественной константы" },
            };
        }
    }
}
