using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YodaApp.Services
{
    public interface IWindowFactory
    {
        Window CreateSignUpWindow();

        Window CreateLogInWindow();

        Window CreateMainWindow();
    }
}
