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
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return parameter != null;
        }

        public void Execute(object parameter)
        {
            var text = parameter.ToString();

            Clipboard.SetText(text);
        }
    }
}
