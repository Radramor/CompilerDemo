using CompilerDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.Model
{
    public class Parser
    {
        private List<ParsingError> Errors { get; set; }
        private int errorNumber;

        public List<ParsingError> Parse(ObservableCollection<TokenViewModel> tokenViewModels)
        {
            Errors = new List<ParsingError>();
            errorNumber = 1;
            Stack<TokenViewModel> stack = new Stack<TokenViewModel>();
            int idx = 0;
            if (tokenViewModels.Count == 0)
            {
                CreateError("Отсутсвует какой-либо токен", null);
                return Errors;
            }
            while (idx < tokenViewModels.Count)
            {
                if (tokenViewModels[idx].RawToken == "(*" || tokenViewModels[idx].RawToken == "{")
                {
                    stack.Push(tokenViewModels[idx]);
                }
                else if ((tokenViewModels[idx].RawToken == "*)" && stack.Count > 0 && stack.Peek().RawToken == "(*") ||
                            (tokenViewModels[idx].RawToken == "}" && stack.Count > 0 && stack.Peek().RawToken == "{"))
                {
                    stack.Pop();
                }
                else if((tokenViewModels[idx].RawToken == "*)" &&  stack.Count == 0) ||
                            (tokenViewModels[idx].RawToken == "}" && stack.Count == 0))
                {
                    CreateError("Лишний токен", tokenViewModels[idx]);
                    errorNumber++;
                }
                else if((tokenViewModels[idx].RawToken == "*)" && stack.Peek().RawToken != "(*") ||
                            (tokenViewModels[idx].RawToken == "}" && stack.Peek().RawToken != "{"))
                {
                    CreateError("Лишний токен", tokenViewModels[idx]);
                    errorNumber++;
                }
                
                idx++;
            }

            if (stack.Count != 0)
            {
                foreach (var i in stack) 
                {
                    CreateError("Нет закрывающего токена", i);
                    errorNumber++;
                }
            }
            return Errors;
        }
        private void CreateError(string message, TokenViewModel? token) 
        {
            if (token == null)
                Errors.Add(new ParsingError
                {
                    NumberOfError = errorNumber,
                    Message = $"Отсутсвует какой-либо токен",
                    StartPos = 0,
                    EndPos = 0,
                    ExpectedToken = ""
                });
            else 
                Errors.Add(new ParsingError
                {
                    NumberOfError = errorNumber,
                    Message = message,
                    StartPos = int.Parse(token.StartPos),
                    EndPos = int.Parse(token.EndPos),
                    ExpectedToken = token.RawToken
                });
        }
    }
}
