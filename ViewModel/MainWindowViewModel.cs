using CompilerDemo.View;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace CompilerDemo
{
    class MainWindowViewModel : ViewModelBase
    {
        private string path = string.Empty;
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }

        public ICommand CreateCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand ExitCommand { get; private set; }
        public ICommand ReferenceCommand { get; }
        public ICommand AboutProgramCommand { get; }

        public MainWindowViewModel()
        {
            CreateCommand = new RelayCommand(Create);
            OpenCommand = new RelayCommand(TryOpen);
            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            UndoCommand = new RelayCommand(Undo);
            RedoCommand = new RelayCommand(Redo);
            CutCommand = new RelayCommand(Cut);
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);
            DeleteCommand = new RelayCommand(Delete);
            SelectAllCommand = new RelayCommand(SelectAll);
            ExitCommand = new RelayCommand(Exit);
            ReferenceCommand = new RelayCommand(Reference);
            AboutProgramCommand = new RelayCommand(AboutProgram);
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
                RichTextBox rtb = ((MainWindow)System.Windows.Application.Current.MainWindow).RTB;
                TextRange doc = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                File.WriteAllText(path, doc.Text);
            }
            else Create();
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

        private void Undo()
        {

        }

        private void Redo()
        {
            RichTextBox rtb = ((MainWindow)System.Windows.Application.Current.MainWindow).RTB;
            rtb.Redo();
        }

        private void Cut()
        {
            RichTextBox rtb = ((MainWindow)System.Windows.Application.Current.MainWindow).RTB;
            rtb.Cut();
        }

        private void Copy()
        {
            RichTextBox rtb = ((MainWindow)System.Windows.Application.Current.MainWindow).RTB;
            rtb.Copy();
        }

        private void Paste()
        {
            RichTextBox rtb = ((MainWindow)System.Windows.Application.Current.MainWindow).RTB;
            rtb.Paste();
        }

        private void Delete()
        {
            RichTextBox rtb = ((MainWindow)System.Windows.Application.Current.MainWindow).RTB;
            rtb.Document.Blocks.Clear();
        }

        private void SelectAll()
        {
            RichTextBox rtb = ((MainWindow)System.Windows.Application.Current.MainWindow).RTB;
            rtb.SelectAll();
        }
        private void Reference()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo("Reference.html")
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

    }
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}


