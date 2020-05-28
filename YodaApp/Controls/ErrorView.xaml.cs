using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YodaApp.ViewModels.Controls;

namespace YodaApp.Controls
{
    /// <summary>
    /// Interaction logic for ErrorView.xaml
    /// </summary>
    public partial class ErrorView : UserControl
    {
        private static bool IsShowingError = false;

        public ErrorView()
        {
            InitializeComponent();
        }

        public static Task Show(Exception exception)
        {
            string space = "", info = "";
            var exc = exception;

            while(exc != null)
            {
                info += space + exception.Message + "\n";
                space += "  ";
                exc = exc.InnerException;
            } 

            var vm = new ErrorViewModel 
            { 
                Type = exception.GetType().Name,
                Info = info,
                DetailedInfo = exception.ToString()
            };
            var v = new ErrorView { DataContext = vm };
            return DialogHost.Show(v, "ErrorDialog");
        }
    }
}
