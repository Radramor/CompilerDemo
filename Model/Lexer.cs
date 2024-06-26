﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.Model
{
    internal class Lexer
    {
        public List<Token> Scan(string code)
        {
            List<Token> _tokens = new List<Token>();
            
            if (code == null)
            {
                return _tokens;
            }
            
            int position = 0;

            code = code.Replace("\t", "").Replace("\r", "");

            do
            {
                string rawToken = ParseToken(code, position);
                _tokens.Add(new Token(rawToken, position));
                position += rawToken.Length;

            } while (position < code.Length);

            return _tokens;
        }

        private string ParseToken(string code, int position)
        {
            char symbol = code[position];
            if (char.IsWhiteSpace(symbol))
            {
                return symbol.ToString();
            }
            if (char.IsLetter(symbol) || symbol == '_')
            {
                return Parse(code, position, (c) => !char.IsLetterOrDigit(c) && c != '_');
            }
            if (char.IsDigit(symbol))
            {
                return Parse(code, position, (c) => !char.IsDigit(c) && c != '.');
            }
            
            if (symbol == '\"')
            {
                return ParseString(code, position);
            }

            return ParseCommentSymbols(code, position);
        }

        private string Parse(string code, int position, Func<char, bool> stopRule)
        {
            char symbol;
            StringBuilder buffer = new StringBuilder();
            while (position < code.Length)
            {
                symbol = code[position];
                if (stopRule(symbol))
                {
                    break;
                }

                buffer.Append(symbol);
                position++;
            }

            return buffer.ToString();
        }

        private string ParseString(string code, int position)
        {
            char symbol;
            StringBuilder buffer = new StringBuilder();
            int quotesCount = 0;
            while (position < code.Length)
            {
                symbol = code[position];
                if (symbol == '\"')
                {
                    quotesCount++;
                }
                else if (quotesCount == 2 || symbol == '\n')
                {
                    break;
                }

                buffer.Append(symbol);

                position++;
            }

            return buffer.ToString();
        }

        private string ParseCommentSymbols(string code, int position)
        {
            string symbol = code[position].ToString();

            string firstCharacter = "(*";
            string secondCharacter = "*)";
            string thirdCharacter = "//";
            if (position < code.Length - 1)
            {
                if (symbol + code[position + 1] == firstCharacter || symbol + code[position + 1] == secondCharacter || symbol + code[position + 1] == thirdCharacter)
                {
                    symbol += code[position + 1];
                }
            }

            return symbol;
        }
    }
}

