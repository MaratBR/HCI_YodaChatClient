using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YodaApiClient;
using YodaApp.ViewModels;
using YodaApp.Views;

namespace YodaApp.Services.Implementation
{
    class WindowFactory : IWindowFactory
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

        public Window CreateNewRoomWindow()
        {
            return new NewRoomWindow
            {
                DataContext = _context.Resolve<NewRoomViewModel>()
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
