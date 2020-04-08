using MaterialDesignThemes.Wpf;
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
        private readonly IAppUIService _windows;

        public NewRoomViewModel(IAuthenticationService authentication, IAppUIService windows)
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

        public event EventHandler CloseForm;

        #region Commands

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
                await session.CreateRoomAsync(new YodaApiClient.CreateRoomRequest
                {
                    Name = Name,
                    Description = Description
                });
                CloseForm?.Invoke(this, EventArgs.Empty);
            }
            catch(ApiException exc)
            {
                Error = exc.Message;
            }
        }




        #endregion
    }
}
