using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Compiler
{
    internal class SyntaxAnalyzer
    {
        private LexicalAnalyzer _lex;
        private bool _hasError = false;

        public SyntaxAnalyzer(LexicalAnalyzer lex)
        {
            _lex = lex;
        }

        public void Analyze()
        {
            _lex.NextToken();
            ParseProgram();
        }

        private void ParseProgram()
        {
            if (_lex.CurrentCode != 1)
            {
                InputOutput.AddError(301, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            if (_lex.CurrentCode != LexicalAnalyzer.IdentifierCode)
            {
                InputOutput.AddError(302, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            if (_lex.CurrentCode != (int)';')
            {
                InputOutput.AddError(303, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            ParseVarSection();
            ParseCompoundStatement();

            if (InputOutput.HasUnclosedComment)
            {
                return;
            }

            if (_lex.CurrentCode != (int)'.')
            {
                InputOutput.AddError(314, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

        }

        private void ParseVarSection()
        {
            if (_lex.CurrentCode != 2)
            {
                return;
            }

            _lex.NextToken();

            while (_lex.CurrentCode == LexicalAnalyzer.IdentifierCode)
            {
                ParseVariableDeclaration();
            }
        }

        private void ParseCompoundStatement()
        {
            if (_lex.CurrentCode != 4)
            {
                InputOutput.AddError(306, _lex.CurrentPosition);
                return;
            }

            _lex.NextToken();

            while (_lex.CurrentCode != 0 && _lex.CurrentCode != 5)
            {
                ParseStatement();

                if (_lex.CurrentCode == (int)';')
                {
                    _lex.NextToken();
                }
                else if (_lex.CurrentCode != 5 && _lex.CurrentCode != 0)
                {
                    if (!_hasError)
                    {
                        InputOutput.AddError(303, _lex.CurrentPosition);
                    }
                    _lex.NextToken();
                }
            }

            if (_lex.CurrentCode == 0)
            {
                return;
            }

            if (_lex.CurrentCode != 5)
            {
                InputOutput.AddError(307, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }
        }

        private void ParseStatement()
        {
            while (_lex.CurrentCode == 100) 
            {
                _lex.NextToken();
            }

            if (_lex.CurrentCode == LexicalAnalyzer.IdentifierCode)
            {
                ParseAssignment();
            }
            else if (_lex.CurrentCode == 4)
            {
                ParseCompoundStatement();
            }
            else if (_lex.CurrentCode == 9)
            {
                ParseIfStatement();
            }
            else if (_lex.CurrentCode == 12)
            {
                ParseWhileStatement();
            }
            else if (_lex.CurrentCode == 17)
            {
                ParseRepeatStatement();
            }
            else if (_lex.CurrentCode == 14)
            {
                ParseForStatement();
            }
            else
            {
                if (!_hasError)
                {
                    InputOutput.AddError(309, _lex.CurrentPosition);
                }
                SkipToNextStatement();
            }
        }

        private void ParseIfStatement()
        {
            _lex.NextToken();
            ParseExpression();

            if (_lex.CurrentCode != 10)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(310, _lex.CurrentPosition);
                    _hasError = true;
                }
                SkipToNextStatement();
                return;
            }

            _lex.NextToken();
            ParseStatement();

            if (_lex.CurrentCode == 11)
            {
                _lex.NextToken();
                ParseStatement();
            }
        }

        private void ParseWhileStatement()
        {
            _lex.NextToken();
            ParseExpression();

            if (_lex.CurrentCode != 13)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(311, _lex.CurrentPosition);
                    _hasError = true;
                }
                SkipToNextStatement();
                return;
            }

            _lex.NextToken();
            ParseStatement();
        }

        private void ParseRepeatStatement()
        {
            _lex.NextToken();

            while (_lex.CurrentCode != 0 && _lex.CurrentCode != 18)
            {
                while (_lex.CurrentCode == (int)';')
                {
                    _lex.NextToken();
                }
                ParseStatement();
                if (_lex.CurrentCode == (int)';')
                {
                    _lex.NextToken();
                }
                else if (_lex.CurrentCode != 18 && _lex.CurrentCode != 0)
                {
                    if (!_hasError)
                    {
                        InputOutput.AddError(303, _lex.CurrentPosition);
                    }
                    _lex.NextToken();
                }
            }


            if (_lex.CurrentCode != 18)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(312, _lex.CurrentPosition);
                    _hasError = true;
                }
                return;
            }
            _lex.NextToken();
            ParseExpression();
        }

        private void ParseForStatement()
        {
            _lex.NextToken();

            if (_lex.CurrentCode != LexicalAnalyzer.IdentifierCode)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(302, _lex.CurrentPosition);
                    _hasError = true;
                }
                SkipToNextStatement();
                return;
            }

            _lex.NextToken();

            if (_lex.CurrentCode != LexicalAnalyzer.AssignCode)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(308, _lex.CurrentPosition);
                    _hasError = true;
                }
                SkipToNextStatement();
                return;
            }

            _lex.NextToken();
            ParseExpression();

            if (_lex.CurrentCode != 15 && _lex.CurrentCode != 16)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(317, _lex.CurrentPosition);
                    _hasError = true;
                }
                SkipToNextStatement();
                return;
            }

            _lex.NextToken();
            ParseExpression();

            if (_lex.CurrentCode != 13)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(311, _lex.CurrentPosition);
                    _hasError = true;
                }
                SkipToNextStatement();
                return;
            }

            _lex.NextToken();
            ParseStatement();
        }

        private void ParseAssignment()
        {
            ParseVariable();

            if (_lex.CurrentCode != LexicalAnalyzer.AssignCode)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(308, _lex.CurrentPosition);
                    _hasError = true;
                }
                SkipToNextStatement();
                return;
            }

            _lex.NextToken();
            ParseExpression();
        }

        private void ParseVariable()
        {
            if (_lex.CurrentCode != LexicalAnalyzer.IdentifierCode)
            {
                if (!_hasError)
                {
                    InputOutput.AddError(302, _lex.CurrentPosition);
                }
                return;
            }

            _lex.NextToken();

            if (_lex.CurrentCode == LexicalAnalyzer.LeftSquareBracketCode)
            {
                _lex.NextToken();
                ParseExpression();

                if (_lex.CurrentCode != LexicalAnalyzer.RightSquareBracketCode)
                {
                    if (!_hasError)
                    {
                        InputOutput.AddError(315, _lex.CurrentPosition);
                        _hasError = true;
                    }
                    SkipTo(LexicalAnalyzer.RightSquareBracketCode);
                    if (_lex.CurrentCode == LexicalAnalyzer.RightSquareBracketCode)
                    {
                        _lex.NextToken();
                    }
                }
                else
                {
                    _lex.NextToken();
                }
            }
        }

        private void ParseExpression()
        {
            ParseSimpleExpression();

            if (IsRelationOperator(_lex.CurrentCode))
            {
                _lex.NextToken();
                ParseSimpleExpression();
            }
        }

        private void ParseSimpleExpression()
        {
            if (_lex.CurrentCode == (int)'+' || _lex.CurrentCode == (int)'-')
            {
                _lex.NextToken();
            }

            ParseTerm();

            while (_lex.CurrentCode == (int)'+' || _lex.CurrentCode == (int)'-')
            {
                _lex.NextToken();
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();

            while (_lex.CurrentCode == (int)'*' || _lex.CurrentCode == (int)'/')
            {
                _lex.NextToken();
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (_lex.CurrentCode == LexicalAnalyzer.IdentifierCode)
            {
                ParseVariable();
            }
            else if (_lex.CurrentCode == LexicalAnalyzer.IntegerCode)
            {
                _lex.NextToken();
            }
            else if (_lex.CurrentCode == (int)'(')
            {
                _lex.NextToken();
                ParseExpression();

                if (_lex.CurrentCode != (int)')')
                {
                    if (!_hasError)
                    {
                        InputOutput.AddError(309, _lex.CurrentPosition);
                    }
                }
                else
                {
                    _lex.NextToken();
                }
            }
            else
            {
                if (!_hasError)
                {
                    InputOutput.AddError(309, _lex.CurrentPosition);
                }
            }
        }

        private bool IsRelationOperator(int code)
        {
            return code == (int)'=' ||
                   code == LexicalAnalyzer.LessCode ||
                   code == LexicalAnalyzer.GreaterCode ||
                   code == LexicalAnalyzer.LessOrEqualCode ||
                   code == LexicalAnalyzer.GreaterOrEqualCode ||
                   code == LexicalAnalyzer.NotEqualCode;
        }

        private void SkipToNextStatement()
        {
            while (_lex.CurrentCode != 0 &&
                   _lex.CurrentCode != (int)';' &&
                   _lex.CurrentCode != 5)
            {
                _lex.NextToken();
            }
            _hasError = false;
        }

        private void ParseVariableDeclaration()
        {
            ParseIdentifierList();

            if (_lex.CurrentCode != (int)':')
            {
                InputOutput.AddError(304, _lex.CurrentPosition);
                SkipTo((int)';');
            }
            else
            {
                _lex.NextToken();
            }

            ParseType();

            if (_lex.CurrentCode != (int)';')
            {
                InputOutput.AddError(303, _lex.CurrentPosition);
                SkipTo((int)';');
            }

            if (_lex.CurrentCode == (int)';')
            {
                _lex.NextToken();
            }
        }

        private void ParseIdentifierList()
        {
            if (_lex.CurrentCode != LexicalAnalyzer.IdentifierCode)
            {
                InputOutput.AddError(302, _lex.CurrentPosition);
                return;
            }

            _lex.NextToken();

            while (_lex.CurrentCode == (int)',')
            {
                _lex.NextToken();

                if (_lex.CurrentCode != LexicalAnalyzer.IdentifierCode)
                {
                    InputOutput.AddError(302, _lex.CurrentPosition);
                    return;
                }

                _lex.NextToken();
            }
        }

        private void ParseType()
        {
            if (_lex.CurrentCode == 6 || _lex.CurrentCode == 7 || _lex.CurrentCode == 8)
            {
                _lex.NextToken();
            }
            else if (_lex.CurrentCode == 19)
            {
                ParseArrayType();
            }
            else
            {
                InputOutput.AddError(305, _lex.CurrentPosition);
            }
        }

        private bool IsArrayBound()
        {
            return _lex.CurrentCode == LexicalAnalyzer.IntegerCode ||
                   _lex.CurrentCode == LexicalAnalyzer.IdentifierCode;
        }

        private void ParseArrayType()
        {
            _lex.NextToken();

            if (_lex.CurrentCode != LexicalAnalyzer.LeftSquareBracketCode)
            {
                InputOutput.AddError(313, _lex.CurrentPosition);
                return;
            }

            _lex.NextToken();

            if (!IsArrayBound())
            {
                InputOutput.AddError(313, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            if (_lex.CurrentCode != LexicalAnalyzer.RangeCode)
            {
                InputOutput.AddError(313, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            if (!IsArrayBound())
            {
                InputOutput.AddError(313, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            if (_lex.CurrentCode != LexicalAnalyzer.RightSquareBracketCode)
            {
                InputOutput.AddError(315, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            if (_lex.CurrentCode != 20)
            {
                InputOutput.AddError(316, _lex.CurrentPosition);
            }
            else
            {
                _lex.NextToken();
            }

            if (_lex.CurrentCode == 6 || _lex.CurrentCode == 7 || _lex.CurrentCode == 8)
            {
                _lex.NextToken();
            }
            else
            {
                InputOutput.AddError(305, _lex.CurrentPosition);
            }
        }

        private void SkipTo(int code)
        {
            while (_lex.CurrentCode != 0 && _lex.CurrentCode != code)
            {
                _lex.NextToken();
            }
        }
    }
}
