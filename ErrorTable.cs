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
                { 101, "ожидался символ ';'" },
                { 102, "несовместимые типы данных в операции" },
                { 103, "повторное объявление переменной" },
                { 104, "деление на 0" },
                { 105, "неверный идентификатор" }
            };
        }
    }
}