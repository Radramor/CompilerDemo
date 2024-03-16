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
                Errors.Add(new ParsingError
                {
                    NumberOfError = errorNumber,
                    Message = $"Отсутсвует какой-либо токен",
                    StartPos = 0,
                    EndPos = 0,
                    ExpectedToken = ""
                });

                return Errors;
            }
            while (idx < tokenViewModels.Count)
            {
                if (tokenViewModels[idx].RawToken == "(*" || tokenViewModels[idx].RawToken == "{" || tokenViewModels[idx].RawToken == "//")
                {
                    stack.Push(tokenViewModels[idx]);
                }
                else if ((tokenViewModels[idx].RawToken == "*)" && stack.Count > 0 && stack.Peek().RawToken == "(*") ||
                            (tokenViewModels[idx].RawToken == "}" && stack.Count > 0 && stack.Peek().RawToken == "{") ||
                            (tokenViewModels[idx].RawToken == "\n" && stack.Count > 0 && stack.Peek().RawToken == "//"))
                {
                    stack.Pop();
                }
                else if((tokenViewModels[idx].RawToken == "*)" && stack.Count == 0) ||
                            (tokenViewModels[idx].RawToken == "}" && stack.Count == 0))
                {
                    Errors.Add(new ParsingError
                    {
                        NumberOfError = errorNumber,
                        Message = $"Лишний токен ",
                        StartPos = int.Parse(tokenViewModels[idx].StartPos),
                        EndPos = int.Parse(tokenViewModels[idx].EndPos),
                        ExpectedToken = tokenViewModels[idx].RawToken
                    });
                    errorNumber++;
                }
                
                idx++;
            }

            if (stack.Count != 0)
            {
                foreach (var i in stack) 
                {
                    Errors.Add(new ParsingError
                    {   
                        NumberOfError = errorNumber,
                        Message = $"нет закрывающего токена ",
                        StartPos = int.Parse(i.StartPos),
                        EndPos = int.Parse(i.EndPos),
                        ExpectedToken = i.RawToken
                    });
                    errorNumber++;
                }
            }
            return Errors;
        }
    }
}
