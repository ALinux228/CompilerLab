using CompilerLab;
using System;
using System.Collections.Generic;
namespace CompilerLab
{
    class LexicalAnalyzer
    {

        public const byte star = 21;   // *
        public const byte slash = 60;  // /
        public const byte equal = 16;  // =
        public const byte comma = 20;  // ,
        public const byte semicolon = 14; // ;
        public const byte colon = 5;   // :
        public const byte point = 61;  // .
        public const byte arrow = 62;	 // ^
        public const byte leftpar = 9; // (
        public const byte rightpar = 4;	// )
        public const byte lbracket = 11;	// [
        public const byte rbracket = 12;	// ]
        public const byte flpar = 63;	// {
        public const byte frpar = 64;	// }
        public const byte later = 65;	// <
        public const byte greater = 66;	// >
        public const byte laterequal = 67;	//  <=
        public const byte greaterequal = 68;	//  >=
        public const byte latergreater = 69;	//  <>
        public const byte plus = 70;	// +
        public const byte minus = 71;	// –
        public const byte lcomment = 72;	//  (*
        public const byte rcomment = 73;	//  *)
        public const byte assign = 51;	//  :=
        public const byte twopoints = 74;	//  ..
        public const byte ident = 2;	// идентификатор
        public const byte floatc = 82;// вещественная константа
        public const byte intc = 15;	// целая константа

        public const byte casesy = 31;
        public const byte elsesy = 32;
        public const byte filesy = 57;
        public const byte gotosy = 33;
        public const byte thensy = 52;
        public const byte typesy = 34;
        public const byte untilsy = 53;
        public const byte dosy = 54;
        public const byte withsy = 37;
        public const byte ifsy = 56;
        public const byte insy = 100;
        public const byte ofsy = 101;
        public const byte orsy = 102;
        public const byte tosy = 103;
        public const byte endsy = 104;
        public const byte varsy = 105;
        public const byte divsy = 106;
        public const byte andsy = 107;
        public const byte notsy = 108;
        public const byte forsy = 109;
        public const byte modsy = 110;
        public const byte nilsy = 111;
        public const byte setsy = 112;
        public const byte beginsy = 113;
        public const byte whilesy = 114;
        public const byte arraysy = 115;
        public const byte constsy = 116;
        public const byte labelsy = 117;
        public const byte downtosy = 118;
        public const byte packedsy = 119;
        public const byte recordsy = 120;
        public const byte repeatsy = 121;
        public const byte programsy = 122;
        public const byte functionsy = 123;
        public const byte procedurensy = 124;

        private byte _symbol; // код символа
        private TextPosition _token; // позиция символа
        private string _name; // название идентификатора
        private int _nmbInt; // значение целой константы
        private float _nmbFloat; // значение вещественной константы
        private Keywords _keywords; // таблица ключевых слов

        public LexicalAnalyzer()
        {
            _symbol = 0;
            _name = "";
            _nmbInt = 0;
            _nmbFloat = 0;
            _keywords = new Keywords();
        }

        public byte Symbol
        {
            get
            {
                return _symbol;
            }
            set
            {
                _symbol = value;
            }
        }

        public TextPosition Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Number
        {
            get
            {
                return _nmbInt;
            }
            set
            {
                _nmbInt = value;
            }
        }

        public float Float
        {
            get
            {
                return _nmbFloat;
            }
            set
            {
                _nmbFloat = value;
            }
        }

