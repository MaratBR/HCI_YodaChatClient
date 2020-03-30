using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    class NewRoomViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authentication;
        private readonly IWindowService _windows;

        public NewRoomViewModel(IAuthenticationService authentication, IWindowService windows)
        {
            _windows = windows;
            _authentication = authentication;
        }

        #region Properties

        private string name;

        public string Name
        {
            get { return name; }
            set => Set(ref name, nameof(Name), value);
        }


        private string description;

        public string Description
        {
            get { return description; }
            set => Set(ref description, nameof(Description), value);
        }


        private string error;

        public string Error
        {
            get { return error; }
            set => Set(ref error, nameof(Error), value);
        }


        #endregion

        #region Commands

        private ICommand _cancelCommand;

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(CancelCommandHandler));

        private void CancelCommandHandler()
        {
            _windows.CloseNewRoomWindow();
        }

        private ICommand _submitCommand;

        public ICommand SubmitCommand => _submitCommand ?? (_submitCommand = new AsyncRelayCommand(Submit));

        private async Task Submit()
        {
            Error = null;

            var session = _authentication.GetCurrentSession();
            if (session == null)
                return;

            try
            {
                await session.CreateRoom(new YodaApiClient.CreateRoomRequest
                {
                    Name = Name,
                    Description = Description
                });
            }
            catch(ApiException exc)
            {
                Error = exc.Message;
            }
        }




        #endregion
    }
}
