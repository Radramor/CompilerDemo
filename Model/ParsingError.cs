using CompilerDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.Model
{
    public class ParsingError : ViewModelBase
    {
        private int _numberOfError { get; set; }
        private string _message { get; set; }
        private int _startPos { get; set; }
        private int _endPos { get; set; }
        private string _expectedToken { get; set; }

        public int NumberOfError
        {
            get { return _numberOfError; }
            set { _numberOfError = value; OnPropertyChanged(); }
        }
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }
        public int StartPos
        {
            get { return _startPos; }
            set { _startPos = value; OnPropertyChanged(); }
        }
        public int EndPos
        {
            get { return _endPos; }
            set { _endPos = value; OnPropertyChanged(); }
        }
        public string ExpectedToken
        {
            get { return _expectedToken; }
            set { _expectedToken = value; OnPropertyChanged(); }
        }
    }
}
