namespace YodaApp.Services
{
    internal interface IAppUIService
    {
        void ShowMainWindow();

        void HideMainWindow();

        void ShowSignUpWindow();

        void CloseSignUpWindow();

        void ShowLogInWindow();

        void CloseLogInWindow();
    }
}