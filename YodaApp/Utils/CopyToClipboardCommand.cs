using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace YodaApp.Utils
{
    class CopyToClipboardCommand : ICommand
    {
        public static CopyToClipboardCommand Instance = new CopyToClipboardCommand();

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var text = parameter == null ? "(null)" : parameter.ToString();

            Clipboard.SetText(text);
        }
    }
}
