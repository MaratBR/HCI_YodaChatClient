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
        private readonly IWindowService _windows;
        private readonly IAuthenticationService _authentication;
        private bool started = false;
        private readonly object _lock = new object();

        public StartUpService(IWindowService windows, IAuthenticationService authentication)
        {
            _windows = windows;
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

            await _authentication.Init();
            if (_authentication.HasAuthenticatedSession())
            {
                _windows.ShowMainWindow();
            }
            else
            {
                _windows.ShowLogInWindow();
            }


            mainWindow.Show();
        }
    }
}
