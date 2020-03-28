using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YodaApp.Services.Implementation
{
    class StartUpService : IStartUpService
    {
        private readonly IWindowFactory _factory;
        private readonly IAuthenticationService _authentication;
        private bool started = false;
        private readonly object _lock = new object();

        public StartUpService(IWindowFactory factory, IAuthenticationService authentication)
        {
            _factory = factory;
            _authentication = authentication;
        }

        public async void Start()
        {
            lock(_lock)
            {
                if (started)
                    return;
                started = true;
            }

            Window mainWindow;
            await _authentication.Init();
            if (_authentication.HasAuthenticatedSession())
            {
                mainWindow = _factory.CreateMainWindow();
            }
            else
            {
                mainWindow = _factory.CreateLogInWindow();
            }


            mainWindow.Show();
        }
    }
}
