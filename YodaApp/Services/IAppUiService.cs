using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApp.Services
{
    interface IAppUIService
    {
        void ShowMainWindow();

        void HideMainWindow();

        void ShowSignUpWindow();

        void CloseSignUpWindow();

        void ShowLogInWindow();

        void CloseLogInWindow();
    }
}
