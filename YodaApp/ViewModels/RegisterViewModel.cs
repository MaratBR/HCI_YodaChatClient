using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authentication;

        public RegisterViewModel(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }

        #region Properties

        private string userName;

        public string UserName
        {
            get { return userName; }
            set => Set(ref userName, nameof(UserName), value);
        }

        private string email;

        public string EMail
        {
            get { return email; }
            set => Set(ref email, nameof(EMail), value);
        }


        private string phone;

        public string Phone
        {
            get { return phone; }
            set => Set(ref phone, nameof(Phone), value);
        }

        private Gender gender;

        public Gender Gender
        {
            get { return gender; }
            set => Set(ref gender, nameof(Gender), value);
        }

        internal class GenderDeclaration
        {
            public BitmapImage Image { get; set; }
            public Gender? Gender { get; set; }
            public string Caption { get; set; }
        }

        public readonly List<GenderDeclaration> GenderDeclarations = new List<GenderDeclaration>
        {
            new GenderDeclaration
            {
                Caption = "Respect",
                Gender = Gender.Respect,
                Image = (BitmapImage)App.Current.Resources["RespectIcon"]
            },
            new GenderDeclaration
            {
                Caption = "Smooch",
                Gender = Gender.Smooch,
                Image = (BitmapImage)App.Current.Resources["SmoochIcon"]
            },
            new GenderDeclaration
            {
                Caption = "Alien",
                Gender = null,
                Image = (BitmapImage)App.Current.Resources["AlienIcon"]
            }
        };

        #endregion

        #region Commands

        private AbstractCommand _submitCommand;

        public AbstractCommand SubmitCommand => _submitCommand ?? (_submitCommand = new AsyncRelayCommand<PasswordBox>(Submit));

        private async Task Submit(PasswordBox passwordBox)
        {
            var pwd = passwordBox.Password;

            var request = new RegistrationRequest
            {
                Password = pwd,
                Email = email,
                Gender = Gender,
                UserName = UserName,
                PhoneNumber = Phone
            };
            try
            {
                await _authentication.GetApiProvider().RegisterUserAndCreateApi(request);
            }
            catch(ApiException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        #endregion
    }
}
