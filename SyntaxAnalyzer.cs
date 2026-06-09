using CompilerLab;
using System;
using System.Collections.Generic;

namespace CompilerLab
{
    class SyntaxAnalyzer
    {
        private LexicalAnalyzer _lex;
        private byte _symbol;

        public SyntaxAnalyzer(LexicalAnalyzer lex)
        {
            _lex = lex;
            NextSym();
        }

        private void NextSym()
        {
            _symbol = _lex.NextSym();
        }

        private void accept(byte symbolExpected, byte err)
        {

            if (_symbol == symbolExpected)
            {
                NextSym();
            }
            else
            {
                ErrorHandler.AddError(InputOutput.PositionNow, err);
            }
        }

        private bool Belong(byte element, HashSet<byte> set)
        {
            return set != null && set.Contains(element);
        }

        private void SkipTo(HashSet<byte> where)
        {
            while (_symbol != 0 && !Belong(_symbol, where))
            {
                NextSym();
            }
        }

        private void SkipTo2(HashSet<byte> start, HashSet<byte> follow)
        {
            while (_symbol != 0 && !Belong(_symbol, start) && !Belong(_symbol, follow))
            {
                NextSym();
            }
        }

        // Программа
        public void Program()
        {
            accept(LexicalAnalyzer.programsy, 214);
            accept(LexicalAnalyzer.ident, 205);

            if (_symbol != LexicalAnalyzer.semicolon)
            {
                ErrorHandler.AddError(InputOutput.PositionNow, 207);
                SkipTo(new HashSet<byte> {
                        LexicalAnalyzer.varsy,
                        LexicalAnalyzer.typesy,
                        LexicalAnalyzer.beginsy
                });
            }
            else
            {
                accept(LexicalAnalyzer.semicolon, 207);
            }

            Block();
            accept(LexicalAnalyzer.point, 215);
        }

        // Блок
        public void Block()
        {
            TypeSection();
            VarSection();
            Operator();
        }

        // Секция типов

        private void TypeSection()
        {
            if (_symbol != LexicalAnalyzer.typesy)
            {
                return;
            }
            NextSym();
            TypeDeclaration();
            accept(LexicalAnalyzer.semicolon, 207);
            while (_symbol == LexicalAnalyzer.ident)
            {
                TypeDeclaration();
                accept(LexicalAnalyzer.semicolon, 207);
            }
        }

        private void TypeDeclaration()
        {
            accept(LexicalAnalyzer.ident, 205);
            accept(LexicalAnalyzer.equal, 213);
            TypeSpec();
        }

        private void TypeSpec()
        {
            if (_symbol == LexicalAnalyzer.recordsy)
            {
                RecordType();
            }
            else if (_symbol == LexicalAnalyzer.arraysy)
            {
                ArrayType();
            }
            else
            {
                SimpleType();
            }
        }

        private void ArrayType()
        {
            if (_symbol == LexicalAnalyzer.arraysy)
            {
                NextSym();  
            }

            accept(LexicalAnalyzer.lbracket, 221);
            accept(LexicalAnalyzer.intc, 222);
            accept(LexicalAnalyzer.twopoints, 223);
            accept(LexicalAnalyzer.intc, 222);
            accept(LexicalAnalyzer.rbracket, 224);
            accept(LexicalAnalyzer.ofsy, 225);
            SimpleType();
        }

        private void RecordType()
        {
            accept(LexicalAnalyzer.recordsy, 213);
            while (_symbol == LexicalAnalyzer.ident)
            {
                IdentList();
                accept(LexicalAnalyzer.colon, 206);
                TypeSpec();
                accept(LexicalAnalyzer.semicolon, 207);
            }
            accept(LexicalAnalyzer.endsy, 209);
        }

        // Список идентификаторов
        private void IdentList()
        {
            accept(LexicalAnalyzer.ident, 205);
            while (_symbol == LexicalAnalyzer.comma)
            {
                NextSym();
                accept(LexicalAnalyzer.ident, 205);
            }
        }

        // Простые типы
        private void SimpleType()
        {
            if (_symbol == LexicalAnalyzer.ident)
            {
                accept(LexicalAnalyzer.ident, 208);
            }
            else
            {
                ErrorHandler.AddError(InputOutput.PositionNow, 208);
                SkipTo(new HashSet<byte> {
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.endsy,
                    LexicalAnalyzer.rightpar
                });
            }
        }

        // Секция переменных
        private void VarSection()
        {
            if (_symbol != LexicalAnalyzer.varsy)
            {
                return;
            }
            NextSym();
            VarDeclaration();
            accept(LexicalAnalyzer.semicolon, 207);
            while (_symbol == LexicalAnalyzer.ident)
            {
                VarDeclaration();
                accept(LexicalAnalyzer.semicolon, 207);
            }
        }

        private void VarDeclaration()
        {
            IdentList();
            accept(LexicalAnalyzer.colon, 206);
            TypeSpec();
        }

        // Операторы
        private void Operator()
        {
            if (_symbol == LexicalAnalyzer.beginsy)
            {
                CompoundOperator();
            }
            else if (_symbol == LexicalAnalyzer.ifsy)
            {
                IfOperator();
            }
            else if (_symbol == LexicalAnalyzer.whilesy)
            {
                WhileOperator();
            }
            else if (_symbol == LexicalAnalyzer.repeatsy)
            {
                RepeatOperator();
            }
            else if (_symbol == LexicalAnalyzer.forsy)
            {
                ForOperator();
            }
            else if (_symbol == LexicalAnalyzer.withsy)
            {
                WithOperator();
            }
            else if (_symbol == LexicalAnalyzer.ident)
            {
                AssignOperator();
            }
            else
            {
                ErrorHandler.AddError(InputOutput.PositionNow, 213);
                SkipTo(new HashSet<byte> {
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.endsy,
                    LexicalAnalyzer.elsesy,
                    LexicalAnalyzer.untilsy
                });
            }
        }

