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
                if (Tokens[i].RawToken == "*")
                {
                    if (i == 0 || i == Tokens.Count - 1)
                    {
                        tetrads.Add(new Tetrads("multiply", "Ошибка: отсутствует один из аргументов для операции", "", ""));
                        return tetrads;
                    }
                    string operand1 = Tokens[i - 1].RawToken;
                    string operand2 = Tokens[i + 1].RawToken;
                    tetrads.Add(new Tetrads("multiply", operand1, operand2, "t" + tetrads.Count()));
                    Tokens[i - 1] = new Token(tetrads.Last().Result, Tokens[i - 1].StartPos);
                    Tokens.RemoveAt(i);
                    Tokens.RemoveAt(i);
                    CreateTetrads(Tokens);
                    return tetrads;
                }
                if (Tokens[i].RawToken == "/")
                {
                    if (i == 0 || i == Tokens.Count - 1)
                    {
                        tetrads.Add(new Tetrads("divide", "Ошибка: отсутствует один из аргументов для операции", "", ""));
                        return tetrads;
                    }
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
                if (Tokens[i].RawToken == "+")
                {
                    if (i == 0 || i == Tokens.Count - 1)
                    {
                        tetrads.Add(new Tetrads("plus", "Ошибка: отсутствует один из аргументов для операции", "", ""));
                        return tetrads;
                    }
                    string operand1 = Tokens[i - 1].RawToken;
                    string operand2 = Tokens[i + 1].RawToken;
                    tetrads.Add(new Tetrads("plus", operand1, operand2, "t" + tetrads.Count()));
                    Tokens[i - 1] = new Token(tetrads.Last().Result, Tokens[i - 1].StartPos);
                    Tokens.RemoveAt(i);
                    Tokens.RemoveAt(i);
                    CreateTetrads(Tokens);
                    return tetrads;
                }

                if (Tokens[i].RawToken == "-")
                {
                    if (i == 0 || i == Tokens.Count - 1)
                    {
                        tetrads.Add(new Tetrads("minus", "Ошибка: отсутствует один из аргументов для операции", "", ""));
                        return tetrads;
                    }
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
                if (token.RawToken == "(")
                {
                    if (OpenParenthesis == null)
                        OpenParenthesis = token;
                    stack.Push(token);
                }
                else if (token.RawToken == ")")
                {
                    if (stack.Count == 0)
                    {
                        // Ошибка: закрывающая скобка без открывающей
                        tetrads.Add(new Tetrads("error", "Лишняя закрывающая скобка" , "", ""));
                        return;
                    }
                    stack.Pop();
                    if (stack.Count == 0)
                        CloseParenthesis = token;
                }
                
                if (OpenParenthesis != null && CloseParenthesis != null) break;
            }

            if (OpenParenthesis != null && CloseParenthesis != null)
            {
                List<Token> tokensBuff = new List<Token>(Tokens.Skip(Tokens.IndexOf(OpenParenthesis) + 1).Take(Tokens.IndexOf(CloseParenthesis) - Tokens.IndexOf(OpenParenthesis) - 1));
                CreateTetrads(tokensBuff);

                int startIndex = Tokens.IndexOf(OpenParenthesis);
                int endIndex = Tokens.IndexOf(CloseParenthesis) + 1;
                Tokens.RemoveRange(startIndex, endIndex - startIndex);
                Tokens.Insert(startIndex, new Token(tetrads.Last().Result, OpenParenthesis.StartPos));
            }

            else if (OpenParenthesis != null)
            {
                // Ошибка: открывающая скобка без закрывающей
                tetrads.Add(new Tetrads("error", "Лишняя открывающая скобка", "", ""));
                return;
            }

            foreach (var token in Tokens.ToList())
            {
                if (token.RawToken == "(") CreateTetrads(Tokens);
            }
        }
    }
}