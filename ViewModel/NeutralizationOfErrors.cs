using CompilerDemo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.ViewModel
{
    internal class NeutralizationOfErrors
    {
        public static void Neutralization (ObservableCollection<ParsingError> ParsingErrors, string Text)
        {
            string textBuff = Text;
            textBuff = textBuff.Replace("\r", "");
            if (ParsingErrors.Count == 0) return;
            while (ParsingErrors.Count > 0)
            {
                switch (ParsingErrors.First().ExpectedToken)
                {
                    case "{":
                        textBuff += "}";
                        ParsingErrors.Remove(ParsingErrors.First());
                        insertText(textBuff);
                        return;

                    case "(*":
                        textBuff += "*)";
                        ParsingErrors.Remove(ParsingErrors.First());
                        insertText(textBuff);
                        return;

                    case "":
                        break;
                    // }, *), //
                    default:
                        textBuff = textBuff.Remove(ParsingErrors.First().StartPos, ParsingErrors.First().EndPos - ParsingErrors.First().StartPos);
                        ParsingErrors.Remove(ParsingErrors.First());
                        insertText(textBuff);
                        return;
                }
            }
            
        }
        private static void insertText (string textBuff)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).RTB.Document.Blocks.Clear();
            ((MainWindow)System.Windows.Application.Current.MainWindow).RTB.AppendText(textBuff);
        }
    }
}