        // Составной оператор
        private void CompoundOperator()
        {
            accept(LexicalAnalyzer.beginsy, 210);
            Operator();
            while (_symbol == LexicalAnalyzer.semicolon)
            {
                NextSym();
                if (_symbol == LexicalAnalyzer.endsy)
                    break;
                Operator();
            }
            accept(LexicalAnalyzer.endsy, 209);
        }

        // Оператор условия
        private void IfOperator()
        {
            accept(LexicalAnalyzer.ifsy, 230);
            Expression();
            accept(LexicalAnalyzer.thensy, 231);
            Operator();
            if (_symbol == LexicalAnalyzer.elsesy)
            {
                NextSym();
                Operator();
            }
        }

        // Оператор цикла
        private void WhileOperator()
        {
            accept(LexicalAnalyzer.whilesy, 240);
            Expression();
            accept(LexicalAnalyzer.dosy, 241);
            Operator();
        }

        private void RepeatOperator()
        {
            accept(LexicalAnalyzer.repeatsy, 242);
            Operator();
            while (_symbol == LexicalAnalyzer.semicolon)
            {
                NextSym();
                if (_symbol == LexicalAnalyzer.untilsy)
                    break;
                Operator();
            }
            accept(LexicalAnalyzer.untilsy, 243);
            Expression();
        }

        private void ForOperator()
        {
            accept(LexicalAnalyzer.forsy, 244);
            Variable();

            if (_symbol == LexicalAnalyzer.assign)
                accept(LexicalAnalyzer.assign, 211);
            else
                ErrorHandler.AddError(InputOutput.PositionNow, 211);

            Expression();

            if (_symbol == LexicalAnalyzer.tosy)
                accept(LexicalAnalyzer.tosy, 245);
            else if (_symbol == LexicalAnalyzer.downtosy)
                accept(LexicalAnalyzer.downtosy, 246);
            else
                ErrorHandler.AddError(InputOutput.PositionNow, 247);

            Expression();
            accept(LexicalAnalyzer.dosy, 241);
            Operator();
        }

        private void WithOperator()
        {
            accept(LexicalAnalyzer.withsy, 212);
            Variable();
            accept(LexicalAnalyzer.dosy, 213);
            Operator();
        }

        // Переменная
        private void Variable()
        {
            accept(LexicalAnalyzer.ident, 205);
            while (_symbol == LexicalAnalyzer.point || _symbol == LexicalAnalyzer.lbracket)
            {
                if (_symbol == LexicalAnalyzer.point)
                {
                    NextSym();
                    accept(LexicalAnalyzer.ident, 205);
                }
                else if (_symbol == LexicalAnalyzer.lbracket)
                {
                    NextSym();
                    Expression();
                    accept(LexicalAnalyzer.rbracket, 224);
                }
            }
        }

        // Оператор присваивания

        private void AssignOperator()
        {
            Variable();
            accept(LexicalAnalyzer.assign, 211);
            Expression();
        }

        // Выражения
        private void Expression()
        {
            SimpleExpression();
            if (_symbol == LexicalAnalyzer.equal ||
                _symbol == LexicalAnalyzer.later ||
                _symbol == LexicalAnalyzer.greater ||
                _symbol == LexicalAnalyzer.laterequal ||
                _symbol == LexicalAnalyzer.greaterequal ||
                _symbol == LexicalAnalyzer.latergreater)
            {
                NextSym();
                SimpleExpression();
            }
        }

        private void SimpleExpression()
        {
            if (_symbol == LexicalAnalyzer.plus || _symbol == LexicalAnalyzer.minus)
            {
                NextSym();
            }
            Term();
            while (_symbol == LexicalAnalyzer.plus || _symbol == LexicalAnalyzer.minus || _symbol == LexicalAnalyzer.orsy)
            {
                NextSym();
                Term();
            }
        }

        private void Term()
        {
            Factor();
            while (_symbol == LexicalAnalyzer.star || _symbol == LexicalAnalyzer.slash ||
                   _symbol == LexicalAnalyzer.divsy || _symbol == LexicalAnalyzer.modsy ||
                   _symbol == LexicalAnalyzer.andsy)
            {
                NextSym();
                Factor();
            }
        }

        private void Factor()
        {
            if (_symbol == LexicalAnalyzer.ident)
            {
                Variable();
            }
            else if (_symbol == LexicalAnalyzer.intc)
            {
                NextSym();
            }
            else if (_symbol == LexicalAnalyzer.floatc)
            {
                NextSym();
            }
            else if (_symbol == LexicalAnalyzer.leftpar)
            {
                NextSym();
                Expression();
                accept(LexicalAnalyzer.rightpar, 248);
            }
            else if (_symbol == LexicalAnalyzer.notsy)
            {
                NextSym();
                Factor();
            }
            else
            {
                ErrorHandler.AddError(InputOutput.PositionNow, 249);
                SkipTo(new HashSet<byte> {
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.rightpar,
                    LexicalAnalyzer.comma,
                    LexicalAnalyzer.endsy,
                    LexicalAnalyzer.thensy,
                    LexicalAnalyzer.dosy,
                    LexicalAnalyzer.untilsy,
                    LexicalAnalyzer.elsesy,
                    LexicalAnalyzer.rbracket
                });
            }
        }
    }
}