        public byte NextSym()
        {
            while (char.IsWhiteSpace(InputOutput.Ch))
            {
                InputOutput.NextCh();
            }
            _token = InputOutput.PositionNow;


            if (char.IsDigit(InputOutput.Ch))
            {
                byte digit = 0;   
                Int16 maxint = Int16.MaxValue;
                _nmbInt = 0;
                string number = ""; 

                // Целое число
                while (char.IsDigit(InputOutput.Ch))
                {
                    digit = (byte)(InputOutput.Ch - '0');
                    if (_nmbInt < maxint / 10 || (_nmbInt == maxint / 10 && digit <= maxint % 10))
                    {
                        _nmbInt = 10 * _nmbInt + digit;
                        number += InputOutput.Ch;
                    }
                    else
                    {
                        ErrorHandler.AddError(InputOutput.PositionNow, 203);
                        _nmbInt = 0;

                        while (char.IsDigit(InputOutput.Ch))
                        {
                            InputOutput.NextCh();
                        }
                        _symbol = intc;
                        return intc;
                    }
                    InputOutput.NextCh();
                }

                // Вещественное число
                if (InputOutput.Ch == '.')
                {
                    InputOutput.NextCh();
                    if (char.IsDigit(InputOutput.Ch))
                    {
                        number += ".";
                        while (char.IsDigit(InputOutput.Ch))
                        {
                            number += InputOutput.Ch;
                            InputOutput.NextCh();
                        }

                        if (double.TryParse(number, out double val))
                        {
                            _nmbFloat = (float)val;
                        }
                        else
                        {
                            ErrorHandler.AddError(InputOutput.PositionNow, 205);
                            _nmbFloat = 0;
                        }
                        _symbol = floatc;
                        return floatc;
                    }
                    else
                    {
                        ErrorHandler.AddError(InputOutput.PositionNow, 206);
                        _symbol = floatc;
                        return floatc;
                    }
                }
                _symbol = intc;
                return intc;
            }

            // Идентификаторы
            else if (char.IsLetter(InputOutput.Ch))
            {
                _name = "";

                while (char.IsLetter(InputOutput.Ch))
                {
                    _name += Char.ToLower(InputOutput.Ch);
                    InputOutput.NextCh();
                }

                byte len = (byte)_name.Length;
                if (_keywords.Kw.ContainsKey(len) && _keywords.Kw[len].ContainsKey(_name))
                {
                    _symbol = _keywords.Kw[len][_name];
                }
                else
                {
                    _symbol = ident;
                }
            }

            else if (InputOutput.Ch == '<')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    _symbol = laterequal;
                    InputOutput.NextCh();
                }
                else if (InputOutput.Ch == '>')
                {
                    _symbol = latergreater;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = later;
                }
            }

            else if (InputOutput.Ch == '>')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    _symbol = greaterequal;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = greater;
                }
            }

            else if (InputOutput.Ch == ':')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    _symbol = assign;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = colon;
                }
            }


            else if (InputOutput.Ch == '=')
            {
                _symbol = equal;
                InputOutput.NextCh();
            }

            else if (InputOutput.Ch == ';')
            {
                _symbol = semicolon;
                InputOutput.NextCh();
            }

            else if (InputOutput.Ch == '.')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '.')
                {
                    _symbol = twopoints;
                    InputOutput.NextCh();
                }
                else
                {
                    _symbol = point;
                }
            }

            else if (InputOutput.Ch == '+')
            {
                _symbol = plus;
                InputOutput.NextCh();
            }

            else if (InputOutput.Ch == '-')
            {
                _symbol = minus;
                InputOutput.NextCh();
            }

            else if (InputOutput.Ch == '*')
            {
                _symbol = star;
                InputOutput.NextCh();
            }

            else if (InputOutput.Ch == '/')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '/')
                {
                    while (!InputOutput.IsEndOfLine())
                    {
                        InputOutput.NextCh();
                    }
                    InputOutput.NextCh();
                    return NextSym();
                }
                else
                {
                    _symbol = slash;
                }
            }

            else if (InputOutput.Ch == '(')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '*')
                {
                    InputOutput.NextCh();
                    while (InputOutput.Ch != '\0')
                    {
                        if (InputOutput.Ch == '*')
                        {
                            InputOutput.NextCh();
                            if (InputOutput.Ch == ')')
                            {
                                InputOutput.NextCh();
                                return NextSym();
                            }
                        }
                        else
                        {
                            InputOutput.NextCh();
                        }
                    }
                    ErrorHandler.AddError(InputOutput.PositionNow, 202);
                    return 0;
                }
                else
                {
                    _symbol = leftpar;
                }
            }

            else if (InputOutput.Ch == ')')
            {
                _symbol = rightpar;
                InputOutput.NextCh();
            }

            else if (InputOutput.Ch == '{')
            {
                InputOutput.NextCh();
                while (InputOutput.Ch != '\0')
                {
                    if (InputOutput.Ch == '}')
                    {
                        InputOutput.NextCh();
                        return NextSym();
                    }
                    InputOutput.NextCh();
                }
                ErrorHandler.AddError(InputOutput.PositionNow, 202);
                return 0;
            }

            else if (InputOutput.Ch == ',')
            {
                _symbol = comma;
                InputOutput.NextCh();
            }

            else if (InputOutput.Ch == '\'')
            {
                string str = "";
                InputOutput.NextCh();
                while (InputOutput.Ch != '\'' && !InputOutput.IsEndOfLine() && InputOutput.Ch != '\0')
                {
                    str += InputOutput.Ch;
                    InputOutput.NextCh();
                }
                if (InputOutput.IsEndOfLine() || InputOutput.Ch == '\0')
                {
                    ErrorHandler.AddError(InputOutput.PositionNow, 204);
                    return NextSym();
                }
                InputOutput.NextCh();
                _name = str;
                _symbol = ident;
            }

            else if (InputOutput.Ch == '\0')
            {
                return 0;
            }

            else
            {
                ErrorHandler.AddError(InputOutput.PositionNow, 200);
                InputOutput.NextCh();
            }
            return _symbol;
        }
    }
}