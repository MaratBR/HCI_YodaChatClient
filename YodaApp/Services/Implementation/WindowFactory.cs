using Autofac;
using System.Windows;
using YodaApp.ViewModels;
using YodaApp.Views;

namespace YodaApp.Services.Implementation
{
    internal class WindowFactory : IWindowFactory
    {
        private readonly IComponentContext _context;

        public WindowFactory(IComponentContext context)
        {
            _context = context;
        }

        public Window CreateLogInWindow()
        {
            return new LoginWindow
            {
                DataContext = _context.Resolve<LoginViewModel>()
            };
        }

        public Window CreateMainWindow()
        {
            return new MainWindow
            {
                DataContext = _context.Resolve<MainWindowViewModel>()
            };
        }

        public Window CreateSignUpWindow()
        {
            return new RegisterWindow
            {
                DataContext = _context.Resolve<RegisterViewModel>()
            };
        }
    }
}