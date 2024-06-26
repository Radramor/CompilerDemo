﻿using CompilerDemo.Model;
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
using System.Text.RegularExpressions;
using System.Text;

namespace CompilerDemo.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private string path = string.Empty;
        private string _text;
        private Lexer _lexer = new Lexer(); 
        private Parser _parser = new Parser();
        private List<Token> _tokens = new List<Token>();
        private TetradsViewModel _tetradsViewModel = new TetradsViewModel();
        private ObservableCollection<Tetrads> _tetrads = new ObservableCollection<Tetrads>();
        private ObservableCollection<TokenViewModel> _tokenViewModels = new ObservableCollection<TokenViewModel>();
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
        public ICommand FindSubstringsCommand { get; }

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

        private ObservableCollection<MatchResult> _matchResults = new ObservableCollection<MatchResult>();

        public ObservableCollection<MatchResult> MatchResults
        {
            get { return _matchResults; }
            set { _matchResults = value; OnPropertyChanged(); }
        }

        public class MatchResult
        {
            public string Substring { get; set; }
            public int Position { get; set; }
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
            if (ParsingErrors.Count == 0)
                CreateTetrads();
            else
                Tetrads.Clear();
        }

        private void CreateTetrads()
        {
            Tetrads.Clear();
            _tetradsViewModel = new TetradsViewModel();
            List<Tetrads> tetradsList = _tetradsViewModel.CreateTetrads(_tokens.ToList());
            foreach (Tetrads tetrads in tetradsList)
            {
                Tetrads.Add(tetrads);
            }
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
            _tokens = _lexer.Scan(Text);
            foreach (Token token in _tokens)
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
                        InsertText();
                    }
                    break;

                case MessageBoxResult.No:
                    Text = string.Empty;
                    InsertText();
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
                        InsertText();
                        Open();
                        break;

                    case MessageBoxResult.No:
                        Text = string.Empty;
                        InsertText();
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
                InsertText();
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
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Radramor/CompilerDemo",
                UseShellExecute = true
            });
        }
        private void AboutProgram()
        {
            AboutProgramWindow window = new AboutProgramWindow();
            window.DataContext = this;
            window.Show();
        }
        public void FindMatchingSubstrings(string text, string pattern)
        {
            MatchResults.Clear();

            if (string.IsNullOrEmpty(text))
            {
                MatchResults.Add(new MatchResult { Substring = "Текст не введен.", Position = -1 });
                return;
            }

            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(text);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    MatchResults.Add(new MatchResult { Substring = match.Value, Position = match.Index });
                }
            }
            else
            {
                MatchResults.Add(new MatchResult { Substring = "Совпадений не найдено.", Position = 0 });
            }
        }

        public void FindIdentifier()
        {
            string text = Text;
            string pattern = @"[$a-zA-Z_][a-zA-Z]*";

            FindMatchingSubstrings(text, pattern);
        }

        public void FindUserName()
        {
            string text = Text;
            string pattern = @"\b[a-z0-9_-]{5,20}\b";

            FindMatchingSubstrings(text, pattern);
        }

        public void FindIPv6()
        {
            string text = Text;
            string pattern = @"(" +
                                @"([0-9a-fA-F]{1,4}:){1,6}:" +
                                @"((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}" +
                                @"(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|" +
                                @"fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,2}|" +
                                @"([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|" +
                                @"::(ffff(:0{1,4}){0,1}:){0,1}" +
                                @"((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}" +
                                @"(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|" +
                                @"([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|" +
                                @"([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|" +
                                @"([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|" +
                                @"([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|" +
                                @"([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|" +
                                @"[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|" +
                                @":((:[0-9a-fA-F]{1,4}){1,7}|:)|" +
                                @"([0-9a-fA-F]{1,4}:){1,7}:" +
                                @")"


            
            ;

            FindMatchingSubstrings(text, pattern);
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

        private void InsertText()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).RTB.Document.Blocks.Clear();
            ((MainWindow)System.Windows.Application.Current.MainWindow).RTB.AppendText(Text);
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

        public ObservableCollection<Tetrads> Tetrads
        {
            get { return _tetrads; }
            set { _tetrads = value; OnPropertyChanged(); }
        }
    }  
}


