using CompilerDemo.Model;
using CompilerDemo.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace CompilerDemo.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private string path = string.Empty;
        private string _text;
        private Lexer _lexer = new Lexer(); 
        private Parser _parser = new Parser();
        private ObservableCollection<TokenViewModel> _tokenViewModels= new ObservableCollection<TokenViewModel>();
        private ObservableCollection<ParsingError> _parsingError = new ObservableCollection<ParsingError>();


        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }
        public ICommand RunAndNeutralizationCommand { get; }
        public ICommand NeutralizationCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ReferenceCommand { get; }
        public ICommand AboutProgramCommand { get; }
        public ICommand OpenBibliographyCommand { get; }
        public ICommand OpenFormulationOfTheProblemCommand { get; }
        public ICommand OpenGrammaticsCommand { get; }
        public ICommand OpenMethodCommand { get; }


        public MainWindowViewModel()
        {
            RunAndNeutralizationCommand = new RelayCommand(RunAndNeutralization);
            NeutralizationCommand = new RelayCommand(Neutralization);
            RunCommand = new RelayCommand(Run);
            CreateCommand = new RelayCommand(Create);
            OpenCommand = new RelayCommand(TryOpen);
            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            DeleteCommand = new RelayCommand(Delete);
            ExitCommand = new RelayCommand(Exit);
            ReferenceCommand = new RelayCommand(Reference);
            OpenBibliographyCommand = new RelayCommand(OpenBibliography);
            OpenFormulationOfTheProblemCommand = new RelayCommand(OpenFormulationOfTheProblem);
            OpenGrammaticsCommand = new RelayCommand(OpenGrammatics);
            OpenMethodCommand = new RelayCommand(OpenMethod);
            AboutProgramCommand = new RelayCommand(AboutProgram);
        }

        private void OpenBibliography() 
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\Bibliography.html")
            {
                UseShellExecute = true
            };
            p.Start();
        }
        private void OpenFormulationOfTheProblem()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\FormulationOfTheProblem.html")
            {
                UseShellExecute = true
            };
            p.Start();
        }
        private void OpenGrammatics()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\Grammatics.html")
            {
                UseShellExecute = true
            };
            p.Start();
        }
        private void OpenMethod() 
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\OpenMethod.html")
            {
                UseShellExecute = true
            };
            p.Start();
        }
        private void RunAndNeutralization()
        {
            Run();
            Neutralization();
        }
        private void Neutralization()
        {
            while (ParsingErrors.Count > 0)
            {
                NeutralizationOfErrors.Neutralization(ParsingErrors, Text);
                Run();
            }
           
        }

        private void Run()
        { 
            Scan();
            Parse();
            PrintCountErrors();
        }
        private void Parse()
        {
            ParsingErrors.Clear();
            List<ParsingError> errorList = _parser.Parse(TokenViewModels);
            foreach (ParsingError error in errorList)
            {
                ParsingErrors.Add(error);
            }
        }
        private void Scan()
        {
            if (Text == string.Empty)
            {
                 return;
            }

            TokenViewModels.Clear();
            List<Token> Tokens = _lexer.Scan(Text);
            foreach (Token token in Tokens)
            {
                TokenViewModels.Add(new TokenViewModel(token));
            }
            
        }
        private void Create()
        {
            if (string.IsNullOrWhiteSpace(Text)) return;
            var result = MessageBox.Show("Вы хотите сохранить изменения в файле?", "Компилятор", MessageBoxButton.YesNoCancel, MessageBoxImage.None, MessageBoxResult.Yes);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        path = saveFileDialog.FileName;
                        File.WriteAllText(path, Text);
                        Text = string.Empty;
                    }
                    break;

                case MessageBoxResult.No:
                    Text = string.Empty;
                    break;

                case MessageBoxResult.Cancel:
                    return;
            }
        }
        private void Save()
        {
            if (File.Exists(path))
            {
                File.WriteAllText(path, Text);
            }
            else SaveAs();
        }
        private void TryOpen()
        {
            if (!string.IsNullOrWhiteSpace(Text) && !File.Exists(path))
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения в файле?", "Компилятор",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.None, MessageBoxResult.Yes);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            path = saveFileDialog.FileName;
                            File.WriteAllText(path, Text);
                            Text = string.Empty;
                            Open();
                        }
                        break;

                    case MessageBoxResult.No:
                        Text = string.Empty;
                        Open();
                        break;

                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else if (!string.IsNullOrWhiteSpace(Text) && File.Exists(path))
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения в файле \n" + path + "?", "Компилятор",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.None, MessageBoxResult.Yes);
                switch (result)
                {
                    case MessageBoxResult.Yes:

                        File.WriteAllText(path, Text);
                        Text = string.Empty;
                        Open();
                        break;

                    case MessageBoxResult.No:
                        Text = string.Empty;
                        Open();
                        break;

                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else Open();
        }
        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                string buffer = File.ReadAllText(path);
                Text = buffer;
            }
        }
        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                path = saveFileDialog.FileName;
                File.WriteAllText(path, Text);
            }
        }
        private void Exit()
        {
            Environment.Exit(0);
        }

        private void Delete()
        {
            Text = string.Empty;
        }

        private void Reference()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\Reference.html")
            {
                UseShellExecute = true
            };
            p.Start();
        }
        private void AboutProgram()
        {
            AboutProgramWindow window = new AboutProgramWindow();
            window.DataContext = this;
            window.Show();
        }
        public void PrintCountErrors()
        {
            if (ParsingErrors.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).TB.Clear();
                ((MainWindow)System.Windows.Application.Current.MainWindow).TB.AppendText(Text);
            }
            else
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).TB.Clear();
                ((MainWindow)System.Windows.Application.Current.MainWindow).TB.AppendText("Количество ошибок: " + ParsingErrors.Count);
            }
        }
        public ObservableCollection<TokenViewModel> TokenViewModels
        {
            get { return _tokenViewModels; }
            set { _tokenViewModels = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ParsingError> ParsingErrors
        {
            get { return _parsingError; }
            set { _parsingError = value; OnPropertyChanged(); }
        }
    }  
}


