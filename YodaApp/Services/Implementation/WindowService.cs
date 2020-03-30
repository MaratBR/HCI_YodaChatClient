using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YodaApp.Services.Implementation
{
    class WindowService : IWindowService
    {
        private readonly IWindowFactory factory;
        private Window main, login, signup, newRoom;
        private bool mainIsShown = false;

        public WindowService(IWindowFactory factory)
        {
            this.factory = factory;
        }

        public void ShowLogInWindow()
        {
            if (login == null)
            {
                login = factory.CreateLogInWindow();
            }

            login.Show();
        }

        public void ShowMainWindow()
        {
            if (main == null)
            {
                main = factory.CreateMainWindow();
            }


            mainIsShown = true;
            main.Show();
        }

        public void ShowSignUpWindow()
        {
            if (signup == null)
            {
                signup = factory.CreateSignUpWindow();
            }

            signup.Show();
            HideMainWindow();
        }

        public void CloseSignUpWindow()
        {
            if (signup != null)
            {
                signup.Close();
                signup = null;
            }
        }

        public void CloseLogInWindow()
        {
            if (login != null)
            {
                login.Close();
                login = null;
            }
        }

        public void HideMainWindow()
        {
            if (main != null && mainIsShown)
            {
                mainIsShown = false;
                main.Hide();
            }
        }

        public void ShowNewRoomWindow()
        {
            if (main == null)
                return;

            newRoom = factory.CreateNewRoomWindow();
            newRoom.Show();
        }

        public void CloseNewRoomWindow()
        {
            if (newRoom != null)
            {
                newRoom.Close();
                newRoom = null;
            }
        }
    }
}
