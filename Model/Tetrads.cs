using CompilerDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.Model
{
    internal class Tetrads : ViewModelBase
    {
        private string _op;
        private string _arg1;
        private string _arg2;
        private string _result;

        public Tetrads(string op, string arg1, string arg2, string result)
        {
            Op = op;
            Arg1 = arg1;
            Arg2 = arg2;
            Result = result;
        }

        public string Op
        {
            get { return _op; }
            set { _op = value; OnPropertyChanged(); }
        }
        public string Arg1
        {
            get { return _arg1; }
            set { _arg1 = value; OnPropertyChanged(); }
        }
        public string Arg2
        {
            get { return _arg2; }
            set { _arg2 = value; OnPropertyChanged(); }
        }
        public string Result
        {
            get { return _result; }
            set { _result = value; OnPropertyChanged(); }
        }
    }
}
