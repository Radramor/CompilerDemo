using CompilerDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompilerDemo.ViewModel
{
    internal class TetradsViewModel
    {
        private List<Tetrads> tetrads = new List<Tetrads>();

        public List<Tetrads> CreateTetrads(List<Token> Tokens)
        {
            if (Tokens == null || Tokens.Count == 0) return null;

            foreach (var token in Tokens.ToList())
            {
                if (token.RawToken == " " || token.RawToken == "\n" || token.RawToken == "\r" || token.RawToken == "\t")
                    Tokens.Remove(token);
            }

            SearchParenthesis(ref Tokens);

            for (int i = 0; i < Tokens.Count(); i++)
            {
                //отрицательное число
                if (Tokens[i].RawToken == "-" && (
                    i == 0 ||
                    Tokens[i - 1].RawToken == "+" ||
                    Tokens[i - 1].RawToken == "-" ||
                    Tokens[i - 1].RawToken == "*" ||
                    Tokens[i - 1].RawToken == "/"))
                {
                    tetrads.Add(new Tetrads("minus", Tokens[i + 1].RawToken, "", "t" + tetrads.Count()));
                    Tokens[i] = new Token(tetrads.Last().Result, Tokens[i].StartPos);
                    Tokens.RemoveAt(i + 1);
                    CreateTetrads(Tokens);
                    return tetrads;

                }
            }

            for (int i = 0; i < Tokens.Count(); i++)
            {
                // Умножение двух чисел
                if (Tokens[i].RawToken == "*")
                {
                    string operand1 = Tokens[i - 1].RawToken;
                    string operand2 = Tokens[i + 1].RawToken;
                    tetrads.Add(new Tetrads("multiply", operand1, operand2, "t" + tetrads.Count()));
                    Tokens[i - 1] = new Token(tetrads.Last().Result, Tokens[i - 1].StartPos);
                    Tokens.RemoveAt(i);
                    Tokens.RemoveAt(i);
                    CreateTetrads(Tokens);
                    return tetrads;
                }
                // Деление двух чисел
                if (Tokens[i].RawToken == "/")
                {
                    string operand1 = Tokens[i - 1].RawToken;
                    string operand2 = Tokens[i + 1].RawToken;
                    tetrads.Add(new Tetrads("divide", operand1, operand2, "t" + tetrads.Count()));
                    Tokens[i - 1] = new Token(tetrads.Last().Result, Tokens[i - 1].StartPos);
                    Tokens.RemoveAt(i);
                    Tokens.RemoveAt(i);
                    CreateTetrads(Tokens);
                    return tetrads;
                }
            }

            for (int i = 0; i < Tokens.Count(); i++)
            {
                // Сложение двух чисел
                if (Tokens[i].RawToken == "+")
                {
                    string operand1 = Tokens[i - 1].RawToken;
                    string operand2 = Tokens[i + 1].RawToken;
                    tetrads.Add(new Tetrads("plus", operand1, operand2, "t" + tetrads.Count()));
                    Tokens[i - 1] = new Token(tetrads.Last().Result, Tokens[i - 1].StartPos);
                    Tokens.RemoveAt(i);
                    Tokens.RemoveAt(i);
                    CreateTetrads(Tokens);
                    return tetrads;
                }

                // Вычитание двух чисел
                if (Tokens[i].RawToken == "-")
                {
                    string operand1 = Tokens[i - 1].RawToken;
                    string operand2 = Tokens[i + 1].RawToken;
                    tetrads.Add(new Tetrads("minus", operand1, operand2, "t" + tetrads.Count()));
                    Tokens[i - 1] = new Token(tetrads.Last().Result, Tokens[i - 1].StartPos);
                    Tokens.RemoveAt(i);
                    Tokens.RemoveAt(i);
                    CreateTetrads(Tokens);
                    return tetrads;
                }
            }

            for (int i = 0; i < Tokens.Count(); i++)
            {
                if (Tokens[i].RawToken == "=" && Tokens.Count == 3)
                {
                    string operand1 = Tokens[i - 1].RawToken;
                    string operand2 = Tokens[i + 1].RawToken;
                    tetrads.Add(new Tetrads("equals", operand2, "", operand1));
                    Tokens[i - 1] = new Token(tetrads.Last().Result, Tokens[i - 1].StartPos);
                    Tokens.RemoveAt(i);
                    Tokens.RemoveAt(i);
                    return tetrads;
                }
            }
            return tetrads;
        }

        private void SearchParenthesis(ref List<Token> Tokens)
        {
            Stack<Token> stack = new Stack<Token>();
            Token OpenParenthesis = null;
            Token CloseParenthesis = null;
            foreach (var token in Tokens)
            {
                if (token.RawToken == "(" && stack.Count == 0) 
                {
                    OpenParenthesis = token;
                    stack.Push(token);
                }
                else if(token.RawToken == "(")
                    stack.Push(token);

                if (token.RawToken == ")" && stack.Count == 1)
                {
                    CloseParenthesis = token;
                    stack.Pop();
                }
                else if (token.RawToken == ")" && stack.Count > 1)
                    stack.Pop();

                if (OpenParenthesis != null && CloseParenthesis != null) break;
            }

            if (OpenParenthesis != null && CloseParenthesis != null)
            {
                List<Token> tokensBuff = new List<Token>(Tokens.Skip(Tokens.IndexOf(OpenParenthesis) + 1).Take(Tokens.IndexOf(CloseParenthesis) - Tokens.IndexOf(OpenParenthesis) - 1));
                CreateTetrads(tokensBuff);

                // Замена подвыражения в скобках на результат
                int startIndex = Tokens.IndexOf(OpenParenthesis);
                int endIndex = Tokens.IndexOf(CloseParenthesis) + 1;
                Tokens.RemoveRange(startIndex, endIndex - startIndex);
                Tokens.Insert(startIndex, new Token(tetrads.Last().Result, OpenParenthesis.StartPos));
            }

            foreach (var token in Tokens.ToList())
            {
                if (token.RawToken == "(") CreateTetrads(Tokens);
            }
        }
    }
}