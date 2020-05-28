using System;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApp.Utils;

namespace YodaApp.ViewModels.Controls
{
    internal class JoinRoomFormViewModel : ViewModelBase
    {
        private IApi api;

        public EventHandler Joined;

        public JoinRoomFormViewModel(IApi api)
        {
            this.api = api;
        }

        private string uuid;

        public string UUID
        {
            get { return uuid; }
            set
            {
                Set(ref uuid, nameof(UUID), value);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Guid result;
                    if (!Guid.TryParse(value, out result))
                        Error = "Invalid UUID";
                }
                else
                {
                    Error = null;
                }
            }
        }

        private string error;

        public string Error
        {
            get { return error; }
            set => Set(ref error, nameof(Error), value);
        }

        private ICommand joinCommand;

        public ICommand JoinCommand => joinCommand ?? (joinCommand = new AsyncRelayCommand(JoinCommandHandler));

        private async Task JoinCommandHandler()
        {
            Error = null;
            Guid result;
            if (Guid.TryParse(UUID, out result))
            {
                try
                {
                    await api.JoinRoomAsync(result);
                    Joined?.Invoke(this, EventArgs.Empty);
                }
                catch (ApiException exc)
                {
                    Error = exc.Message;
                }
            }
            else
            {
                Error = "Invalid UUID";
            }
        }
    }
}