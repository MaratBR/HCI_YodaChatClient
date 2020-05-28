using System;
using System.Windows;

namespace YodaApp.Services.Implementation
{
    internal class WindowService : IAppUIService
    {
        private readonly IWindowFactory factory;
        private Window main, login, signup;
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
                signup.Closed += Signup_Closed;
            }

            signup.Show();
            HideMainWindow();
        }

        private void Signup_Closed(object sender, EventArgs e)
        {
            ShowLogInWindow();
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
    }
}