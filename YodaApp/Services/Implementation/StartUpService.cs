namespace YodaApp.Services.Implementation
{
    internal class StartUpService : IStartUpService
    {
        private readonly IAppUIService _windows;
        private readonly IAuthenticationService _authentication;
        private bool started = false;
        private readonly object _lock = new object();

        public StartUpService(IAppUIService windows, IAuthenticationService authentication)
        {
            _windows = windows;
            _authentication = authentication;
        }

        public async void Start()
        {
            lock (_lock)
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
        }
    }
}