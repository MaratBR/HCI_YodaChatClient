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